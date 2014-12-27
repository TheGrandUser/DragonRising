using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;

namespace DraconicEngine.GameWorld.Effects
{
   public class MessageEffect : IEffect
   {
      private string message;
      private RogueColor foreColor;

      public MessageEffect(string message, RogueColor foreColor)
      {
         this.message = message;
         this.foreColor = foreColor;
      }
      public void Do()
      {
         MessageService.Current.PostMessage(message, foreColor);
      }
   }
}
