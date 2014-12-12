using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.Storage;

namespace DraconicEngine.Generators
{
   public class MapBuilder
   {
      static int clearId = 1;

      private Tile[,] map;
      public MapBuilder(Scene scene)
      {
         this.map = scene.Map;


      }

      public void CreateRoom(TerminalRect room)
      {
         foreach (var y in Enumerable.Range(room.Y + 1, room.Height - 1))
         {
            foreach (var x in Enumerable.Range(room.X + 1, room.Width - 1))
            {
               map[x, y].TileTypeId = clearId;
            }
         }
      }

      public void CreateHTunnel(int x1, int x2, int y)
      {
         foreach (var x in Enumerable.Range(Math.Min(x1, x2), Math.Abs(x2 - x1) + 1))
         {
            map[x, y].TileTypeId = clearId;
         }
      }

      public void CreateVTunnel(int y1, int y2, int x)
      {
         foreach (var y in Enumerable.Range(Math.Min(y1, y2), Math.Abs(y2 - y1) + 1))
         {
            map[x, y].TileTypeId = clearId;
         }
      }
   }
}