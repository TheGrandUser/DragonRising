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
      public IItemUsage Usage { get; set; }
      public int MaxCharges { get; set; }
      public bool IsCharged { get; set; }
      public int Charges { get; set; }

      public ItemComponent()
      {

      }

      protected ItemComponent(ItemComponent original, bool fresh)
         : base(original, fresh)
      {
         this.Usage = original.Usage;
         this.MaxCharges = original.MaxCharges;
         this.IsCharged = original.IsCharged;

         this.Charges = fresh ? MaxCharges : original.Charges;
      }

      public ItemUseResult Use(Entity user, Some<RequirementFulfillment> itemsRequirements)
      {
         var usage = this.Usage;
         if (usage != null)
         {
            if (usage.Use(user, itemsRequirements))
            {
               if (this.IsCharged)
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

      protected override Component CloneCore(bool fresh)
      {
         return new ItemComponent(this, fresh);
      }
   }

   //public class ItemComponentTemplate : ComponentTemplate
   //{
   //   public IItemUsage Usage { get; set; }
   //   public int MaxCharges { get; set; }
   //   public bool IsCharged { get; set; }

   //   public override Type ComponentType => typeof(ItemComponent);

   //   public override Component CreateComponent()
   //   {
   //      return new ItemComponent() { Template = this, Charges = this.MaxCharges };
   //   }
   //}

   public enum ItemUseResult
   {
      Used,
      Destroyed,
      NotUsed,
   }
}
