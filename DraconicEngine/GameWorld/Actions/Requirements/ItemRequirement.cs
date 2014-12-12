using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Prelude;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;

namespace DraconicEngine.GameWorld.Actions.Requirements
{
   public class ItemRequirement : ActionRequirement
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
         if(itemFulfillment != null)
         {
            if (this.NeedsItemsFulfillment)
            {
               return itemFulfillment.Item.Value.GetComponentOrDefault<ItemComponent>()?.Template.Usage.Requirements.MeetsRequirement(itemFulfillment.ItemsFulfillments) ?? false;
            }
            else
            {
               return true;
            }
         }
         return false;
      }
   }

   public class ItemFulfillment : RequirementFulfillment
   {
      public Some<Entity> Item { get; }
      public Some<RequirementFulfillment> ItemsFulfillments { get; }
      public ItemFulfillment(Some<Entity> item, Some<RequirementFulfillment> itemsRequirements)
      {
         this.Item = item;
         this.ItemsFulfillments = itemsRequirements;
      }
   }
}