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
   class OnlyEnemiesFilter : IEntityFilter
   {
      private OnlyEnemiesFilter()
      {

      }

      public static IEntityFilter Instance { get; } = new OnlyEnemiesFilter();

      public bool Matches(Entity user, Entity target)
      {
         return OnlyCreaturesFilter.Instance.Matches(user, target) &&
            user.IsEnemy(target);
      }
   }
}
