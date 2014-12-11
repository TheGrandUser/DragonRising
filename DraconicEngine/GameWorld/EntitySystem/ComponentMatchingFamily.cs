using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem
{
   public class ComponentMatchingFamily<TNode> : IFamily<TNode>
      where TNode : Node, new()
   {
      Engine engine;
      ReactiveList<TNode> nodes = new ReactiveList<TNode>();
      Dictionary<Entity, TNode> entities = new Dictionary<Entity, TNode>();
      Dictionary<Type, PropertyInfo> componentFields = new Dictionary<Type, PropertyInfo>();
      NodePool<TNode> nodePool;

      public ComponentMatchingFamily(Engine engine)
      {
         this.engine = engine;

         nodePool = new NodePool<TNode>(this.componentFields);

         nodePool.Return(nodePool.Get());

         var nodeType = typeof(TNode);

         var nodeVariables = nodeType.GetProperties()
            .Where(pi => pi.PropertyType.GetInterface(nameof(Component)) != null);

         foreach (var variable in nodeVariables)
         {
            componentFields[variable.PropertyType] = variable;
         }
      }

      public IReadOnlyReactiveList<TNode> Nodes => nodes;
      IReadOnlyReactiveList<Node> IFamily.Nodes => nodes;

      public void ComponentAddedToEntity(Entity entity, Type componentType)
      {
         AddIfMatch(entity);
      }

      public void ComponentRemovedFromEntity(Entity entity, Type componentType)
      {
         if (componentFields.ContainsKey(componentType))
         {
            RemoveIfMatch(entity);
         }
      }

      public void Dispose()
      {
         foreach (var node in nodes)
         {
            entities.Remove(node.Entity);
         }
         nodes.Clear();
      }

      public void NewEntity(Entity entity)
      {
         AddIfMatch(entity);
      }

      public void RemoveEntity(Entity entity)
      {
         RemoveIfMatch(entity);
      }

      private void AddIfMatch(Entity entity)
      {
         if (!entities.ContainsKey(entity))
         {
            if (componentFields.Keys.Any(componentType => !entity.HasComponent(componentType)))
            {
               return;
            }

            var node = nodePool.Get();

            node.Entity = entity;
            node.SetComponents(entity);
            entities[entity] = node;
            nodes.Add(node);
         }
      }

      private void RemoveIfMatch(Entity entity)
      {
         if (entities.ContainsKey(entity))
         {
            var node = entities[entity];

            entities.Remove(entity);
            nodes.Remove(node);
            if (engine.IsUpdating)
            {
               if (nodePool.Cache(node))
               {
                  engine.UpdateComplete.FirstOrDefaultAsync().Subscribe(ReleaseNodePoolCache);
               }
            }
            else
            {
               nodePool.Return(node);
            }
         }
      }

      private void ReleaseNodePoolCache(Unit _)
      {
         nodePool.ReleaseCache();
      }
   }
}