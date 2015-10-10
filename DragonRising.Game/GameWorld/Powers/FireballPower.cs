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
   public class FireballPower : EffectPlan
   {
      private readonly LocationInRangeTargeter targeter;

      public override IEnumerable<ILocationBasedTargeter> Targeters => EnumerableEx.Return(targeter);
      
      public FireballPower(int radius = 3, int damage = 12, int senseIntensity = 12)
         : base("Fireball")
      {
         targeter = LocationInRangeTargeter.Build(new SelectionRange(null, RangeLimits.LineOfEffect))
               .Add(new AffectAllInRangeQuery(radius,
                  OnlyCreaturesFilter.Instance,
                  new DamageEffect(new Damage(damage, "Fire"))))
               .Add(new SensoryEffect("A great ball of fire bursts.",
                  new Sensed(Sense.Sound, "Explosion", "Fiery", senseIntensity),
                  new Sensed(Sense.Sight, "Flame", "Bright", senseIntensity))).Finish();
      }
   }

}
