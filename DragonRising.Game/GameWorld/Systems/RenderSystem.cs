using DraconicEngine;
using DraconicEngine.GameWorld;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Terminals;
using DraconicEngine.Widgets;
using DragonRising.GameWorld.Nodes;
using DragonRising.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Systems
{
   public class RenderSystem : ListIteratingSystemSync<DrawNode>
   {
      ITerminal terminal;
      SceneView sceneView;
      World world;

      public RenderSystem(ITerminal terminal, SceneView sceneView, World world)
      {
         this.terminal = terminal;
         this.sceneView = sceneView;
         this.world = world;
      }

      protected override void NodeUpdateFunction(DrawNode node, double time)
      {
         var display = node.Location.Location - sceneView.ViewOffset;

         if (display.X >= 0 && display.X < terminal.Size.X &&
             display.Y >= 0 && display.Y < terminal.Size.Y)
         {
            var tile = world.Scene.GetTileSafe(node.Location.Location);
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
      World world;

      public ItemRenderSystem(ITerminal terminal, SceneView sceneView, World world)
      {
         this.terminal = terminal;
         this.sceneView = sceneView;
         this.world = world;
      }

      protected override void NodeUpdateFunction(ItemDrawNode node, double time)
      {
         var display = node.Location.Location - sceneView.ViewOffset;

         if (display.X >= 0 && display.X < terminal.Size.X &&
             display.Y >= 0 && display.Y < terminal.Size.Y)
         {
            var tile = world.Scene.GetTileSafe(node.Location.Location);
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
      World world;

      public CreatureRenderSystem(ITerminal terminal, SceneView sceneView, World world)
      {
         this.terminal = terminal;
         this.sceneView = sceneView;
         this.world = world;
      }

      protected override void NodeUpdateFunction(CreatureDrawNode node, double time)
      {
         var display = node.Location.Location - sceneView.ViewOffset;

         if (display.X >= 0 && display.X < terminal.Size.X &&
             display.Y >= 0 && display.Y < terminal.Size.Y)
         {
            var tile = world.Scene.GetTileSafe(node.Location.Location);
            if (tile.Visibility == TileVisibility.Seen)
            {
               terminal.Set(display, node.Drawn.SeenCharacter);
            }
         }
      }
   }
}
