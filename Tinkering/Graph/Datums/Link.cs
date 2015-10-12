using System;

namespace Tinkering.Graph.Datums
{
   class Link : HObject
   {
      public Link(Datum parent)
         : base(parent)
      {

      }

      public bool HasTarget => Target.HasReference();

      public WeakReference<Datum> Target { get; set; }
   }

   static class WeakReferenceExtensions
   {
      public static bool HasReference<T>(this WeakReference<T> wr)
         where T : class
      {
         T t;
         return wr.TryGetTarget(out t);
      }
   }
}

