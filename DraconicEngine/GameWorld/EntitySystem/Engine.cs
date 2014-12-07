using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem
{
   public class Engine
   {
      private Subject<Unit> updateComplete = new Subject<Unit>();

      private Dictionary<Entity, IDisposable> entityList = new Dictionary<Entity, IDisposable>();


      private SortedList<int, GameSystem> systemList = new SortedList<int, GameSystem>();
      private Dictionary<Type, IFamily> families = new Dictionary<Type, IFamily>();

      public Engine()
      {

      }

      public bool IsUpdating { get; private set; }
      public IObservable<Unit> UpdateComplete => updateComplete.AsObservable();

      public void AddEntity(Entity entity)
      {
         if (entityList.ContainsKey(entity))
         {
            throw new ArgumentException("The entity \{entity.Name} is already added.");
         }

         var subscriptions = new CompositeDisposable(
            entity.ComponentAdded.Subscribe(comp =>
            {
               foreach (var family in families.Values)
               {
                  family.ComponentAddedToEntity(entity, comp.GetType());
               }
            }),
            entity.ComponentRemoved.Subscribe(comp =>
            {
               foreach (var family in families.Values)
               {
                  family.ComponentRemovedFromEntity(entity, comp.GetType());
               }
            }));
         foreach (var family in families.Values)
         {
            family.NewEntity(entity);
         }
         entityList.Add(entity, subscriptions);
      }

      public IDisposable ObserveStore(IEntityStore entityStore)
      {
         return new CompositeDisposable(
            entityStore.AllEntities.ToObservable().Merge(entityStore.Added).Subscribe(this.AddEntity),
            entityStore.Removed.Subscribe(this.RemoveEntity));
      }

      public void RemoveEntity(Entity entity)
      {
         if (entityList.ContainsKey(entity))
         {
            var subscriptions = entityList[entity];
            subscriptions.Dispose();
            foreach (var family in families.Values)
            {
               family.RemoveEntity(entity);
            }

            entityList.Remove(entity);
         }
      }

      public void RemoveAllEntities()
      {
         foreach (var entity in entityList.Keys.ToArray())
         {
            RemoveEntity(entity);
         }
      }

      public IEnumerable<Entity> Entities => entityList.Keys.AsEnumerable();

      public IReadOnlyReactiveList<TNode> GetNodes<TNode>()
         where TNode : Node, new()
      {
         if (families.ContainsKey(typeof(TNode)))
         {
            return ((IFamily<TNode>)families[typeof(TNode)]).Nodes;
         }
         var family = new ComponentMatchingFamily<TNode>(this);
         families.Add(typeof(TNode), family);
         foreach (var entity in entityList.Keys)
         {
            family.NewEntity(entity);
         }
         return family.Nodes;
      }

      public void ReleaseNodeList(Type nodeType)
      {
         if (families.ContainsKey(nodeType))
         {
            families[nodeType].Dispose();
            families.Remove(nodeType);
         }
      }

      public void AddSystem(GameSystem system, int priority)
      {
         system.priority = priority;
         system.AddToEngine(this);
         this.systemList.Add(system.priority, system);
      }

      public GameSystem GetSystem(Type type)
      {
         return this.systemList.Values.FirstOrDefault(sys => type.IsInstanceOfType(sys));
      }

      public IEnumerable<GameSystem> Systems => this.systemList.Values.AsEnumerable();

      public void RemoveSystem(GameSystem system)
      {
         systemList.Remove(system.priority);
         system.RemoveFromEngine(this);
      }

      public void RemoveAllSystems()
      {
         foreach (var system in systemList.Values.ToArray())
         {
            RemoveSystem(system);
         }
      }

      public async Task Update(double time)
      {
         this.IsUpdating = true;
         foreach (var system in systemList.Values)
         {
            if (system is GameSystemSync)
            {
               var syncSystem = (GameSystemSync)system;
               syncSystem.Update(time);
            }
            else if (system is GameSystemAsync)
            {
               var asyncSystem = (GameSystemAsync)system;
               await asyncSystem.Update(time);
            }
         }
         this.IsUpdating = false;
         updateComplete.OnNext(Unit.Default);
      }
   }
}
