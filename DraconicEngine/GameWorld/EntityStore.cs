using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DraconicEngine
{
   public interface IEntityStore
   {
      IEnumerable<Entity> AllEntities { get; }
      IEnumerable<Entity> RegularEntities { get; }
      IEnumerable<Entity> SpecialEntities { get; }


      void Add(Entity entity);
      bool Remove(Entity entity);
      void SetSpecial(Entity creature, bool isSpecial);
      void SendToBack(Entity entity);

      IObservable<Entity> Added { get; }
      IObservable<Entity> Removed { get; }

      IObservable<Entity> Killed { get; }
      void KillEntity(Entity owner);
   }

   [Serializable]
   public class EntityStore : IEntityStore
   {
      [NonSerialized]
      [JsonIgnore]
      Subject<Entity> added = new Subject<Entity>();
      [NonSerialized]
      [JsonIgnore]
      Subject<Entity> removed = new Subject<Entity>();

      [NonSerialized]
      [JsonIgnore]
      Subject<Entity> killed = new Subject<Entity>();

      HashSet<Entity> entities = new HashSet<Entity>();
      HashSet<Entity> specialEntities = new HashSet<Entity>();

      public IObservable<Entity> Added => added.AsObservable();

      public IObservable<Entity> Removed => removed.AsObservable();
      public IObservable<Entity> Killed => killed.AsObservable();

      public IEnumerable<Entity> RegularEntities => this.entities.AsEnumerable();
      public IEnumerable<Entity> SpecialEntities => this.specialEntities.AsEnumerable();
      public IEnumerable<Entity> AllEntities
      {
         get
         {
            foreach (var specialEntity in this.specialEntities)
            {
               yield return specialEntity;
            }
            foreach (var entity in this.entities)
            {
               yield return entity;
            }
         }
      }

      public void Add(Entity entity)
      {
         if (this.entities.Add(entity))
         {
            added.OnNext(entity);
         }
      }

      public bool Remove(Entity entity)
      {
         var wasRemoved = this.entities.Remove(entity);
         if (!wasRemoved)
         {
            wasRemoved = this.specialEntities.Remove(entity);
         }

         if (wasRemoved)
         {
            this.removed.OnNext(entity);
         }

         return wasRemoved;
      }

      public void SetSpecial(Entity creature, bool isSpecial)
      {
         if (isSpecial)
         {
            if (this.entities.Contains(creature))
            {
               this.entities.Remove(creature);
            }
            this.specialEntities.Add(creature);
         }
         else
         {
            if (this.specialEntities.Contains(creature))
            {
               this.specialEntities.Remove(creature);
            }
            this.entities.Add(creature);
         }
      }

      public void SendToBack(Entity entity)
      {
         if (this.entities.Contains(entity))
         {
            this.entities.Remove(entity);
            this.entities.Add(entity);
         }
         else if (this.specialEntities.Contains(entity))
         {
            this.specialEntities.Remove(entity);
            this.specialEntities.Add(entity);
         }
      }

      public void KillEntity(Entity entity)
      {
         //if (!entity.HasComponent<CombatantComponent>() || !entity.HasComponent<CreatureComponent>())
         //{
         //   return;
         //}

         killed.OnNext(entity);
      }
   }
}
