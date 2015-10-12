using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.EntitySystem
{
   public class ComponentMatchingFamily<TNode> : IFamily<TNode>
      where TNode : Node, new()
   {
      EntityEngine engine;
      ReactiveList<TNode> nodes = new ReactiveList<TNode>();
      Dictionary<Entity, TNode> entities = new Dictionary<Entity, TNode>();
      Dictionary<Type, PropertyInfo> componentFields = new Dictionary<Type, PropertyInfo>();
      HashSet<Type> shouldNotHave = new HashSet<Type>();
      NodePool<TNode> nodePool;

      public ComponentMatchingFamily(EntityEngine engine)
      {
         this.engine = engine;

         nodePool = new NodePool<TNode>(this.componentFields);

         nodePool.Return(nodePool.Get());

         var nodeType = typeof(TNode);

         var nodeVariables = nodeType.GetProperties()
            .Where(pi => typeof(Component).IsAssignableFrom(pi.PropertyType));
         Debug.Assert(nodeVariables.Any(), "No node variables");
         foreach (var variable in nodeVariables)
         {
            componentFields[variable.PropertyType] = variable;
         }

         foreach(var attribute in nodeType.GetCustomAttributes<DoesNotHaveAttribute>())
         {
            this.shouldNotHave.Add(attribute.ComponentType);
         }
      }

      public IReadOnlyReactiveList<TNode> Nodes => nodes;
      IReadOnlyReactiveList<Node> IFamily.Nodes => nodes;

      public void ComponentAddedToEntity(Entity entity, Type componentType)
      {
         if (componentFields.ContainsKey(componentType))
         {
            AddIfMatch(entity);
         }
         else if (shouldNotHave.Contains(componentType))
         {
            RemoveIfMatch(entity);
         }
      }

      public void ComponentRemovedFromEntity(Entity entity, Type componentType)
      {
         if (componentFields.ContainsKey(componentType))
         {
            RemoveIfMatch(entity);
         }
         else if (shouldNotHave.Contains(componentType))
         {
            AddIfMatch(entity);
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
            if (
               !componentFields.Keys.All(componentType => entity.HasComponent(componentType)) ||
               shouldNotHave.Any(componentType => entity.HasComponent(componentType)))
            {
               return;
            }
            

            var node = nodePool.Get();

            node.Entity = entity;
            foreach(var pi in this.componentFields.Values)
            {
               var component = entity.GetComponent(pi.PropertyType);
               pi.SetValue(node, component);
            }
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