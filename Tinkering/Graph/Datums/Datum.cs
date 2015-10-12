using IronPython.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Tinkering.Graph.Nodes;

namespace Tinkering.Graph.Datums
{
   abstract class Datum : HObject
   {
      protected dynamic value = null;
      protected bool valid;

      protected bool editable;
      protected string repr;

      protected InputHandler inputHandler;

      protected List<Datum> upstream = new List<Datum>();
      protected bool postInitCalled;

      Subject<HObject> disconnectFrom = new Subject<HObject>();
      Subject<HObject> changed = new Subject<HObject>();

      public Datum(string name, Node parent)
         : base(parent)
      {
         Name = name;
      }

      public bool IsValid { get; internal set; }
      public dynamic Value => value;

      public abstract DatumType DatumType { get; }
      public abstract Type PyType { get; }

      public static Datum FromTypeString(string type, string name, Node parent)
      {
         if (type == FloatDatum.TypeString)
            return new FloatDatum(name, parent);
         else if (type == FloatOutputDatum.TypeString)
            return new FloatOutputDatum(name, parent);
         else if (type == IntDatum.TypeString)
            return new IntDatum(name, parent);
         else if (type == NameDatum.TypeString)
            return new NameDatum(name, parent);
         else if (type == StringDatum.TypeString)
            return new StringDatum(name, parent);
         else if (type == ScriptDatum.TypeString)
            return new ScriptDatum(name, parent);
         else if (type == ShapeOutputDatum.TypeString)
            return new ShapeOutputDatum(name, parent);
         else if (type == ShapeDatum.TypeString)
            return new ShapeDatum(name, parent);

         Debug.Assert(false);
         return null;
      }

      public bool HasInput => inputHandler != null;
      public bool HasInputValue => inputHandler?.HasInput() ?? false;

      public ImmutableList<Link> InputLinks => inputHandler?.GetLinks() ?? ImmutableList<Link>.Empty;

      public virtual bool HasOutput => true;
      public bool HasConnectedLink => FindChildren<Link>().FirstOrDefault(l => l.HasTarget) != null;

      public virtual bool CanEdit => !HasInputValue;
      public abstract string GetString();

      public Link LinkFrom()
      {
         var link = new Link(this);

         Connect(link, link.Destroyed, ConnectionChanged.OnNext);
         return link;
      }

      public bool AcceptsLink(Link upstream)
      {
         if ((upstream.Parent as Datum)?.upstream.Contains(this) == true)
         {
            return false;
         }
         return inputHandler.Accepts(upstream);
      }

      public void AddLink(Link input)
      {
         inputHandler.AddInput(input);
         input.Target = new WeakReference<Datum>(this);
         Connect(this, this.Destroyed, input, input.DeleteLater);
         Connect(input, input.Destroyed, this, _ => Update());
      }

      public void DeleteLink(Datum upstream)
      {
         inputHandler.DeleteInput(upstream);
      }

      public bool ConnectUpstream(Datum upstream)
      {
         this.upstream.AddRange(upstream.upstream);
         Connect(upstream, upstream.Changed, this, _ => Update(), ConnectionType.UniqueConnection);
         Connect(upstream, upstream.Destroyed, this, _ => Update(), ConnectionType.UniqueConnection);
         Connect(upstream, upstream.DisconnectFrom, this, OnDisconnectRequest, ConnectionType.UniqueConnection);

         return upstream.upstream.Contains(this) ? false : true;
      }

      public ImmutableList<Datum> GetInputDatums() => inputHandler?.GetInputDatums() ?? ImmutableList<Datum>.Empty;

      public NodeRoot Root
      {
         get
         {
            Debug.Assert(Parent is Node);
            Debug.Assert(Parent.Parent is NodeRoot);

            return (NodeRoot)Parent.Parent;
         }
      }

      public ISubject<Datum> Changed { get; private set; }
      public ISubject<Datum> DisconnectFrom { get; private set; }

      public ISubject<HObject> ConnectionChanged { get; private set; }

      public void Update()
      {
         if(IsRecursing() || !(Parent is Node) || !(Parent.Parent is NodeRoot))
         {
            return;
         }

         if (!postInitCalled)
         {
            PostInit();
         }

         disconnectFrom.OnNext(this);
         upstream.Clear();
         upstream.Add(this);

         dynamic newValue = HasInputValue ?
            inputHandler.GetValue() :
            GetCurrentValue();

         var hasChanged = false;
         if(newValue == null && valid)
         {
            valid = false;
            hasChanged = true;
         }
         else if(newValue != null &&(!valid || newValue != value))
         {
            // value decrease ref
            value = newValue;
            // value increase ref
            valid = true;
            hasChanged = true;
         }

         if(CanEdit != editable)
         {
            editable = CanEdit;
            hasChanged = true;
         }

         if(GetString() != repr)
         {
            repr = GetString();
            hasChanged = true;
         }

         if (hasChanged)
         {
            changed.OnNext(this);
         }
      }

      public void OnDisconnectRequest(Datum downstream)
      {

         Disconnect(downstream, downstream.DisconnectFrom, this, OnDisconnectRequest);
      }

      protected virtual bool IsRecursing() => false;

      protected abstract dynamic GetCurrentValue();

      protected virtual void PostInit()
      {
         postInitCalled = true;

         value = this.PyType;// Have python create the default value

         var p = Parent as Node;
         var n = p?.GetDatum<NameDatum>("__name");
         if(n != null)
         {
            Root.OnNameChanged(n.Expr + "." + Name);
         }
      }
   }

   enum DatumType
   {
      FLOAT,
      FLOAT_OUTPUT,
      INT,
      NAME,
      SCRIPT,
      STRING,
      SHAPE_OUTPUT,
      SHAPE_FUNCTION, // DEPRECATED, but still here to preserve numbering
      SHAPE_INPUT, // DEPRECATED, but still here to preserve numbering
      SHAPE
   }
}
