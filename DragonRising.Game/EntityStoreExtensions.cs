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
         return store.Entities.Where(e => e.GetComponentOrDefault<LocationComponent>()?.Location == location);
      }

      public static Entity GetCreatureAt(this IEntityStore store, Loc location)
      {
         var creature = store.AllCreatures().FirstOrDefault(c => c.GetComponentOrDefault<LocationComponent>()?.Location == location);

         return creature;
      }

      public static IEnumerable<Entity> AllItems(this IEntityStore store)
      {
         return store.Entities.Where(e => e.HasComponent<ItemComponent>());
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
      
      public static IEnumerable<Entity> AllCreatures(this IEntityStore store)
      {
         foreach (var creature in store.Entities.Where(e => e.HasComponent<CreatureComponent>()))
         {
            yield return creature;
         }
      }
   }
}
