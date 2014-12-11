using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.Actions.Requirements;
using LanguageExt;
using LanguageExt.Prelude;

namespace DraconicEngine.Items
{
   public enum ItemUseResult
   {
      Used,
      Destroyed,
      NotUsed,
   }
   [Serializable]
   public class Item
   {
      public ItemTemplate Template { get; private set; }
      public int Charges { get; set; }

      public Item(string name, ItemTemplate template)
      {
         this.Template = template;
      }

      public ItemUseResult Use(Entity user, Some<RequirementFulfillment> itemsRequirements)
      {
         var usage = this.Template.Usage;
         if (usage != null)
         {
            if(usage.Use(user, itemsRequirements))
            {
               if (this.Template.IsCharged)
               {
                  this.Charges--;

                  if(this.Charges <= 0)
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
}
