using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Tinkering.Graph.Datums
{
   internal class SingleInputHandler : InputHandler
   {
      Link @in;
      private Datum datum;

      public SingleInputHandler(Datum datum)
         : base(datum)
      {
         this.datum = datum;
      }

      public override dynamic GetValue()
      {
         Debug.Assert(@in != null);
         Debug.Assert(@in.Parent is Datum);
         var source = @in.Parent as Datum;

         Debug.Assert(Parent is Datum);
         var target = Parent as Datum;

         if (target.ConnectUpstream(source) && source.IsValid)
         {
            var v = source.Value;

            return v;
         }
         else
         {
            return null;
         }
      }

      public override bool Accepts(Link upstream)
      {
         Debug.Assert(Parent is Datum);
         Debug.Assert(upstream.Parent is Datum);

         return @in == null &&
            ((Datum)Parent).PyType ==
            ((Datum)upstream.Parent).PyType;
      }

      public override void AddInput(Link input)
      {
         Debug.Assert(@in == null);

         @in = input;
         Debug.Assert(Parent is Datum);

         ((Datum)Parent).Update();
      }

      public override void DeleteInput(Datum upstream)
      {
         Debug.Assert(@in != null && @in.Parent == upstream);
         @in = null;

      }

      public override ImmutableList<Link> GetLinks()
      {
         if (@in != null)
            return ImmutableList.Create(@in);
         return ImmutableList<Link>.Empty;
      }

      public override string GetString()
      {
         Debug.Assert(@in != null);
         Debug.Assert(@in.Parent is Datum);

         return ((Datum)@in.Parent).GetString();
      }

      public override bool HasInput()
      {
         if(@in == null)
         {
            return false;
         }

         var d = @in.Parent as Datum;

         return d != null;
      }
   }
}