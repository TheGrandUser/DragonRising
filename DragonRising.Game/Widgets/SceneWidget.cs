using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Terminals;
using DraconicEngine.EntitySystem;
using DragonRising;
using DraconicEngine;
using DraconicEngine.Widgets;
using DragonRising.GameWorld;

namespace DragonRising.Widgets
{
   public class SceneWidget : Widget
   {
      //Character darkWall = new Character(Glyph.Space, RogueColors.White, new RogueColor(0, 0, 100));
      //Character lightWall = new Character(Glyph.Space, RogueColors.White, new RogueColor(130, 110, 50));
      //Character darkGround = new Character(Glyph.Space, RogueColors.White, new RogueColor(50, 50, 150));
      //Character lightGround = new Character(Glyph.Space, RogueColors.White, new RogueColor(200, 180, 50));

      World world;
      SceneView sceneView;

      public SceneWidget(World world, SceneView sceneView, ITerminal sceneTerminal)
         : base(sceneTerminal)
      {
         this.world = world;
         this.sceneView = sceneView;
      }

      public override void Draw()
      {
         this.sceneView.Update();
         var scene = world.Scene;
         scene.UpdateFoV();

         var rowStart = Math.Max(this.sceneView.ViewOffset.Y, 0);
         var rowCount = Math.Min(this.Panel.Size.Y, scene.MapHeight - this.sceneView.ViewOffset.Y);
         var colStart = Math.Max(this.sceneView.ViewOffset.X, 0);
         var colCount = Math.Min(this.Panel.Size.X, scene.MapWidth - this.sceneView.ViewOffset.X);

         foreach (var row in Enumerable.Range(rowStart, rowCount))
         {
            var stride = row * scene.MapWidth;
            foreach (var col in Enumerable.Range(colStart, colCount))
            {
               var tile = scene.Map[col + stride];
               var tileType = tile.GetTileType();
               if (tile.Visibility == TileVisibility.Explored)
               {
                  var display = tileType.Explored;
                  this.Panel.Set(col - this.sceneView.ViewOffset.X, row - this.sceneView.ViewOffset.Y, display);
               }
               else if (tile.Visibility == TileVisibility.Seen)
               {
                  var display = tileType.InView;
                  this.Panel.Set(col - this.sceneView.ViewOffset.X, row - this.sceneView.ViewOffset.Y, display);
               }
            }
         }
      }
   }
}
