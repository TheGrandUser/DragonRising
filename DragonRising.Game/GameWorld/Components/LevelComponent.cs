using DraconicEngine.GameWorld.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Components
{
   public class LevelComponent : Component
   {
      public int Level { get; set; }
      public LevelComponent()
      {

      }

      protected LevelComponent(LevelComponent original)
      {
         this.Level = original.Level;
      }
      protected override Component CloneCore(bool fresh)
      {
         return new LevelComponent(this);
      }
   }
}
