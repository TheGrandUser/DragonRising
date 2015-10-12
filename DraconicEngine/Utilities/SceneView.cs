using System;
using DraconicEngine.Terminals;
using DraconicEngine;


namespace DraconicEngine
{
   public class SceneView
   {
      public ITerminal Panel { get; }

      public SceneView(ITerminal panel) { Panel = panel; }

      public Vector ViewOffset { get; set; } = new Vector();

      public void SetFocus(Loc location, Vector mapSize)
      {
         var offsetSize = this.Panel.Size / 2;
         this.ViewOffset = new Vector(
            Math.Min(Math.Max(location.X - offsetSize.X, 0), mapSize.X - Panel.Size.X),
            Math.Min(Math.Max(location.Y - offsetSize.Y, 0), mapSize.Y - Panel.Size.Y));
      }
   }
}
