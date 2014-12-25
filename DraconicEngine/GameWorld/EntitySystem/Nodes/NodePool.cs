using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem
{
   internal class NodePool<TNode>
      where TNode : Node, new()
   {
      Dictionary<Type, PropertyInfo> components;
      Queue<TNode> nodes = new Queue<TNode>();

      public NodePool(Dictionary<Type, PropertyInfo> components)
      {
         this.components = components;
      }

      public TNode Get() => nodes.Any() ? nodes.Dequeue() : new TNode();

      public void Return(TNode node)
      {
         foreach(var pi in components.Values)
         {
            pi.SetValue(node, null);
         }
         node.Entity = null;
         nodes.Enqueue(node);
      }

      LinkedList<TNode> cachedNodes = new LinkedList<TNode>();
      internal bool Cache(TNode node)
      {
         cachedNodes.AddLast(node);

         return cachedNodes.Count == 1;
      }

      internal void ReleaseCache()
      {
         foreach (var node in cachedNodes)
         {
            Return(node);
         }
         cachedNodes.Clear();
      }
   }
}
