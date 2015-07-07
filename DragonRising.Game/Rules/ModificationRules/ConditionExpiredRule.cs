using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Rules.ModificationRules
{
   class ConditionExpiredRule : Rule<ConditionExpiredEvent>
   {
      public override RuleResult Do(ConditionExpiredEvent gameEvent)
      {
         var conditionComponent = gameEvent.Entity.GetComponent<ConditionComponent>();

         conditionComponent.RemoveStatus(gameEvent.Condition);

         return new RuleResult(new ConditionRemovedEvent(gameEvent.Entity, gameEvent.Condition));
      }
   }
}
