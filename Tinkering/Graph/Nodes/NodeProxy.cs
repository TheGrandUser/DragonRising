using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkering.Graph.Datums;

namespace Tinkering.Graph.Nodes
{
   class NodeProxy
   {
      public Node Node { get; set; }
      public Datum Caller { get; set; }
      public bool Settable { get; set; }

      public object GetAttr(string name)
      {
         var datum = Node.GetDatum(name);

         if (datum == null)
         {
            throw new ProxyException("Nonexistent datum lookup.");
         }

         if(Caller != null)
         {
            if (!Caller.ConnectUpstream(datum))
            {
               throw new ProxyException("");
            }

            Debug.Assert(datum.Parent is Node);
            var n = (Node)datum.Parent;

            var nodeName = n.GetDatum("__name");
            Caller.ConnectUpstream(nodeName);
         }

         if (!datum.IsValid)
         {
            throw new ProxyException("Invalid datum lookup.");
         }

         var value = datum.Value;
         
         return value;
      }

      public void SetAttr(string name, object obj)
      {

      }
   }

   class ProxyException : Exception
   {
      public ProxyException(string message)
         : base(message)
      {

      }
   }
}
