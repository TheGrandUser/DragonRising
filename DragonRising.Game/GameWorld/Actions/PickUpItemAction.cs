using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine;
using DraconicEngine.GameWorld;
using DraconicEngine.GameWorld.Actions;
using DragonRising.GameWorld.Components;

namespace DragonRising.GameWorld.Actions
{
   public class PickUpItemAction : RogueAction
   {
      public Entity Picker { get; }
      public Entity ItemToPick { get; }
      
      public PickUpItemAction (Entity picker, Entity itemToPick)
      {
         Picker = picker;
         ItemToPick = itemToPick;
      }
   }

   public class BasePickupItemRule : IActionRule<PickUpItemAction>
   {
      public void Apply(PickUpItemAction action)
      {
         var inventory = action.Picker.GetComponent<InventoryComponent>();
         var scene = World.Current.Scene;

         var itemComponent = action.ItemToPick.GetComponent<ItemComponent>();

         if (inventory.TryPickUp(action.ItemToPick))
         {
            scene.EntityStore.RemoveEntity(action.ItemToPick);
         }
      }
   }
}
