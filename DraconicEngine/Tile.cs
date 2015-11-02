using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
   [Serializable]
   public enum TileVisibility
   {
      NotSeen,
      Explored,
      Seen,
   }

   [Serializable]
   public class Tile
   {
      public static int VoidId = 0;

      public int TileTypeId { get; set; }
      public TileVisibility Visibility { get; set; }

      public Tile(int tileId)
      {
         TileTypeId = tileId;
         Visibility = TileVisibility.NotSeen;
      }
   }
}
