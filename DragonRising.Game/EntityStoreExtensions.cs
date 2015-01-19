using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DragonRising.GameWorld.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising
{
   public static class EntityStoreExtensions
   {
      public static IEnumerable<Entity> GetEntitiesAt(this IEntityStore store, Loc location)
      {
         return store.AllEntities.Where(e => e.GetComponentOrDefault<LocationComponent>()?.Location == location);
      }

      public static Entity GetCreatureAt(this IEntityStore store, Loc location)
      {
         var creature = store.AllCreaturesSpecialFirst().FirstOrDefault(c => c.GetComponentOrDefault<LocationComponent>()?.Location == location);

         return creature;
      }

      public static IEnumerable<Entity> AllItems(this IEntityStore store)
      {
         return store.RegularEntities.Where(e => e.HasComponent<ItemComponent>());
      }

      public static IEnumerable<Entity> GetItemsAt(this IEntityStore store, Loc location)
      {
         foreach (var entity in store.AllItems())
         {
            if (entity.GetComponentOrDefault<LocationComponent>()?.Location == location)
            {
               yield return entity;
            }
         }
      }


      public static IEnumerable<Entity> AllCreaturesSpecialFirst(this IEntityStore store)
      {
         foreach (var specialCreature in store.AllSpecialCreatures())
         {
            yield return specialCreature;
         }
         foreach (var creature in store.RegularEntities.Where(e => e.HasComponent<CreatureComponent>()))
         {
            yield return creature;
         }
      }

      public static IEnumerable<Entity> AllCreaturesSpecialLast(this IEntityStore store)
      {
         foreach (var creature in store.RegularEntities.Where(e => e.HasComponent<CreatureComponent>()))
         {
            yield return creature;
         }
         foreach (var specialCreature in store.AllSpecialCreatures())
         {
            yield return specialCreature;
         }
      }

      public static IEnumerable<Entity> AllCreaturesExceptSpecial(this IEntityStore store)
      {
         return store.AllEntities.Where(e => e.HasComponent<CreatureComponent>());
      }

      public static IEnumerable<Entity> AllSpecialCreatures(this IEntityStore store)
      {
         return store.SpecialEntities.Where(e => e.HasComponent<CreatureComponent>());
      }
   }
}
