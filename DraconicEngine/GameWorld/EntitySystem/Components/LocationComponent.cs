using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   public class LocationComponent : Component
   {
      public LocationComponent()
      {
      }

      protected LocationComponent(LocationComponent original, bool fresh)
         : base(original, fresh)
      {
         Blocks = original.Blocks;
         Location = fresh ? Loc.Zero : original.Location;
      }

      public bool Blocks { get; set; }
      public Loc Location { get; set; }

      protected override Component CloneCore(bool fresh)
      {
         return new LocationComponent(this, fresh);
      }
   }
}
