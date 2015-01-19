using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DragonRising.GameWorld.Components;
using DragonRising.Storage;

namespace DragonRising
{
   [Serializable]
   public sealed class Scene
   {
      Entity focusEntity = null;
      Tile beyondTheEdge = new Tile(Tile.VoidId);
      bool recomputeFov = true;

      public int MapWidth { get; set; }
      public int MapHeight { get; set; }

      public Scene(Tile[] map, int width, int height)
      {
         this.Map = map;
         this.MapWidth = width;
         this.MapHeight = height;
      }

      public Scene(int mapWidth, int mapHeight)
      {
         this.MapWidth = mapWidth;
         this.MapHeight = mapHeight;
         var wallId = TileLibrary.Current.BasicWallId;

         this.Map = new Tile[mapWidth * mapHeight];
         for (int index = 0; index < Map.Length; index++)
         {
            Map[index] = new Tile(wallId);
         }
      }

      public IEntityStore EntityStore { get; } = new EntityStore();

      public Tile[] Map { get; set; }

      private void ResetFoV()
      {
         var loc = this.FocusEntity.GetLocation();
         ShadowCaster.ComputeFieldOfViewWithShadowCasting(loc.X, loc.Y, this.FocusEntity.GetComponentOrDefault<CreatureComponent>()?.VisionRadius ?? 1,
            (x, y) => GetTileSafe(x, y).BlocksSight,
            (x, y) =>
            {
               var tile = GetTileSafe(x, y);
               if (tile.TileTypeId != Tile.VoidId)
               {
                  tile.Visibility = TileVisibility.Seen;
               }
            });

         recomputeFov = false;
      }

      public Tile GetTileSafe(int col, int row)
      {
         if (col < 0 || row < 0 ||
            col >= MapWidth || row >= MapHeight)
         {
            return beyondTheEdge;
         }
         return Map[col + row * MapWidth];
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

         var loc = this.FocusEntity.GetLocation();

         var radius = (this.FocusEntity.GetComponentOrDefault<CreatureComponent>()?.VisionRadius ?? 1) + 1;
         var xStart = Math.Max(loc.X - radius - 1, 0);
         var yStart = Math.Max(loc.Y - radius - 1, 0);
         var xEnd = Math.Min(loc.X + radius + 1, this.MapWidth);
         var yEnd = Math.Min(loc.Y + radius + 1, this.MapHeight);


         for (int row = yStart; row < yEnd; row++)
         {
            var stride = row * MapWidth;
            for (int col = xStart; col < xEnd; col++)
            {
               var tile = Map[col + stride];
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
         int col = location.X;
         int row = location.Y;

         if (col < 0 || col >= MapWidth || row < 0 || row >= MapHeight)
         {
            return Blockage.OffMap;
         }
         if (Map[col + row * MapWidth].BlocksMovement)
         {
            return Blockage.Tile;
         }

         if (ignoreWhere != null)
         {
            return this.EntityStore.GetEntitiesAt(location).Any(e => e.GetBlocks() && !ignoreWhere(e)) ? Blockage.Entity : Blockage.None;
         }
         else
         {
            return this.EntityStore.GetEntitiesAt(location).Any(e => e.GetBlocks()) ? Blockage.Entity : Blockage.None;
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

         var closestEnemy = from c in scene.EntityStore.AllCreaturesSpecialFirst()
                            where c != originator
                            let distance = (c.GetLocation() - originator.GetLocation()).KingLength
                            where distance < max && scene.IsVisible(c.GetLocation()) && c.HasComponent<CombatantComponent>() && originator.IsEnemy(c)
                            orderby distance
                            select c;

         return closestEnemy.FirstOrDefault();
      }
   }
}
