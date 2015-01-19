using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using DragonRising.GameWorld.Items;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine;
using DraconicEngine.GameWorld;
using DraconicEngine.GameWorld.Actions;
using DragonRising.GameWorld.Components;

namespace DragonRising.GameWorld.Actions
{
   public class UseItemAction : RogueAction
   {
      Some<Entity> item;
      Some<RequirementFulfillment> itemReqFulfillment;

      public UseItemAction(Some<Entity> item, Some<RequirementFulfillment> itemReqFulfillment)
      {
         this.itemReqFulfillment = itemReqFulfillment;
         this.item = item;
      }

      public override void Do(Entity executer)
      {
         var itemComponent = item.Value.GetComponent<ItemComponent>();
         var usable = itemComponent.Usable;

         var result = usable.Use(executer, itemReqFulfillment);
         if (result == ItemUseResult.Destroyed)
         {
            executer.As<InventoryComponent>(inventory => inventory.Items.Remove(item));
         }
      }
   }
}
