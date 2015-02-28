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

   public class Engine
   {
      private Subject<Unit> updateComplete = new Subject<Unit>();

      private Dictionary<Entity, IDisposable> entityList = new Dictionary<Entity, IDisposable>();


      private SortedList<int, GameSystem> gameSystemList = new SortedList<int, GameSystem>();
      private SortedList<int, GameSystem> renderSystemList = new SortedList<int, GameSystem>();
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
            throw new ArgumentException($"The entity {entity.Name} is already added.");
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
   }
}
