using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.EntitySystem.Nodes;
using DraconicEngine.Terminals;
using DraconicEngine.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Systems
{
   public class RenderSystem : ListIteratingSystemSync<DrawNode>
   {
      ITerminal terminal;
      SceneView sceneView;
      Scene scene;

      public RenderSystem(ITerminal terminal, SceneView sceneView, Scene scene)
      {
         this.terminal = terminal;
         this.sceneView = sceneView;
         this.scene = scene;
      }

      protected override void NodeUpdateFunction(DrawNode node, double time)
      {
         var display = node.Location.Location - sceneView.ViewOffset;

         if (display.X >= 0 && display.X < terminal.Size.X &&
             display.Y >= 0 && display.Y < terminal.Size.Y)
         {
            var tile = scene.GetTileSafe(node.Location.Location);
            if (tile.Visibility == TileVisibility.Seen)
            {
               terminal.Set(display, node.Drawn.SeenCharacter);
            }
            else if (tile.Visibility == TileVisibility.Explored && node.Drawn.ExploredCharacter.HasValue)
            {
               terminal.Set(display, node.Drawn.ExploredCharacter.Value);
            }
         }
      }
   }

   public class ItemRenderSystem : ListIteratingSystemSync<ItemDrawNode>
   {
      ITerminal terminal;
      SceneView sceneView;
      Scene scene;

      public ItemRenderSystem(ITerminal terminal, SceneView sceneView, Scene scene)
      {
         this.terminal = terminal;
         this.sceneView = sceneView;
         this.scene = scene;
      }

      protected override void NodeUpdateFunction(ItemDrawNode node, double time)
      {
         var display = node.Location.Location - sceneView.ViewOffset;

         if (display.X >= 0 && display.X < terminal.Size.X &&
             display.Y >= 0 && display.Y < terminal.Size.Y)
         {
            var tile = scene.GetTileSafe(node.Location.Location);
            if (tile.Visibility == TileVisibility.Seen)
            {
               terminal.Set(display, node.Drawn.SeenCharacter);
            }
            else if (tile.Visibility == TileVisibility.Explored && node.Drawn.ExploredCharacter.HasValue)
            {
               terminal.Set(display, node.Drawn.ExploredCharacter.Value);
            }
         }
      }
   }

   public class CreatureRenderSystem : ListIteratingSystemSync<CreatureDrawNode>
   {
      ITerminal terminal;
      SceneView sceneView;
      Scene scene;

      public CreatureRenderSystem(ITerminal terminal, SceneView sceneView, Scene scene)
      {
         this.terminal = terminal;
         this.sceneView = sceneView;
         this.scene = scene;
      }

      protected override void NodeUpdateFunction(CreatureDrawNode node, double time)
      {
         var display = node.Location.Location - sceneView.ViewOffset;

         if (display.X >= 0 && display.X < terminal.Size.X &&
             display.Y >= 0 && display.Y < terminal.Size.Y)
         {
            var tile = scene.GetTileSafe(node.Location.Location);
            if (tile.Visibility == TileVisibility.Seen)
            {
               terminal.Set(display, node.Drawn.SeenCharacter);
            }
         }
      }
   }
}
