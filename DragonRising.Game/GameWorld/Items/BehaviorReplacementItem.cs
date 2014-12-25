using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.Timers;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Items;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.Behaviors;
using DraconicEngine.GameWorld.Actions.Requirements;
using LanguageExt;
using LanguageExt.Prelude;

namespace DragonRising.Entities.Items
{
   public class BehaviorReplacementItem : IItemUsage
   {
      Behavior behaviorPrototype;
      int duration;
      int range;

      string verb;
      Func<Entity, string> beginMessage;
      Func<Entity, string> endMessage;

      public BehaviorReplacementItem(Behavior behaviorFactory,
         string verb,
         Func<Entity, string> beginMessage,
         Func<Entity, string> endMessage,
         int duration = 10, int range = 8)
      {
         this.behaviorPrototype = behaviorFactory;
         this.duration = duration;
         this.verb = verb;
         this.beginMessage = beginMessage;
         this.endMessage = endMessage;
         this.range = range;
      }

      public static BehaviorReplacementItem CreateConfusionItem()
      {
         return new BehaviorReplacementItem(new ConfusedBehavior(),
            "confuse",
            c => "The eyes of the " + c.Name + " look vacant, as he starts to stumble around.",
            c => "The " + c.Name + " is no longer confused.");
      }

      public bool Use(Entity user, Some<RequirementFulfillment> fulfillment)
      {
         var closestMonster = Scene.CurrentScene.ClosestEnemy(user, range);

         return closestMonster.Match(
            Some: target =>
            {
               var behavior = behaviorPrototype.Clone();
               var controller = target.GetComponent<BehaviorComponent>();
               controller.PushBehavior(behavior);
               MessageService.Current.PostMessage(beginMessage(target), RogueColors.LightGreen);

               var setTimer = new PopBehaviorTimer(this.duration, target);
               var messageTimer = new MessageTimer(this.duration, endMessage(target), RogueColors.Red);

               target.AttachTimer(setTimer);
               target.AttachTimer(messageTimer);

               return true;
            },
            None: () => false);
      }

      public ActionRequirement Requirements => NoRequirement.None;
   }
}