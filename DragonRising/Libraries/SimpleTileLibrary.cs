using DraconicEngine.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;

namespace DragonRising.Libraries
{
   class SimpleTileLibrary : ITileLibrary
   {
      Dictionary<int, TileType> tileTypes = new Dictionary<int, TileType>();

      public int BasicClearId => 1;

      public int BasicWallId => 2;

      public TileType GetById(int id)
      {
         return tileTypes[id];
      }

      public SimpleTileLibrary()
      {
         Character darkWall = new Character(Glyph.Space, RogueColors.White, new RogueColor(0, 0, 100));
         Character lightWall = new Character(Glyph.Space, RogueColors.White, new RogueColor(130, 110, 50));
         Character darkGround = new Character(Glyph.Space, RogueColors.White, new RogueColor(50, 50, 150));
         Character lightGround = new Character(Glyph.Space, RogueColors.White, new RogueColor(200, 180, 50));

         AddTileType(new TileType()
         {
            Id = Tile.VoidId,
            Name = "Void",
            InView = new Character(Glyph.Space, RogueColors.Black),
            Explored = new Character(Glyph.Space, RogueColors.Black),
            BlocksMovement = true,
            BlocksSight = true
         });
         AddTileType(new TileType()
         {
            Id = BasicClearId,
            Name = "Ground",
            Explored = new Character(Glyph.Space, RogueColors.White, new RogueColor(50, 50, 150)),
            InView = new Character(Glyph.Space, RogueColors.White, new RogueColor(200, 180, 50)),
         });
         AddTileType(new TileType()
         {
            Id = BasicWallId,
            Name = "Stone wall",
            InView = new Character(Glyph.Space, RogueColors.Black, new RogueColor(130, 110, 50)),
            Explored = new Character(Glyph.Space, RogueColors.White, new RogueColor(0, 0, 100)),
            BlocksMovement = true,
            BlocksSight = true
         });
      }

      public void AddTileType(TileType tileType)
      {
         this.tileTypes.Add(tileType.Id, tileType);
      }
   }
}
