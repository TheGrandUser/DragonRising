using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Prelude;
using DraconicEngine.GameWorld.Actions.Requirements;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   public class ItemComponent : Component
   {
      public ItemComponentTemplate Template { get; set; }
      public int Charges { get; set; }

      public ItemUseResult Use(Entity user, Some<RequirementFulfillment> itemsRequirements)
      {
         var usage = this.Template.Usage;
         if (usage != null)
         {
            if (usage.Use(user, itemsRequirements))
            {
               if (this.Template.IsCharged)
               {
                  this.Charges--;

                  if (this.Charges <= 0)
                  {
                     return ItemUseResult.Destroyed;
                  }
               }

               return ItemUseResult.Used;
            }
         }

         return ItemUseResult.NotUsed;
      }
   }

   public class ItemComponentTemplate : ComponentTemplate
   {
      public IItemUsage Usage { get; set; }
      public int MaxCharges { get; set; }
      public bool IsCharged { get; set; }

      public override Type ComponentType => typeof(ItemComponent);

      public override Component CreateComponent()
      {
         return new ItemComponent() { Template = this, Charges = this.MaxCharges };
      }
   }
   public enum ItemUseResult
   {
      Used,
      Destroyed,
      NotUsed,
   }
}
