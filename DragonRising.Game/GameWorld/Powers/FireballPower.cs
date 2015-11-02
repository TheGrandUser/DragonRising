using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using System.Collections.Immutable;
using DraconicEngine;
using DragonRising.Commands.Requirements;
using DragonRising.GameWorld.Effects;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Events;
using DragonRising.Rules.CombatRules;
using DragonRising.GameWorld.Events.SensoryEvents;
using DragonRising.Plans.Effects;
using DragonRising.Plans.Queries;
using DragonRising.Plans.Targeters;
using DragonRising.Plans.EntityFilters;
using DragonRising.Plans;

namespace DragonRising.GameWorld.Powers
{
   public static class FireballPower
   {
      public static EffectPlan CreateFireballEffectPlan(int radius = 3, int damage = 12, int senseIntensity = 12)
      {
         return EffectPlan.CreatePower("Fireball").Add(LocationInRangeTargeter.Build(new SelectionRange(null, RangeLimits.LineOfEffect))
               .Add(new AffectAllInRangeQuery(3,
                  OnlyCreaturesFilter.Instance,
                  new DamageEffect(new Damage(12, "Fire"))))
               .Add(new SensoryEffect("A great ball of fire bursts.",
                  new Sensed(Sense.Sound, "Explosion", "Fiery", 12),
                  new Sensed(Sense.Sight, "Flame", "Bright", 12))).Finish()).Finish();
      }
   }
}
