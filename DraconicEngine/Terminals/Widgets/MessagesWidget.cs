using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Terminals;

namespace DraconicEngine.Widgets
{
   public enum MessagePriority
   {
      ShowOldest,
      ShowNewest,
   }

   public class MessagesWidget : Widget
   {
      IEnumerable<RogueMessage> messagesSource;
      MessagePriority messagePriority;

      public MessagesWidget(
         ITerminal panel,
         IEnumerable<RogueMessage> messagesSource,
         MessagePriority messagePriority = MessagePriority.ShowNewest)
         : base(panel)
      {
         this.messagesSource = messagesSource;
         this.messagePriority = messagePriority;
      }

      public override void Draw()
      {
         IEnumerable<Tuple<string, RogueColor>> processedMessages;
         if (messagePriority == MessagePriority.ShowNewest)
         {
            processedMessages = GetNewestMessages();
         }
         else
         {
            processedMessages = GetOldestMessages();
         }

         int y = 0;
         foreach (var message in processedMessages)
         {
            var line = message.Item1;
            if (line.Length > this.Panel.Size.X)
            {
               line = line.Substring(0, this.Panel.Size.X);
            }
            this.Panel[0, y][message.Item2].Write(line);
            y++;
            if (y >= this.Panel.Size.Y)
            {
               break;
            }
         }
      }

      private IEnumerable<Tuple<string, RogueColor>> GetOldestMessages()
      {
         var messages = new Queue<Tuple<string, RogueColor>>();

         int maxMessages = this.Panel.Size.Y;
         int maxWidth = this.Panel.Size.X;

         foreach (var message in this.messagesSource)
         {
            var text = message.Message;
            var color = message.Color;
            var lines = BreakLargeString(text, maxWidth);

            foreach (var line in lines)
            {
               messages.Enqueue(Tuple.Create(line, color));

               if (messages.Count == maxMessages)
               {
                  return messages;
               }
            }
         }

         return messages;
      }

      private IEnumerable<Tuple<string, RogueColor>> GetNewestMessages()
      {
         var messages = new Stack<Tuple<string, RogueColor>>();

         int maxMessages = this.Panel.Size.Y;
         int maxWidth = this.Panel.Size.X;

         foreach (var message in this.messagesSource.Reverse())
         {
            var text = message.Message;
            var color = message.Color;
            var lines = BreakLargeString(text, maxWidth);

            foreach (var line in lines.Reverse())
            {
               messages.Push(Tuple.Create(line, color));

               if (messages.Count == maxMessages)
               {
                  return messages;
               }
            }
         }

         return messages;
      }

      static IEnumerable<string> BreakLargeString(string text, int maxWidth)
      {
         if (text.Length < maxWidth)
         {
            yield return text;
         }
         else
         {
            int offset = 0;

            while (offset < text.Length)
            {
               if (offset + maxWidth > text.Length)
               {
                  yield return text.Substring(offset);
                  offset = text.Length;
                  yield break;
               }
               else
               {
                  int index = text.LastIndexOf(" ", Math.Min(text.Length, offset + maxWidth));

                  string line = text.Substring(offset, (index - offset <= 0 ? text.Length : index) - offset);

                  offset += line.Length + 1;

                  yield return line;
               }
            }
         }
      }
   }
}