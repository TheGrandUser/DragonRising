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
using System.Diagnostics.Contracts;
using DraconicEngine;
using DraconicEngine.GameWorld;
using DraconicEngine.GameWorld.Actions;
using DragonRising.GameWorld.Components;

namespace DragonRising.GameWorld.Actions
{
   public class DropItemAction : RogueAction
   {
      public Entity Dropper { get; }
      public Entity Item { get; }
      public DropItemAction(Entity dropper, Entity item)
      {
         Contract.Requires(dropper != null);
         Contract.Requires(item != null);
         this.Dropper = dropper;
         this.Item = item;
      }
   }

   public class BaseDropItemRule
   {
      public void Apply(DropItemAction action)
      {
         var scene = World.Current.Scene;

         action.Dropper.GetComponent<InventoryComponent>().Items.Remove(action.Item);
         action.Item.GetComponent<LocationComponent>().Location = action.Dropper.GetComponent<LocationComponent>().Location;
         scene.EntityStore.AddEntity(action.Item);
      }
   }
}
