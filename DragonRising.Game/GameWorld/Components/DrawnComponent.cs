using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld;
using DraconicEngine;

namespace DragonRising.GameWorld.Components
{
   public class DrawnComponent : Component
   {
      public DrawnComponent()
      {
      }

      protected DrawnComponent(DrawnComponent original, bool fresh)
         : base(original, fresh)
      {
         ExploredCharacter = original.ExploredCharacter;
         SeenCharacter = original.SeenCharacter;
      }

      public Character SeenCharacter { get; set; }
      public Character? ExploredCharacter { get; set; }

      protected override Component CloneCore(bool fresh)
      {
         return new DrawnComponent(this, fresh);
      }
   }
}
