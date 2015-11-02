using DraconicEngine.EntitySystem;
using LanguageExt;
using System;
using System.Linq;
using DraconicEngine;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Components;
using DragonRising.Storage;

namespace DragonRising
{
   public enum Blockage
   {
      None,
      Tile,
      Entity,
      OffMap,
   }

   public static class SceneExtensions
   {
      public static Tile GetTileSafe(this Scene scene, Loc location) => scene.GetTileSafe(location.X, location.Y);

      public static bool IsUnblockedBetween(this Scene scene, Loc startLocation, Loc location) { return true; }

      public static Option<Entity> ClosestEnemy(this Scene scene, Entity originator, int? maxRange)
      {
         int max = maxRange + 1 ?? int.MaxValue;

         var closestEnemy = from c in scene.EntityStore.AllCreatures()
                            where c != originator
                            let distance = (c.GetLocation() - originator.GetLocation()).KingLength
                            where distance < max && scene.IsVisible(c.GetLocation()) && c.HasComponent<CombatantComponent>() && originator.IsEnemy(c)
                            orderby distance
                            select c;

         return closestEnemy.FirstOrDefault();
      }

      public static Option<Entity> ClosestEntity(this Scene scene, Entity originator, int? maxRange, IEntityFilter filter)
      {
         int max = maxRange + 1 ?? int.MaxValue;

         var closestEnemy = from c in scene.EntityStore.AllCreatures()
                            where c != originator
                            let distance = (c.GetLocation() - originator.GetLocation()).KingLength
                            where distance < max && scene.IsVisible(c.GetLocation()) && c.HasComponent<CombatantComponent>() && filter.Matches(originator, c)
                            orderby distance
                            select c;

         return closestEnemy.FirstOrDefault();
      }


      public static Blockage IsBlocked(this Scene scene, Loc location, Predicate<Entity> ignoreWhere = null)
      {
         int col = location.X;
         int row = location.Y;

         if (col < 0 || col >= scene.MapWidth || row < 0 || row >= scene.MapHeight)
         {
            return Blockage.OffMap;
         }
         if (TileLibrary.Current.GetById(scene.Map[col + row * scene.MapWidth].TileTypeId).BlocksMovement)
         {
            return Blockage.Tile;
         }

         if (ignoreWhere != null)
         {
            return scene.EntityStore.GetEntitiesAt(location).Any(e => e.GetBlocks() && !ignoreWhere(e)) ? Blockage.Entity : Blockage.None;
         }
         else
         {
            return scene.EntityStore.GetEntitiesAt(location).Any(e => e.GetBlocks()) ? Blockage.Entity : Blockage.None;
         }
      }

      public static bool IsVisible(this Scene scene, Loc vec) => scene.GetTileSafe(vec).Visibility == TileVisibility.Seen;

   }
}
