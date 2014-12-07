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

   public class Item
   {
      public ItemTemplate Template { get; private set; }
      public int Charges { get; set; }

      public Item(string name, ItemTemplate template)
      {
         this.Template = template;
      }

      public ItemUseResult PrepUse(Entity executer, Some<RequirementFulfillment> itemsRequirements)
      {
         var usage = this.Template.Usage;
         if (usage != null)
         {
            return usage.PrepUse(executer, itemsRequirements);
         }

         return ItemUseResult.NotUsed;
      }

      public void Use(Entity user)
      {
         var usage = this.Template.Usage;
         if (usage != null)
         {
            usage.Use(user);
         }
      }
   }
}
