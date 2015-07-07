using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.RulesSystem
{
   public interface IEffect
   {
   }

   public interface IEntityEffect : IEffect
   {
      IEnumerable<Fact> GetFacts(Entity initiator, Entity target);
   }

   public interface ILocationEffect : IEffect
   {
      IEnumerable<Fact> GetFacts(Entity initiator, Loc target);
   }

   public interface IAreaEffect : IEffect
   {
      IEnumerable<Fact> GetFacts(Entity initiator, Area target);
   }

   public static class Effects
   {
      public static T Match<T>(this IEffect self,
         Func<IEntityEffect, T> entity,
         Func<ILocationEffect, T> loc,
         Func<IAreaEffect, T> area)
      {
         if (self is IEntityEffect)
         {
            return entity((IEntityEffect)self);
         }
         else if (self is ILocationEffect)
         {
            return loc((ILocationEffect)self);
         }
         else if (self is IAreaEffect)
         {
            return area((IAreaEffect)self);
         }
         throw new ArgumentException("Unknown effect type");
      }
   }
}
