using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.RulesSystem;
using DragonRising.Plans.Queries;
using DragonRising.GameWorld.Effects;
using DragonRising.GameWorld.Conditions;
using DragonRising.Plans.EntityFilters;

namespace DragonRising.GameWorld.Powers
{
   class ConfuseNearestPower : Power
   {
      ILocationBasedQuery query;

      public ConfuseNearestPower(int range = 8, int duration = 10)
         : base("Confuse nearest")
      {
         this.query = new SelectClosestCreatureQuery(
            range, new ApplyTemporaryCondition(ConfusedCondition.Instance, duration),
            OnlyEnemiesFilter.Instance);
            
      }
      
      public override IEnumerable<ILocationBasedQuery> Queries => EnumerableEx.Return(query);
   }
}
