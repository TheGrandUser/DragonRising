using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkering.Graph.Datums;

namespace Tinkering.Graph.Nodes
{
   class Node : HObject
   {
      public Node()
      {

      }

      public Datum GetDatum(string name)
      {
         throw new NotImplementedException();
      }
   }

   delegate Node NodeConstructor(NodeRoot root);

   static class NodeExtensions
   {
      public static T GetDatum<T>(this Node node, string name)
         where T : Datum
      {
         return (T)node.GetDatum(name);
      }
   }
}
