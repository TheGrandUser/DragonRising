﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using DraconicEngine.Items;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using System.Diagnostics.Contracts;

namespace DraconicEngine.GameWorld.Actions
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
         var scene = Scene.CurrentScene;

         executer.GetComponent<InventoryComponent>().Items.Remove(item);
         item.GetComponent<LocationComponent>().Location = executer.GetComponent<LocationComponent>().Location;
         scene.EntityStore.Add(this.item);
      }
   }
}
