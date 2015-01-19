using Newtonsoft.Json;
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

      [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
      public bool Blocks { get; set; }
      [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
      public Loc Location { get; set; }

      protected override Component CloneCore(bool fresh)
      {
         return new LocationComponent(this, fresh);
      }
   }
}
