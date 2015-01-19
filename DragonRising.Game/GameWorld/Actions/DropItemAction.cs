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
      Entity item;
      public DropItemAction(Entity item)
      {
         Contract.Requires(item != null);
         this.item = item;
      }

      public override void Do(Entity executer)
      {
         var scene = World.Current.Scene;

         executer.GetComponent<InventoryComponent>().Items.Remove(item);
         item.GetComponent<LocationComponent>().Location = executer.GetComponent<LocationComponent>().Location;
         scene.EntityStore.Add(this.item);
      }
   }
}
