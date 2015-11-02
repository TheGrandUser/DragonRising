using DraconicEngine;
using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using DraconicEngine.Services;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Rules.ModificationRules
{
   class AddConditionRule : Rule<AddConditionEvent>
   {
      public override RuleResult Do(AddConditionEvent gameEvent, Scene scene)
      {
         // Check if status is applicable to entity, if not, interupt further rules

         var conditionComponent = gameEvent.Entity.GetComponent<ConditionComponent>();
         conditionComponent.AddStatus(gameEvent.Condition);

         if (gameEvent.Duration.HasValue)
         {
            TimedEvents.Current.AddFromNow(gameEvent.Duration.Value, new ConditionExpiredEvent(gameEvent.Entity, gameEvent.Condition));
         }

         return RuleResult.Empty;
      }
   }
}
