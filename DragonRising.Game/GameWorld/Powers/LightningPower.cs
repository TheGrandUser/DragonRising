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
using IFromLocationTargeter = DraconicEngine.RulesSystem.IFromLocationTargeter<DragonRising.Scene>;
using IFromLocationQuery = DraconicEngine.RulesSystem.IFromLocationQuery<DragonRising.Scene>;
using TargetResult = DraconicEngine.RulesSystem.TargetResult<DragonRising.Scene>;
using LocationTargetResult = DraconicEngine.RulesSystem.LocationTargetResult<DragonRising.Scene>;

namespace DragonRising.GameWorld.Powers
{
   public class LightningPower : EffectPlan
   {
      IFromLocationTargeter targeter;
      public LightningPower(int range = 5, int damage = 20)
         : base("Lightning bolt")
      {
         var strikingMessage = fun<Entity, RogueMessage>(target => new RogueMessage($"A lightning bolt strikes the {target.Name} with a loud thunder!", RogueColors.LightBlue));
         //var damageMessage = fun<InflictDamageEvent, RogueMessage>(de => new RogueMessage($"The damage is {de.Damage.Amount} hit points.", RogueColors.LightBlue));
         
         
         var damageEffect = new DamageEffect(new Damage(damage, "Lightning"));


         var selectionRange = new SelectionRange(range, RangeLimits.LineOfSight | RangeLimits.LineOfEffect);

         targeter = new EntityInRangeTargeter(
            selectionRange,
            "A lightning bolt strikes the {target} with a loud thunder!", RogueColors.LightBlue,
            Enumerable.Empty<IFromLocationTargeter>(),
            Enumerable.Empty<IFromLocationQuery>(),
            EnumerableEx.Return(damageEffect));
      }

      public override IEnumerable<IFromLocationTargeter> Targeters => EnumerableEx.Return(targeter);
   }
}
