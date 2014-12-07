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
      Queue<RogueMessage> messages = new Queue<RogueMessage>();
      public Queue<RogueMessage> Messages { get { return messages; } }

      int maxMessages;

      public SimpleMessageService(int maxMessages)
      {
         this.maxMessages = maxMessages;
      }

      public void PostMessage(string message, RogueColor color)
      {
         this.messages.Enqueue(new RogueMessage(message, color));
         if (this.messages.Count > maxMessages)
         {
            this.messages.Dequeue();
         }
      }

      IEnumerable<RogueMessage> IMessageService.Messages { get { return messages.AsEnumerable(); } }
   }
}