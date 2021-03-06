﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
   public interface IMessageService
   {
      IEnumerable<RogueMessage> EventMessages { get; }
      void PostMessage(string message, RogueColor color);

      IEnumerable<RogueMessage> InfoMessages { get; }
      void AddInfoMessage(RogueMessage message);
      void ClearInfoMessages();
   }

   public static class MessageService
   {
      static IMessageService currentService;

      public static IMessageService Current { get { return currentService; } }

      public static void SetMessageService(IMessageService messageService)
      {
         currentService = messageService;
      }
   }

   public static class MessageServiceExtensions
   {
      public static void PostMessage(this IMessageService messageService, string message)
      {
         messageService.PostMessage(message, RogueColors.White);
      }
   }
}
