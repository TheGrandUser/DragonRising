using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Tinkering.Graph.Datums
{
   abstract class InputHandler : HObject
   {
      public InputHandler(Datum parent)
         : base(parent)
      {
      }

      public abstract dynamic GetValue();

      public abstract bool Accepts(Link upstream);

      public abstract void AddInput(Link input);

      public abstract bool HasInput();

      public abstract void DeleteInput(Datum upstream);

      public abstract string GetString();

      public ImmutableList<Datum> GetInputDatums()
      {
         AssertLinksParentsAreDatums();

         return GetLinks().Select(l => l.Parent as Datum).ToImmutableList();
      }
      
      public abstract ImmutableList<Link> GetLinks();

      [Conditional("DEBUG")]
      void AssertLinksParentsAreDatums()
      {
         foreach (var link in GetLinks())
         {
            Debug.Assert(link.Parent is Datum);
         }
      }
   }
}