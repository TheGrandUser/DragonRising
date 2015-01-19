using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Terminals;
using DraconicEngine;
using DraconicEngine.Widgets;
using DragonRising.GameWorld;

namespace DragonRising.Widgets
{
   public abstract class SceneView
   {
      private World world;
      protected World World { get { return world; } }

      private ITerminal panel;
      protected ITerminal Panel { get { return panel; } }

      Vector viewOffset = new Vector();

      public SceneView(World world, ITerminal panel)
      {
         this.world = world;
         this.panel = panel;
      }

      public Vector ViewOffset
      {
         get { return viewOffset; }
         set { viewOffset = value; }
      }

      public abstract void Update();
   }

   public class FocusEntitySceneView : SceneView
   {
      Entity entity;
      private Loc lastLocation;

      public FocusEntitySceneView(World world, ITerminal panel, Entity entity)
         : base(world, panel)
      {
         this.entity = entity;
         this.lastLocation = this.entity.GetLocation();
      }

      public override void Update()
      {
         if (this.lastLocation != entity.GetLocation())
         {
            this.World.Scene.ClearFoV();
            this.World.Scene.UpdateFoV();
         }

         var offsetSize = this.Panel.Size / 2;
         this.ViewOffset = new Vector(
            Math.Min(Math.Max(this.World.Scene.FocusEntity.GetLocation().X - offsetSize.X, 0), this.World.Scene.MapWidth - this.Panel.Size.X),
            Math.Min(Math.Max(this.World.Scene.FocusEntity.GetLocation().Y - offsetSize.Y, 0), this.World.Scene.MapHeight - this.Panel.Size.Y));
      }
   }
}
