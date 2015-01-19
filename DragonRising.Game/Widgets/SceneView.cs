using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Terminals;
using DraconicEngine;
using DraconicEngine.Widgets;

namespace DragonRising.Widgets
{
   public abstract class SceneView
   {
      private Scene scene;
      protected Scene Scene { get { return scene; } }

      private ITerminal panel;
      protected ITerminal Panel { get { return panel; } }

      Vector viewOffset = new Vector();

      public SceneView(Scene scene, ITerminal panel)
      {
         this.scene = scene;
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

      public FocusEntitySceneView(Scene scene, ITerminal panel, Entity entity)
         : base(scene, panel)
      {
         this.entity = entity;
         this.lastLocation = this.entity.GetLocation();
      }

      public override void Update()
      {
         if (this.lastLocation != entity.GetLocation())
         {
            this.Scene.ClearFoV();
            this.Scene.UpdateFoV();
         }

         var offsetSize = this.Panel.Size / 2;
         this.ViewOffset = new Vector(
            Math.Min(Math.Max(this.Scene.FocusEntity.GetLocation().X - offsetSize.X, 0), this.Scene.MapWidth - this.Panel.Size.X),
            Math.Min(Math.Max(this.Scene.FocusEntity.GetLocation().Y - offsetSize.Y, 0), this.Scene.MapHeight - this.Panel.Size.Y));
      }
   }
}
