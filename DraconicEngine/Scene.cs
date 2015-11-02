using DraconicEngine.EntitySystem;
using LanguageExt;
using System;
using System.Linq;
using DraconicEngine;
using DraconicEngine.RulesSystem;

namespace DraconicEngine
{
   [Serializable]
   public sealed class Scene
   {
      Entity focusEntity = null;
      Tile beyondTheEdge = new Tile(Tile.VoidId);

      public Scene(Tile[] map, int width, int height, IEntityStore entityStore)
      {
         this.Map = map;
         this.MapWidth = width;
         this.MapHeight = height;
         this.EntityStore = entityStore;
      }

      public Scene(int mapWidth, int mapHeight, int fillTile, IEntityStore entityStore)
      {
         this.MapWidth = mapWidth;
         this.MapHeight = mapHeight;

         this.Map = new Tile[mapWidth * mapHeight];
         for (int index = 0; index < Map.Length; index++)
         {
            Map[index] = new Tile(fillTile);
         }
         this.EntityStore = entityStore;
      }

      public Entity Stairs { get; set; }

      public int MapWidth { get; set; }
      public int MapHeight { get; set; }

      public IEntityStore EntityStore { get; }

      public Tile[] Map { get; set; }

      public Entity FocusEntity
      {
         get { return focusEntity; }
         set
         {
            if (value != null && !this.EntityStore.Entities.Contains(value)) { throw new InvalidOperationException($"Entity {value.Name} is not in the scene's entity store"); }
            if (this.focusEntity != value)
            {
               this.focusEntity = value;
            }
         }
      }

      public int Level { get; set; }

      public Tile GetTileSafe(int col, int row) =>
         (col < 0 || row < 0 || col >= MapWidth || row >= MapHeight) ? beyondTheEdge : Map[col + row * MapWidth];

   }
}
