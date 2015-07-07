using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tinkering
{
   class WorldContext
   {
      private readonly Random r = new Random();

      public IEnumerable<Entity> GetEntityInArea(Area area)
      {
         return Enumerable.Empty<Entity>();
      }

      public Random Random => r;
   }

}
