using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Storage
{
   public static class TileLibrary
   {
      static ITileLibrary current;
      public static ITileLibrary Current { get { return current; } }
      public static void Set(ITileLibrary library)
      {
         current = library;
      }
   }

   public interface ITileLibrary
   {
      TileType GetById(int id);

      int VoidId { get; }
      int BasicClearId { get; }
      int BasicWallId { get; }
   }
}
