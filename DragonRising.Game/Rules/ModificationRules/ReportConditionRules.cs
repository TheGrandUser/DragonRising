using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Events;
using DraconicEngine;

namespace DragonRising.Rules.ModificationRules
{
   class ReportStatusAddedRule : Rule<AddConditionEvent>
   {
      public override RuleResult Do(AddConditionEvent gameEvent, Scene scene)
      {
         var message = gameEvent.Condition.AppliedMessage(gameEvent.Entity);
         MessageService.Current.PostMessage(message, RogueColors.LightGreen);

         return RuleResult.Empty;
      }

      public override int Priority => 0;
   }

   class ReportStatusRemovedRule : Rule<ConditionRemovedEvent>
   {
      public override RuleResult Do(ConditionRemovedEvent gameEvent, Scene scene)
      {
         var message = gameEvent.Condition.RemovedMessage(gameEvent.Entity);
         MessageService.Current.PostMessage(message, RogueColors.LightGreen);

         return RuleResult.Empty;
      }
      public override int Priority => 0;
   }
}
