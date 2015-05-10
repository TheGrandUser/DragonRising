﻿using System;
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
      Entity itemToPick;

      Action<Entity> onSuccess = null;
      Action<Entity> onFailure = null;

      public PickUpItemAction(Entity itemToPick, Action<Entity> onSuccess = null, Action<Entity> onFailure = null)
      {
         this.itemToPick = itemToPick;
         this.onFailure = onFailure;
         this.onSuccess = onSuccess;
      }

      public override void Do(Entity executer)
      {
         var inventory = executer.GetComponent<InventoryComponent>();
         var scene = World.Current.Scene;

         var itemComponent = itemToPick.GetComponent<ItemComponent>();

         if (inventory.TryPickUp(itemToPick))
         {
            scene.EntityStore.RemoveEntity(itemToPick);
         }
      }
   }
}
