using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;

namespace DragonRising
{
   public class SimpleMessageService : IMessageService
   {
      List<RogueMessage> infoMessages = new List<RogueMessage>();
      Queue<RogueMessage> eventMessages = new Queue<RogueMessage>();

      int maxEventMessages;

      public SimpleMessageService(int maxEventMessages)
      {
         this.maxEventMessages = maxEventMessages;
      }

      public void PostMessage(string message, RogueColor color)
      {
         eventMessages.Enqueue(new RogueMessage(message, color));
         if (eventMessages.Count > maxEventMessages)
         {
            eventMessages.Dequeue();
         }
      }

      public void ClearInfoMessages()
      {
         infoMessages.Clear();
      }

      public void AddInfoMessage(RogueMessage message)
      {
         infoMessages.Add(message);
      }
      
      public IEnumerable<RogueMessage> EventMessages => eventMessages.AsEnumerable();

      public IEnumerable<RogueMessage> InfoMessages => infoMessages.AsEnumerable();
   }
}