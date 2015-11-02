using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Plans.EntityFilters
{
   public class OnlyCreaturesFilter : IEntityFilter
   {
      private OnlyCreaturesFilter()
      {
      }

      public bool Matches(Entity user, Entity target)
      {
         return target.HasComponent<CreatureComponent>() && target.HasComponent<CombatantComponent>();
      }

      public static IEntityFilter Instance { get; } = new OnlyCreaturesFilter();
   }
}
