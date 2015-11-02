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
using DragonRising.Plans;

namespace DragonRising.GameWorld.Powers
{
   public static class ConfuseNearestPower
   {
      public static EffectPlan CreateConfusionPlan(int range = 8, int duration = 10)
      {
         return EffectPlan.CreatePower("Confuse nearest")
            .Add(new SelectClosestCreatureQuery(8, new ApplyTemporaryCondition(ConfusedCondition.Instance, 10), OnlyEnemiesFilter.Instance))
            .Finish();
      }
   }
}
