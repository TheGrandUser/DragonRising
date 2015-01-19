using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.GameWorld.Items;
using LanguageExt;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld;
using DraconicEngine;

namespace DragonRising.GameWorld.Components
{
   public class ManipulatableComponent : Component
   {
      public ManipulatableComponent()
      {

      }

      protected ManipulatableComponent(ManipulatableComponent original, bool fresh)
         : base(original, fresh)
      {
         this.RequiresItem = original.RequiresItem;
      }
      public bool RequiresItem { get; set; }

      public bool Use(Option<Entity> itemToUse)
      {
         return false;
      }

      protected override Component CloneCore(bool fresh)
      {
         return new ManipulatableComponent(this, fresh);
      }
   }
}