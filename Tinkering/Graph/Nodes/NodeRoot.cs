using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkering.Graph.Datums;

namespace Tinkering.Graph.Nodes
{
   class NodeRoot : HObject
   {
      public void OnNameChanged(string newName)
      {
         throw new NotImplementedException();
      }

      public ScriptScope ProxyDict(EvalDatum evalDatum)
      {
         throw new NotImplementedException();
      }
   }
}
