using DraconicEngine.Storage;
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
      public TileType GetTileType() { return TileLibrary.Current.GetById(TileTypeId); }
      public int TileTypeId { get; set; }
      public TileVisibility Visibility { get; set; }

      public bool BlocksSight => GetTileType().BlocksSight;
      public bool BlocksMovement => GetTileType().BlocksMovement;

      public Tile(int tileId)
      {
         this.TileTypeId = tileId;
         this.Visibility = TileVisibility.NotSeen;
      }

   }

   [Serializable]
   public class TileType
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public string Description { get; set; } = string.Empty;
      public Character InView { get; set; }
      public Character Explored { get; set; }
      public bool BlocksMovement { get; set; }
      public bool BlocksSight { get; set; }
   }
}