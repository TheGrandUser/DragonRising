using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.EntitySystem;
using DragonRising.Commands.Requirements;
using LanguageExt;
using static LanguageExt.Prelude;
using DragonRising.GameWorld.Components;
using System.Collections.Immutable;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Alligences;
using DragonRising.Plans.Effects;
using DragonRising.Plans.Targeters;
using DragonRising.Rules.CombatRules;
using DragonRising.GameWorld.Events;
using DragonRising.Plans;

namespace DragonRising.GameWorld.Powers
{
   public static class LightningPower
   {
      public static EffectPlan CreateEffect(int range = 5, int damage = 20)
      {
         //var strikingMessage = fun<Entity, RogueMessage>(target => new RogueMessage($"A lightning bolt strikes the {target.Name} with a loud thunder!", RogueColors.LightBlue));
         //var damageMessage = fun<InflictDamageEvent, RogueMessage>(de => new RogueMessage($"The damage is {de.Damage.Amount} hit points.", RogueColors.LightBlue));
         
         return EffectPlan.CreatePower("Lightning bolt").Add(new EntityInRangeTargeter(
            new SelectionRange(range, RangeLimits.LineOfSight | RangeLimits.LineOfEffect),
            "A lightning bolt strikes the {target} with a loud thunder!", RogueColors.LightBlue,
            Enumerable.Empty<IFromLocationTargeter>(),
            Enumerable.Empty<IFromLocationQuery>(),
            EnumerableEx.Return(new DamageEffect(new Damage(damage, "Lightning"))))).Finish();
      }
   }
}
