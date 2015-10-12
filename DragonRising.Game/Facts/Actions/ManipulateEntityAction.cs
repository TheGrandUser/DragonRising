using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using DraconicEngine.EntitySystem;
using System.Diagnostics.Contracts;
using DraconicEngine;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Components;
using DragonRising.Rules;

namespace DragonRising.GameWorld.Actions
{
   public class ManipulateEntityAction : ActionTaken
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
}
