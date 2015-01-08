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
   public class ManipulateEntityAction : RogueAction
   {
      Some<Entity> target;
      Option<Entity> itemToUse;

      public ManipulateEntityAction(Some<Entity> target, Option<Entity> item)
      {
         this.target = target;
         this.itemToUse = item;
      }

      public override void Do(Entity executer)
      {
         var m = target.Value.GetComponent<ManipulatableComponent>();
         if (m.Use(this.itemToUse))
         {
            // success!
         }
         else
         {
            // failed!
         }
      }
   }
}
