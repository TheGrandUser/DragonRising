using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Effects;
using DragonRising.GameWorld.Events;
using DragonRising.GameWorld.Conditions;
using DragonRising.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.RulesSystem;

namespace DragonRising.Rules.ModificationRules
{
   class OnConfusedStatusAddedRule : Rule<AddConditionEvent>
   {
      public override bool UseFilter => true;
      protected override bool Filter(AddConditionEvent gameEvent) => gameEvent.Condition is ConfusedCondition;

      public override RuleResult Do(AddConditionEvent gameEvent)
      {
         var target = gameEvent.Entity;

         var confusedBehavior = Library.Current.Behaviors.Get("Confused");
         target.GetComponent<BehaviorComponent>().PushBehavior(confusedBehavior);
         
         return RuleResult.Empty;
      }

      public override int Priority => 5;
   }

   class OnConfusedStatusRemovedRule : Rule<ConditionRemovedEvent>
   {
      public override bool UseFilter => true;
      protected override bool Filter(ConditionRemovedEvent gameEvent) => gameEvent.Condition is ConfusedCondition;

      public override RuleResult Do(ConditionRemovedEvent gameEvent)
      {
         var target = gameEvent.Entity;
         var behaviors = target.GetComponent<BehaviorComponent>();

         behaviors.RemoveBehavior("Confused");

         return RuleResult.Empty;
      }
   }
}
