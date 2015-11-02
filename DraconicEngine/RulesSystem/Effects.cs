using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.RulesSystem
{
   public interface IEntityEffect
   {
      IEnumerable<Fact> GetFacts(Entity initiator, Entity target, Scene context);
   }

   public interface ILocationEffect
   {
      IEnumerable<Fact> GetFacts(Entity initiator, Loc target, Scene context);
   }

   public interface IAreaEffect
   {
      IEnumerable<Fact> GetFacts(Entity initiator, Area target, Scene context);
   }
   
}
