using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Prelude;

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
               return itemFulfillment.Item.Value.Template.Usage.Requirements.MeetsRequirement(itemFulfillment.ItemsFulfillments);
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
      public Some<Item> Item { get; }
      public Some<RequirementFulfillment> ItemsFulfillments { get; }
      public ItemFulfillment(Some<Item> item, Some<RequirementFulfillment> itemsRequirements)
      {
         this.Item = item;
         this.ItemsFulfillments = itemsRequirements;
      }
   }
}