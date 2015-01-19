using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Terminals;
using DraconicEngine;
using DraconicEngine.Widgets;

namespace DragonRising.Widgets
{
   public class BarWidget : Widget
   {
      private Func<int> getValue;
      private Func<int> getMaxValue;
      private string name;

      public RogueColor BarColor { get; set; }
      public RogueColor BackColor { get; set; }

      public BarWidget(ITerminal panel, string name, Func<int> getValue, Func<int> getMaxValue, RogueColor barColor, RogueColor backColor)
         : base(panel)
      {
         this.name = name;
         this.getValue = getValue;
         this.getMaxValue = getMaxValue;
         this.BarColor = barColor;
         this.BackColor = backColor;
      }

      public override void Draw()
      {
         int value = getValue();
         int maximum = getMaxValue();

         var barWidth = (int)(this.Panel.Size.X * (double)value / maximum);

         this.Panel[RogueColors.Black, BackColor].Fill(Glyph.Space);
         if (barWidth > 0)
         {
            this.Panel[RogueColors.Black, BarColor][0, 0, barWidth, 1].Fill(Glyph.Space);
         }

         string message = string.Format("{0}: {1}/{2}", name, value, maximum);
         if (message.Length < this.Panel.Size.X)
         {
            var margin = (this.Panel.Size.X - message.Length) / 2;
            this.Panel[margin, 0][RogueColors.White, null].Write(message);
         }
         else
         {
            this.Panel[RogueColors.White, null].Write(message.Substring(0, this.Panel.Size.X));
         }
      }
   }
}
