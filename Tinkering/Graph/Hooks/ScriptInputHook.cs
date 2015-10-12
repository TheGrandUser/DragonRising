using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkering.Graph.Datums;

namespace Tinkering.Graph.Hooks
{
   class ScriptInputHook
   {
      ScriptDatum datum;

      public ScriptInputHook()
      {
      }

      public ScriptInputHook(ScriptDatum d)
      {
         datum = d;
      }

      public void call(string name, dynamic type)
      {
         datum.MakeInput(name, type);
      }

      public void callWithDefault(string name, dynamic type, dynamic defaultValue)
      {
         datum.MakeInput(name, type, defaultValue);
      }


   }
}
