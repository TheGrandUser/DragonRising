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
   public class BehaviorReplacementItem : Component, IItemUsage
   {
      Func<IBehavior> controllerFactory;
      int duration;
      int range;

      string verb;
      Func<Entity, string> beginMessage;
      Func<Entity, string> endMessage;

      Entity target = null;

      public BehaviorReplacementItem(Func<IBehavior> controllerFactory,
         string verb,
         Func<Entity, string> beginMessage,
         Func<Entity, string> endMessage,
         int duration = 10, int range = 8)
      {
         this.controllerFactory = controllerFactory;
         this.duration = duration;
         this.verb = verb;
         this.beginMessage = beginMessage;
         this.endMessage = endMessage;
         this.range = range;
      }

      public static BehaviorReplacementItem CreateConfusionItem()
      {
         return new BehaviorReplacementItem(() => new ConfusedBehavior(),
            "confuse",
            c => "The eyes of the " + c.Name + " look vacant, as he starts to stumble around.",
            c => "The " + c.Name + " is no longer confused.");
      }

      public void Use(Entity user)
      {
         var behavior = controllerFactory();
         var controller = this.target.GetComponent<BehaviorComponent>();
         controller.PushBehavior(behavior);
         MessageService.Current.PostMessage(beginMessage(this.target), RogueColors.LightGreen);

         var setTimer = new PopBehaviorTimer(this.duration, this.target);
         var messageTimer = new MessageTimer(this.duration, endMessage(this.target), RogueColors.Red);

         this.target.AttachTimer(setTimer);
         this.target.AttachTimer(messageTimer);

         this.target = null;
      }

      public ItemUseResult PrepUse(Entity user, Some<RequirementFulfillment> fulfillment)
      {
         //Entity creature = await (user.GetComponentOrDefault<SelectorComponent>()?.Selector.SelectTargetCreature(user, range: this.range) ?? Task.FromResult<Entity>(null));
         var closestMonster = Scene.CurrentScene.ClosestEnemy(user, range);

         return closestMonster.Match(
            Some: monster =>
            {
               this.target = monster;
               return ItemUseResult.Used;
            },
            None: () =>
            {
               MessageService.Current.PostMessage("No creature close enough to " + verb + ".");
               return ItemUseResult.NotUsed;
            });
      }

      public IItemUsageTemplate Template
      {
         get { throw new NotImplementedException(); }
      }

      public ActionRequirement Requirements => NoRequirement.None;
   }
}