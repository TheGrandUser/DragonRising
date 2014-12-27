using DraconicEngine.GameWorld.Alligences;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
   public static class Extensions
   {
      public static bool IsAdjacent(this Entity me, Entity other)
      {
         var diff =
            me.GetComponentOrDefault<LocationComponent>()?.Location - 
            other.GetComponentOrDefault<LocationComponent>()?.Location;
         return diff?.KingLength == 1;
      }

      public static bool IsAdjacent(this LocationComponent me, LocationComponent other)
      {
         var diff = me.Location - other.Location;
         return diff.KingLength == 1;
      }

      public static bool IsEnemy(this Entity me, Entity other)
      {
         var meCreature = me.GetComponentOrDefault<CreatureComponent>();
         var otherCreature = other.GetComponentOrDefault<CreatureComponent>();

         if(meCreature == null || otherCreature == null)
         {
            return false;
         }


         return AlligenceManager.Current.AreEnemies(meCreature.Alligence, otherCreature.Alligence);
      }

      public static T AsCreature<T>(this Entity entity, Func<CreatureComponent, T> isTrue, T otherwise = default(T))
      {
         var creature = entity.GetComponentOrDefault<CreatureComponent>();
         if(creature != null)
         {
            return isTrue(creature);
         }

         return otherwise;
      }
   }
}