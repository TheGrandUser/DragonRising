using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Terminals;

namespace DraconicEngine.Widgets
{
   public class HighlightWidget : Widget
   {
      public HighlightWidget(ITerminal panel)
         : base(panel)
      {
      }

      public override void Draw()
      {
         if (this.Enabled)
         {
            var currentCharacter = this.Panel.Get(Location);

            this.Panel[Location].Write(currentCharacter.Glyph);
         }
      }

      public Loc Location { get; set; }
      public bool Enabled { get; set; }
   }
}