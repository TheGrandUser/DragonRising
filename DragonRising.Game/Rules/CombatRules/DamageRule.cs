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
using DragonRising.GameWorld;

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
         if (World.Current.Scene.IsVisible(gameEvent.Target.Location))
         {
            var isSelf = gameEvent.Target == World.Current.Player;
            var prefix = isSelf ? "You suffer" : $"The {gameEvent.Target.Name} suffers";
            var message = gameEvent.Damage.Type != "Normal" ?
               $"{prefix} {gameEvent.Damage.Amount} {gameEvent.Damage.Type} damage." :
               $"{prefix} {gameEvent.Damage.Amount} damage.";

            MessageService.Current.PostMessage(message,
                  isSelf ? RogueColors.Red : RogueColors.LightBlue);
         }

         return RuleResult.Empty;
      }

      public override int Priority => 1;
   }

   class ReportInteruptedDamageRule : Rule<FactInterupted<InflictDamageEvent>>
   {
      public override int Priority => 1;

      public override RuleResult Do(FactInterupted<InflictDamageEvent> gameEvent)
      {
         if (World.Current.Scene.IsVisible(gameEvent.Fact.Target.Location))
         {
            if (gameEvent.Reason == "Negated")
            {
               var isSelf = gameEvent.Fact.Target == World.Current.Player;
               var prefix = isSelf ? "You ignore" : $"The {gameEvent.Fact.Target.Name} ignores";
               var message = gameEvent.Fact.Damage.Type != "Normal" ?
                  $"{prefix} {gameEvent.Fact.Damage.Amount} {gameEvent.Fact.Damage.Type} damage." :
                  $"{prefix} {gameEvent.Fact.Damage.Amount} damage.";

               MessageService.Current.PostMessage(message,
                  isSelf ? RogueColors.LightBlue : RogueColors.Yellow);
            }
         }

         return RuleResult.Empty;
      }
   }
}