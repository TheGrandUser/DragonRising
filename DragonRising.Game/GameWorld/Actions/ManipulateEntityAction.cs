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
   public class ManipulateEntityAction : RogueAction
   {
      public Some<Entity> User { get; }
      public Some<Entity> Target { get; }
      public Option<Entity> ItemToUse { get; }

      public ManipulateEntityAction(Some<Entity> user, Some<Entity> target, Option<Entity> item)
      {
         User = user;
         this.Target = target;
         this.ItemToUse = item;
      }
   }

   public class BaseManipulateEntityRule : IActionRule<ManipulateEntityAction>
   {
      public void Apply(ManipulateEntityAction action)
      {
         var m = action.Target.Value.GetComponent<ManipulatableComponent>();
         if (m.Use(action.ItemToUse))
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
