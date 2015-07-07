using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Components;
using DragonRising.Commands.Requirements;
using DraconicEngine.RulesSystem;

namespace DragonRising.Commands.Requirements
{
   public class ItemRequirement : PlanRequirement
   {
      public ItemRequirement(string message, bool needsItemsFulfillment)
      {
         this.Message = message;
         this.NeedsItemsFulfillment = needsItemsFulfillment;
      }

      public bool NeedsItemsFulfillment { get; }

      public override bool MeetsRequirement(RequirementFulfillment fulfillment)
      {
         var itemFulfillment = fulfillment as ItemFulfillment;
         if (itemFulfillment != null)
         {
            return itemFulfillment.Item.HasComponent<ItemComponent>() &&
               (!NeedsItemsFulfillment || itemFulfillment.FinalizedPlan.IsSome);
         }
         return false;
      }
   }

   public class ItemFulfillment : RequirementFulfillment
   {
      public Entity Item { get; }
      public Option<FinalizedPlan> FinalizedPlan { get; }
      private ItemFulfillment(Some<Entity> item, Option<FinalizedPlan> finalizedPlan)
      {
         this.Item = item;
         this.FinalizedPlan = finalizedPlan;
      }

      public static RequirementFulfillment Create(Some<Entity> item, Option<FinalizedPlan> finalizedPlan)
      {
         return new ItemFulfillment(item, finalizedPlan);
      }
   }
}