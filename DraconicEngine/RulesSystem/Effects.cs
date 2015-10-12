using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.RulesSystem
{
   public interface IEntityEffect<TContext>
   {
      IEnumerable<Fact> GetFacts(Entity initiator, Entity target, TContext context);
   }

   public interface ILocationEffect<TContext>
   {
      IEnumerable<Fact> GetFacts(Entity initiator, Loc target, TContext context);
   }

   public interface IAreaEffect<TContext>
   {
      IEnumerable<Fact> GetFacts(Entity initiator, Area target, TContext context);
   }
   
}
