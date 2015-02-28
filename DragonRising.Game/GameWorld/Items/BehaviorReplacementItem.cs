using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DragonRising.GameWorld.Items;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.Behaviors;
using DraconicEngine.GameWorld.Actions.Requirements;
using LanguageExt;
using static LanguageExt.Prelude;
using DraconicEngine.GameWorld.Effects;
using DraconicEngine.Services;
using DragonRising.Storage;
using DragonRising.GameWorld.Effects;

namespace DragonRising.GameWorld.Items
{
   public class BehaviorReplacementItem : IItemUsage
   {
      Behavior behaviorPrototype;
      int duration;
      int range;

      string verb;
      string beginMessageTemplate;
      string endMessageTemplate;

      public BehaviorReplacementItem(Behavior behaviorFactory,
         string verb,
         string beginMessageTemplate,
         string endMessageTemplate,
         int duration = 10, int range = 8)
      {
         this.behaviorPrototype = behaviorFactory;
         this.duration = duration;
         this.verb = verb;
         this.beginMessageTemplate = beginMessageTemplate;
         this.endMessageTemplate = endMessageTemplate;
         this.range = range;
      }

      public static BehaviorReplacementItem CreateConfusionItem()
      {
         Library.Current.Behaviors.Get("Confused");
         return new BehaviorReplacementItem(Library.Current.Behaviors.Get("Confused"),
            "confuse",
            "The eyes of the {target} look vacant, as he starts to stumble around.",
            "The {target} is no longer confused.");
      }

      public bool Use(Entity user, Some<RequirementFulfillment> fulfillment)
      {
         var closestMonster = World.Current.Scene.ClosestEnemy(user, range);

         return closestMonster.Match(
            Some: target =>
            {
               var effect = new BehaviorReplacementEffect(user, target, duration, this.behaviorPrototype)
               {
                  BeginMessageTemplate = beginMessageTemplate,
                  EndMessageTemplate = endMessageTemplate,
               };
               
               return true;
            },
            None: () => false);
      }

      public ActionRequirement Requirements => NoRequirement.None;
   }
}