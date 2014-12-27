using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.Behaviors;
using DraconicEngine.Powers.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Services;
using System.Text.RegularExpressions;

namespace DraconicEngine.GameWorld.Effects
{
   public class BehaviorReplacementEffect : IEffect
   {
      Entity initiator;
      Entity target;
      int duration;

      public Behavior BehaviorTemplate { get; set; }
      public string BeginMessageTemplate { get; set; }
      public string EndMessageTemplate { get; set; }

      public BehaviorReplacementEffect(Entity initiator, Entity target, int duration, Behavior behaviorTemplate)
      {
         this.initiator = initiator;
         this.target = target;
         this.duration = duration;
         this.BehaviorTemplate = behaviorTemplate;
      }

      public void Do()
      {

         //var behavior = behaviorPrototype.Clone();
         //var controller = target.GetComponent<BehaviorComponent>();
         //controller.PushBehavior(behavior);
         //MessageService.Current.PostMessage(beginMessage(target), RogueColors.LightGreen);

         //var setTimer = new TurnTimer(this.duration, user,
         //   new RemoveBehaviorEffect(behavior, target),
         //   new MessageEffect(endMessage(target), RogueColors.Red));

         //target.AttachTimer(setTimer);




         var targets = this.target;
         var newBehavior = this.BehaviorTemplate.Clone();
         target.GetComponent<BehaviorComponent>().PushBehavior(newBehavior);
         var duration = this.duration;
         
         if (!string.IsNullOrEmpty(this.BeginMessageTemplate))
         {
            var beginMessage = Regex.Replace(this.BeginMessageTemplate, @"\{[a-zA-Z][a-zA-Z0-9]*\}", match =>
            {
               switch (match.Value)
               {
                  case "{user}":
                     return initiator.Name;
                  case "{duration}":
                     return duration.ToString();
                  case "{target}":
                     return target.Name;
                  default:
                     return match.Value;
               }
            });

            MessageService.Current.PostMessage(beginMessage, RogueColors.LightGreen);
         }


         if (string.IsNullOrEmpty(EndMessageTemplate))
         {
            TimedEvents.Current.Add(duration, new RemoveBehaviorEffect(newBehavior, target));
         }
         else
         {
            var endMessage = Regex.Replace(this.EndMessageTemplate, @"\{[a-zA-Z][a-zA-Z0-9]*\}", match =>
            {
               switch (match.Value)
               {
                  case "{user}":
                     return initiator.Name;
                  case "{duration}":
                     return duration.ToString();
                  case "{target}":
                     return target.Name;
                  default:
                     return match.Value;
               }
            });

            TimedEvents.Current.Add(duration,
               new RemoveBehaviorEffect(newBehavior, target),
               new MessageEffect(endMessage, RogueColors.Red));
         }
      }
   }
}