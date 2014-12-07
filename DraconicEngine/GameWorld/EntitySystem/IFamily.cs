using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem
{
   public interface IFamily : IDisposable
   {
      IReadOnlyReactiveList<Node> Nodes { get; }
      void NewEntity(Entity entity);
      void RemoveEntity(Entity entity);
      void ComponentAddedToEntity(Entity entity, Type componentType);
      void ComponentRemovedFromEntity(Entity entity, Type componentType);
   }

   public interface IFamily<TNode> : IFamily
      where TNode : Node
   {
      new IReadOnlyReactiveList<TNode> Nodes { get; }
   }
}
