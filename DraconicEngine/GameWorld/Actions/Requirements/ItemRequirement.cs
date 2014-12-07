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
   }

   public class ItemFulfillment : RequirementFulfillment
   {
      public Item Item { get; }
      public Some<RequirementFulfillment> ItemsFulfillments { get; }
      public ItemFulfillment(Item item, Some<RequirementFulfillment> itemsRequirements)
      {
         this.Item = item;
         this.ItemsFulfillments = itemsRequirements;
      }
   }
}