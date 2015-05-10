using Newtonsoft.Json;
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
   public enum SystemTrack
   {
      Game,
      Render,
   }

   [Flags]
   public enum UpdateTrack
   {
      None = 0,
      Game,
      Render,
      Both = Game | Render,
   }

   public interface IEntityStore : IDisposable
   {
      IEnumerable<Entity> Entities { get; }

      void AddEntity(Entity entity);
      bool RemoveEntity(Entity entity);
      void RemoveAllEntities();
      void SendToBack(Entity entity);

      IObservable<Entity> Added { get; }
      IObservable<Entity> Removed { get; }
   }

   public interface IEntityEngine : IEntityStore
   {
      bool IsUpdating { get; }
      IObservable<Unit> UpdateComplete { get; }
      IReadOnlyReactiveList<TNode> GetNodes<TNode>() where TNode : Node, new();
      void ReleaseNodeList(Type nodeType);
      void AddSystem(GameSystem system, int priority, SystemTrack track);
      GameSystem GetSystem(Type type);
      IEnumerable<GameSystem> GameSystems { get; }
      IEnumerable<GameSystem> RenderSystems { get; }
      void RemoveSystem(GameSystem system);
      void RemoveAllSystems();
      Task Update(double time, UpdateTrack track);

      IEntityStore CreateChildStore();
   }

   public class EntityEngine : IEntityEngine
   {
      private Subject<Unit> updateComplete = new Subject<Unit>();

      private Dictionary<Entity, IDisposable> entityList = new Dictionary<Entity, IDisposable>();

      List<ChildEntityStore> childStores = new List<ChildEntityStore>();
      private SortedList<int, GameSystem> gameSystemList = new SortedList<int, GameSystem>();
      private SortedList<int, GameSystem> renderSystemList = new SortedList<int, GameSystem>();
      private Dictionary<Type, IFamily> families = new Dictionary<Type, IFamily>();
      
      Subject<Entity> added = new Subject<Entity>();
      Subject<Entity> removed = new Subject<Entity>();


      public EntityEngine() { }

      public bool IsUpdating { get; private set; }
      public IObservable<Unit> UpdateComplete => updateComplete.AsObservable();

      public void AddEntity(Entity entity)
      {
         if (entityList.ContainsKey(entity))
         {
            throw new ArgumentException($"The entity {entity.Name} is already added.");
         }

         var subscriptions = AddEntityToSystems(entity);

         entityList.Add(entity, subscriptions);

         added.OnNext(entity);
      }

      private IDisposable AddEntityToSystems(Entity entity)
      {
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
            }),
            Disposable.Create(() =>
            {
               foreach (var family in families.Values)
               {
                  family.RemoveEntity(entity);
               }
            }));
         foreach (var family in families.Values)
         {
            family.NewEntity(entity);
         }

         return subscriptions;
      }

      public bool RemoveEntity(Entity entity)
      {
         if (entityList.ContainsKey(entity))
         {
            var subscriptions = entityList[entity];
            subscriptions.Dispose();

            entityList.Remove(entity);
            removed.OnNext(entity);
            return true;
         }
         return false;
      }

      public void RemoveAllEntities()
      {
         foreach (var entity in entityList.Keys.ToArray())
         {
            RemoveEntity(entity);
         }
      }

      public IEnumerable<Entity> Entities
      {
         get
         {
            foreach(var entity in entityList.Keys)
            {
               yield return entity;
            }
            foreach(var childStore in childStores)
            {
               foreach (var entity in childStore.Entities)
               {
                  yield return entity;
               }
            }
         }
      }

      public IReadOnlyReactiveList<TNode> GetNodes<TNode>()
         where TNode : Node, new()
      {
         if (families.ContainsKey(typeof(TNode)))
         {
            return ((IFamily<TNode>)families[typeof(TNode)]).Nodes;
         }
         var family = new ComponentMatchingFamily<TNode>(this);
         families.Add(typeof(TNode), family);
         foreach (var entity in this.Entities)
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

      public void AddSystem(GameSystem system, int priority, SystemTrack track)
      {
         system.priority = priority;
         system.AddToEngine(this);
         if (track == SystemTrack.Game)
         {
            this.gameSystemList.Add(system.priority, system);
         }
         else
         {
            this.renderSystemList.Add(system.priority, system);
         }
      }

      public GameSystem GetSystem(Type type)
      {
         return
            this.gameSystemList.Values.FirstOrDefault(sys => type.IsInstanceOfType(sys)) ??
            this.renderSystemList.Values.FirstOrDefault(sys => type.IsInstanceOfType(sys));
      }

      public IEnumerable<GameSystem> GameSystems => this.gameSystemList.Values.AsEnumerable();
      public IEnumerable<GameSystem> RenderSystems => this.renderSystemList.Values.AsEnumerable();

      public IObservable<Entity> Added => added.AsObservable();

      public IObservable<Entity> Removed => removed.AsObservable();

      public void RemoveSystem(GameSystem system)
      {
         if (gameSystemList.ContainsValue(system))
         {
            gameSystemList.Remove(system.priority);
            system.RemoveFromEngine(this);
         }
         else if (renderSystemList.ContainsValue(system))
         {
            renderSystemList.Remove(system.priority);
            system.RemoveFromEngine(this);
         }
      }

      public void RemoveAllSystems()
      {
         foreach (var system in gameSystemList.Values.Concat(renderSystemList.Values).ToArray())
         {
            RemoveSystem(system);
         }
      }

      IEnumerable<GameSystem> GetInterleavedsystems()
      {
         using (IEnumerator<KeyValuePair<int, GameSystem>>
            gameEnumerator = gameSystemList.GetEnumerator(),
            renderEnumerator = renderSystemList.GetEnumerator())
         {
            var hasGame = gameEnumerator.MoveNext();
            var hasRender = renderEnumerator.MoveNext();

            while (hasGame && hasRender)
            {
               var currentGame = gameEnumerator.Current;
               var currentRender = renderEnumerator.Current;

               if (currentGame.Key <= currentRender.Key)
               {
                  yield return currentGame.Value;
                  hasGame = gameEnumerator.MoveNext();
               }
               else
               {
                  yield return currentRender.Value;
                  hasRender = renderEnumerator.MoveNext();
               }
            }
            while (hasGame)
            {
               yield return gameEnumerator.Current.Value;
               gameEnumerator.MoveNext();
            }
            while (hasRender)
            {
               yield return renderEnumerator.Current.Value;
               renderEnumerator.MoveNext();
            }
         }
      }


      public async Task Update(double time, UpdateTrack track)
      {
         IEnumerable<GameSystem> systems;
         switch (track)
         {
            case UpdateTrack.Game:
               systems = gameSystemList.Values;
               break;
            case UpdateTrack.Render:
               systems = renderSystemList.Values;
               break;
            case UpdateTrack.Both:
               systems = GetInterleavedsystems();
               break;
            default:
               return;
         }

         this.IsUpdating = true;

         foreach (var system in systems)
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
      public void SendToBack(Entity entity)
      {
         //if (this.entityList.ContainsKey(entity))
         //{
         //   var sub = this.entityList[entity];
         //   this.entityList.Remove(entity);
         //   this.entityList.Add(entity, sub);
         //}
      }

      public IEntityStore CreateChildStore()
      {
         var childStore = new ChildEntityStore(this, AddEntityToSystems, OnChildDisposed);

         this.childStores.Add(childStore);

         return childStore;
      }

      private void OnChildDisposed(ChildEntityStore child)
      {
         this.childStores.Remove(child);
      }
      
      public void Dispose()
      {
      }
   }

   public class ChildEntityStore : IEntityStore
   {
      Subject<Entity> added = new Subject<Entity>();
      Subject<Entity> removed = new Subject<Entity>();

      Dictionary<Entity, IDisposable> entities = new Dictionary<Entity, IDisposable>();

      IEntityEngine parent;
      Action<ChildEntityStore> removeSelf;
      Func<Entity, IDisposable> onAdd;

      public ChildEntityStore(IEntityEngine parent,
         Func<Entity, IDisposable> onAdd,
         Action<ChildEntityStore> removeSelf)
      {
         this.parent = parent;
         this.onAdd = onAdd;
         this.removeSelf = removeSelf;
      }
      public IObservable<Entity> Added => added.AsObservable();

      public IObservable<Entity> Removed => removed.AsObservable();

      public IEnumerable<Entity> Entities => entities.Keys.AsEnumerable();
      
      public void AddEntity(Entity entity)
      {
         if(!entities.ContainsKey(entity))
         {
            var sub = onAdd(entity);

            this.entities.Add(entity, sub);

            added.OnNext(entity);
         }
      }

      public bool RemoveEntity(Entity entity)
      {
         if(entities.ContainsKey(entity))
         {
            var sub = this.entities[entity];
            this.entities.Remove(entity);
            sub.Dispose();
            removed.OnNext(entity);
            return true;
         }
         return false;
      }

      public void RemoveAllEntities()
      {
         foreach(var entity in this.entities.Keys.ToArray())
         {
            RemoveEntity(entity);
         }
      }

      public void SendToBack(Entity entity)
      {
         //if (this.entities.Remove(entity))
         //{
         //   this.entities.Add(entity);
         //}
      }

      public void Dispose()
      {
         this.RemoveAllEntities();
         this.removeSelf(this);
      }
   }
}
