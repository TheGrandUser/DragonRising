using DraconicEngine.EntitySystem;
using LanguageExt;
using System;
using System.Linq;
using DraconicEngine;
using DragonRising.GameWorld.Components;
using DragonRising.Storage;
using DraconicEngine.RulesSystem;

namespace DragonRising.GameWorld
{
   public class FieldOfView
   {
      bool recomputeFov = true;
      World world;

      public FieldOfView(World world)
      {
         this.world = world;
      }

      public void ClearFoV()
      {
         if (recomputeFov)
         {
            return;
         }

         var loc = world.Scene.FocusEntity.GetLocation();

         var radius = (world.Scene.FocusEntity.GetComponentOrDefault<CreatureComponent>()?.VisionRadius ?? 1) + 1;
         var xStart = Math.Max(loc.X - radius - 1, 0);
         var yStart = Math.Max(loc.Y - radius - 1, 0);
         var xEnd = Math.Min(loc.X + radius + 1, world.Scene.MapWidth);
         var yEnd = Math.Min(loc.Y + radius + 1, world.Scene.MapHeight);


         for (int row = yStart; row < yEnd; row++)
         {
            var stride = row * world.Scene.MapWidth;
            for (int col = xStart; col < xEnd; col++)
            {
               var tile = world.Scene.Map[col + stride];
               if (tile.Visibility == TileVisibility.Seen)
               {
                  tile.Visibility = TileVisibility.Explored;
               }
            }
         }

         recomputeFov = true;
      }

      public void UpdateFoV()
      {
         if (recomputeFov && world.Scene.FocusEntity != null)
         {
            ResetFoV();
         }
      }

      private void ResetFoV()
      {
         var tileLib = TileLibrary.Current;

         var loc = world.Scene.FocusEntity.GetLocation();
         ShadowCaster.ComputeFieldOfViewWithShadowCasting(loc.X, loc.Y, world.Scene.FocusEntity.GetComponentOrDefault<CreatureComponent>()?.VisionRadius ?? 1,
            (x, y) => tileLib.GetById(world.Scene.GetTileSafe(x, y).TileTypeId).BlocksSight,
            (x, y) =>
            {
               var tile = world.Scene.GetTileSafe(x, y);
               if (tile.TileTypeId != Tile.VoidId)
               {
                  tile.Visibility = TileVisibility.Seen;
               }
            });

         recomputeFov = false;
      }

   }
}
