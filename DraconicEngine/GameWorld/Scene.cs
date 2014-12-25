using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Storage;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
   [Serializable]
   public sealed class Scene
   {
      public static int VoidId;
      Entity focusEntity = null;
      Tile beyondTheEdge;
      bool recomputeFov = true;

      public Scene(int mapWidth, int mapHeight)
      {
         this.beyondTheEdge = new Tile(TileLibrary.Current.VoidId);
         var wallId = TileLibrary.Current.BasicWallId;

         this.Map = new Tile[mapWidth, mapHeight];
         for (int row = 0; row < mapHeight; row++)
         {
            for (int col = 0; col < mapWidth; col++)
            {
               Map[col, row] = new Tile(wallId);
            }
         }
      }

      public IEntityStore EntityStore { get; } = new EntityStore();

      public Tile[,] Map { get; }

      public int MapWidth => Map.GetUpperBound(0) + 1;
      public int MapHeight => Map.GetUpperBound(1) + 1;

      private void ResetFoV()
      {
         ShadowCaster.ComputeFieldOfViewWithShadowCasting(this.FocusEntity.Location.X, this.FocusEntity.Location.Y, this.FocusEntity.GetComponentOrDefault<CreatureComponent>()?.VisionRadius ?? 1,
            (x, y) => GetTileSafe(x, y).BlocksSight,
            (x, y) =>
            {
               var tile = GetTileSafe(x, y);
               if (tile.TileTypeId != VoidId)
               {
                  tile.Visibility = TileVisibility.Seen;
               }
            });

         recomputeFov = false;
      }

      public Tile GetTileSafe(int x, int y)
      {
         if (x < 0 || y < 0 ||
            x >= MapWidth || y >= MapHeight)
         {
            return beyondTheEdge;
         }
         return Map[x, y];
      }

      public Tile GetTileSafe(Loc location) => GetTileSafe(location.X, location.Y);

      public Entity FocusEntity
      {
         get { return focusEntity; }
         set
         {
            if (this.focusEntity != value)
            {
               if (this.focusEntity != null)
               {
                  this.EntityStore.SetSpecial(this.focusEntity, false);
               }
               this.focusEntity = value;

               if (this.focusEntity != null)
               {
                  this.EntityStore.SetSpecial(this.focusEntity, true);
                  this.ResetFoV();
               }
            }
         }
      }

      public void ClearFoV()
      {
         if (recomputeFov)
         {
            return;
         }

         var radius = (this.FocusEntity.GetComponentOrDefault<CreatureComponent>()?.VisionRadius ?? 1) + 1;
         var xStart = Math.Max(this.FocusEntity.Location.X - radius - 1, 0);
         var yStart = Math.Max(this.FocusEntity.Location.Y - radius - 1, 0);
         var xEnd = Math.Min(this.FocusEntity.Location.X + radius + 1, this.MapWidth);
         var yEnd = Math.Min(this.FocusEntity.Location.Y + radius + 1, this.MapHeight);

         for (int x = xStart; x < xEnd; x++)
         {
            for (int y = yStart; y < yEnd; y++)
            {
               var tile = Map[x, y];
               if (tile.Visibility == TileVisibility.Seen)
               {
                  tile.Visibility = TileVisibility.Explored;
               }
            }
         }

         recomputeFov = true;
      }

      public Blockage IsBlocked(Loc location, Predicate<Entity> ignoreWhere = null)
      {
         int x = location.X;
         int y = location.Y;

         if (x < 0 || x >= MapWidth || y < 0 || y >= MapHeight)
         {
            return Blockage.OffMap;
         }
         if (Map[x, y].BlocksMovement)
         {
            return Blockage.Tile;
         }

         if (ignoreWhere != null)
         {
            return this.EntityStore.GetEntitiesAt(location).Any(e => e.Blocks && !ignoreWhere(e)) ? Blockage.Entity : Blockage.None;
         }
         else
         {
            return this.EntityStore.GetEntitiesAt(location).Any(e => e.Blocks) ? Blockage.Entity : Blockage.None;
         }
      }

      public void UpdateFoV()
      {
         if (recomputeFov && this.FocusEntity != null)
         {
            ResetFoV();
         }
      }

      public bool IsVisible(Loc vec) => GetTileSafe(vec).Visibility == TileVisibility.Seen;

      public void ClearMap()
      {
         var width = this.MapWidth;
         var height = this.MapHeight;
         var wallId = TileLibrary.Current.BasicWallId;

         for (int column = 0; column < width; column++)
         {
            for (int row = 0; row < height; row++)
            {
               Map[column, row] = new Tile(wallId);
            }
         }
      }

      static Stack<Scene> scenes = new Stack<Scene>();
      public static Scene CurrentScene => scenes.Count > 0 ? scenes.Peek() : null;

      public static void PushScene(Scene scene) => scenes.Push(scene);

      public static void PopScene() => scenes.Pop();
   }

   public enum Blockage
   {
      None,
      Tile,
      Entity,
      OffMap,
   }

   public static class SceneExtensions
   {
      public static Option<Entity> ClosestEnemy(this Scene scene, Entity originator, int? maxRange)
      {
         int max = maxRange + 1 ?? int.MaxValue;

         var closestEnemy = from c in scene.EntityStore.AllCreaturesSpecialFirst
                            where c != originator
                            let distance = (c.Location - originator.Location).KingLength
                            where distance < max && scene.IsVisible(c.Location) && c.HasComponent<CombatantComponent>() && originator.IsEnemy(c)
                            orderby distance
                            select c;

         return closestEnemy.FirstOrDefault();
      }
   }
}
