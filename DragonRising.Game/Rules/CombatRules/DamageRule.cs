using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using DraconicEngine.RulesSystem;
using DraconicEngine;

namespace DragonRising.Rules.CombatRules
{
   class DamageRule : Rule<InflictDamageEvent>
   {
      public override RuleResult Do(InflictDamageEvent damageParameters)
      {
         var targetCombatant = damageParameters.Target.GetComponent<CombatantComponent>();

         var finalDamage = damageParameters.Damage.Amount;

         // Check special types (werewolves vs. silver weapons), DR, etc. and adjust final damage, add other effects/events

         targetCombatant.HP -= finalDamage;

         if (targetCombatant.HP <= 0)
         {
            targetCombatant.IsAlive = false;
            targetCombatant.Owner.SetBlocks(false);
            
            var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            var creatureKilledEvent = eventAggregator.GetEvent<CreatureKilledPubSubEvent>();
            creatureKilledEvent.Publish(targetCombatant.Owner, "Entity", damageParameters.Initiator);

            return
               new RuleResult(
                  new CreatureKilledEvent(
                     creatureKilled: damageParameters.Target,
                     cause: "Entity",
                     killingEntity: damageParameters.Initiator));
         }

         return RuleResult.Empty;
      }
   }

   class ReportDamageRule : Rule<InflictDamageEvent>
   {
      public override RuleResult Do(InflictDamageEvent gameEvent)
      {
         if (gameEvent.Damage.Type != "Normal")
         {
            MessageService.Current.PostMessage(gameEvent.Target.Name + $"suffers {gameEvent.Damage.Amount} {gameEvent.Damage.Type} damage.", RogueColors.LightBlue);
         }
         else
         {
            MessageService.Current.PostMessage(gameEvent.Target.Name + $"suffers {gameEvent.Damage.Amount} damage.", RogueColors.LightBlue);
         }

         return RuleResult.Empty;
      }
   }
}