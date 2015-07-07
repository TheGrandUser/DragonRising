using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkering.Graph.Datums;

namespace Tinkering.Graph.Hooks
{
   static class Hook
   {
      public static void PreInit()
      {

      }

      public static dynamic LoadHooks(dynamic g, ScriptDatum d)
      {
         return null;
      }
   }

   class HookException : Exception
   {
      public HookException(string message)
         : base(message)
      {

      }
   }
}
