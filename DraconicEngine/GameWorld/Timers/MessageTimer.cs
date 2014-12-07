using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Timers
{
   public class MessageTimer : TurnTimer
   {
      private string message;
      private RogueColor foreColor;

      public MessageTimer(int duration, string message, RogueColor foreColor)
         : base(duration)
      {
         this.message = message;
         this.foreColor = foreColor;
      }
      protected override void Trigger()
      {
         MessageService.Current.PostMessage(message, foreColor);
      }
   }
}
