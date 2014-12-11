using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld
{
   public class GameState
   {
      public List<MapInfo> Maps { get; } = new List<MapInfo>();
   }

   public class MapInfo
   {
      public Guid Id { get; set; }

      public int Width { get; set; }
      public int Height { get; set; }

      public Loc WorldLocation { get; set; }
   }
}