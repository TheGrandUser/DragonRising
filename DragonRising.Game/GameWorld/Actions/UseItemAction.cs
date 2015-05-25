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
      public Some<Entity> User { get; }
      public Some<Entity> Item { get; }
      public Some<RequirementFulfillment> ItemReqFulfillment { get; }

      public UseItemAction(Some<Entity> user, Some<Entity> item, Some<RequirementFulfillment> itemReqFulfillment)
      {
         User = user;
         this.ItemReqFulfillment = itemReqFulfillment;
         this.Item = item;
      }
   }

   public class BaseUseItemRule : IActionRule<UseItemAction>
   {
      public void Apply(UseItemAction action)
      {
         var itemComponent = action.Item.Value.GetComponent<ItemComponent>();
         var usable = itemComponent.Usable;

         var result = usable.Use(action.User, action.ItemReqFulfillment);
         if (result == ItemUseResult.Destroyed)
         {
            action.User.Value.As<InventoryComponent>(inventory => inventory.Items.Remove(action.Item));
         }
      }
   }
}
