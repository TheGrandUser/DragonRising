using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.Behaviors;
using DraconicEngine.Powers.Nodes;
using DraconicEngine.Timers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Effects
{
   public class BehaviorReplacementEffect : IEffect
   {
      CreatureNodeInput targetInput = new CreatureNodeInput();
      public CreatureNodeInput TargetInput { get { return targetInput; } }
      NumberNodeInput durationInput = new NumberNodeInput();
      public NumberNodeInput DurationInput { get { return durationInput; } }

      public Behavior BehaviorTemplate { get; set; }
      public string BeginMessageTemplate { get; set; }
      public string EndMessageTemplate { get; set; }

      public void Do(Entity initiator)
      {
         var targets = this.targetInput.Value;
         foreach (var target in targets)
         {
            var newBehavior = this.BehaviorTemplate.Clone();
            target.GetComponent<BehaviorComponent>().PushBehavior(newBehavior);
            var duration = this.durationInput.Value;

            if (!string.IsNullOrEmpty(this.BeginMessageTemplate))
            {
               MessageService.Current.PostMessage(string.Format(this.BeginMessageTemplate, target, initiator), RogueColors.LightGreen);
            }

            var setTimer =
               string.IsNullOrEmpty(EndMessageTemplate) ?
               new TurnTimer(duration, initiator, new RemoveBehaviorEffect(newBehavior, target)) :
               new TurnTimer(duration, initiator,
                  new RemoveBehaviorEffect(newBehavior, target),
                  new MessageEffect(string.Format(this.EndMessageTemplate, target, initiator), RogueColors.Red));

            target.AttachTimer(setTimer);
         }
      }
   }
}