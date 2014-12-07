using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Terminals;

namespace DraconicEngine.Widgets
{
   public abstract class Widget
   {
      public ITerminal Panel { get; set; }

      public Widget(ITerminal panel)
      {
         this.Panel = panel;
      }

      public abstract void Draw();
   }
}