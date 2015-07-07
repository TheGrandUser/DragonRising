using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tinkering.Graph.Hooks
{
   class ScriptUIHooks
   {
      public int GetInstruction()
      {
         throw new NotImplementedException();
      }

      public static object point(List<object> args, Dictionary<object, object> kwargs)
      {
         var self = args[0] as ScriptUIHooks;

         var lineNo = self.GetInstruction();

         if(args.Count != 4 && args.Count != 3)
         {
            throw new HookException("Expected x, y, z as arguments");
         }

         throw new NotImplementedException();
      }

      
   }
}
