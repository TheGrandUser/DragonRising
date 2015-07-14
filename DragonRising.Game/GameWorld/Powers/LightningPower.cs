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
   public class LightningPower : EffectPlan
   {
      ILocationBasedTargeter targeter;
      public LightningPower(int range = 5, int damage = 20)
         : base("Lightning bolt")
      {

         var strikingMessage = fun<Entity, RogueMessage>(target => new RogueMessage($"A lightning bolt strikes the {target.Name} with a loud thunder!", RogueColors.LightBlue));
         var damageMessage = fun<InflictDamageEvent, RogueMessage>(de => new RogueMessage($"The damage is {de.Damage.Amount} hit points.", RogueColors.LightBlue));
         //MessageService.Current.PostMessage("A lightning bolt strikes the " + target.Name + " with a loud thunder! The damage is " + damage + " hit points.", RogueColors.LightBlue);

         targeter = new EntityInRangeTargeter(
            new SelectionRange(range, RangeLimits.LineOfSight | RangeLimits.LineOfEffect), Enumerable.Empty<ILocationBasedTargeter>(),
            Enumerable.Empty<ILocationBasedQuery>(),
            EnumerableEx.Return(new DamageEffect(new Damage(damage, "Lightning"))));
      }

      public override IEnumerable<ILocationBasedTargeter> Targeters => EnumerableEx.Return(targeter);
   }
}
