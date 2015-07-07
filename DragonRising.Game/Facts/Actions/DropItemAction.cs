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

namespace DragonRising.GameWorld.Actions
{
   public class DropItemAction : ActionTaken
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
}
