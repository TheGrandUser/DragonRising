using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Items;
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
      IEnumerable<Entity> AllItems { get; }
      IEnumerable<Entity> AllNonCreatures { get; }

      IEnumerable<Entity> AllCreaturesSpecialFirst { get; }
      IEnumerable<Entity> AllCreaturesSpecialLast { get; }
      IEnumerable<Entity> AllCreaturesExceptSpecial { get; }
      IEnumerable<Entity> AllSpecialCreatures { get; }


      void Add(Entity entity);
      bool Remove(Entity entity);
      void SetSpecial(Entity creature, bool isSpecial);
      void SendToBack(Entity entity);

      IEnumerable<Entity> GetEntitiesAt(Loc location);
      Entity GetCreatureAt(Loc location);
      IEnumerable<Entity> GetItemsAt(Loc location);

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
      HashSet<Entity> specialCreatures = new HashSet<Entity>();
      
      public IObservable<Entity> Added => added.AsObservable();
      
      public IObservable<Entity> Removed => removed.AsObservable();
      public IObservable<Entity> Killed => killed.AsObservable();

      public IEnumerable<Entity> AllEntities
      {
         get
         {
            foreach (var specialCreature in this.specialCreatures)
            {
               yield return specialCreature;
            }
            foreach (var entity in this.entities)
            {
               yield return entity;
            }
         }
      }

      public IEnumerable<Entity> AllItems
      {
         get { return entities.Where(e => e.HasComponent<ItemComponent>()); }
      }

      public IEnumerable<Entity> AllNonCreatures
      {
         get { return entities.Where(e => !e.HasComponent<CreatureComponent>()); }
      }

      public IEnumerable<Entity> AllCreaturesSpecialFirst
      {
         get
         {
            foreach (var specialCreature in this.specialCreatures)
            {
               yield return specialCreature;
            }
            foreach (var creature in this.entities.Where(e => e.HasComponent<CreatureComponent>()))
            {
               yield return creature;
            }
         }
      }

      public IEnumerable<Entity> AllCreaturesSpecialLast
      {
         get
         {
            foreach (var creature in this.entities.Where(e => e.HasComponent<CreatureComponent>()))
            {
               yield return creature;
            }
            foreach (var specialCreature in this.specialCreatures)
            {
               yield return specialCreature;
            }
         }
      }

      public IEnumerable<Entity> AllCreaturesExceptSpecial
      {
         get { return entities.Where(e => e.HasComponent<CreatureComponent>()); }
      }

      public IEnumerable<Entity> AllSpecialCreatures
      {
         get { return this.specialCreatures.AsEnumerable(); }
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
         if (!wasRemoved && entity.HasComponent<CreatureComponent>())
         {
            wasRemoved = this.specialCreatures.Remove(entity);
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
            this.specialCreatures.Add(creature);
         }
         else
         {
            if (this.specialCreatures.Contains(creature))
            {
               this.specialCreatures.Remove(creature);
            }
            this.entities.Add(creature);
         }
      }

      public IEnumerable<Entity> GetEntitiesAt(Loc location)
      {
         return this.AllEntities.Where(e => e.Location == location);
      }

      public Entity GetCreatureAt(Loc location)
      {
         var creature = this.AllCreaturesSpecialFirst.FirstOrDefault(c => c.Location == location);

         return creature;
      }

      public IEnumerable<Entity> GetItemsAt(Loc location)
      {
         foreach (var entity in this.AllItems)
         {
            if (entity.Location == location)
            {
               yield return entity;
            }
         }
      }

      public void SendToBack(Entity entity)
      {
         if (this.entities.Contains(entity))
         {
            this.entities.Remove(entity);
            this.entities.Add(entity);
         }
         else if (entity.HasComponent<CreatureComponent>())
         {
            if (this.specialCreatures.Contains(entity))
            {
               this.specialCreatures.Remove(entity);
               this.specialCreatures.Add(entity);
            }
         }
      }

      public void KillEntity(Entity entity)
      {
         if (!entity.HasComponent<CombatantComponent>() || !entity.HasComponent<CreatureComponent>())
         {
            return;
         }

         killed.OnNext(entity);
      }
   }
}
