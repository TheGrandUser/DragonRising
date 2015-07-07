using DraconicEngine;
using DragonRising.GameWorld.Alligences;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.GameWorld;

namespace DragonRising
{
   public static class EntityExtensions
   {
      public static bool IsAdjacent(this Entity me, Entity other)
      {
         var diff =
            me.Location -
            other.Location;
         return diff.KingLength == 1;
      }
      
      public static bool IsEnemy(this Entity me, Entity other)
      {
         var meCreature = me.GetComponentOrDefault<CreatureComponent>();
         var otherCreature = other.GetComponentOrDefault<CreatureComponent>();

         if (meCreature == null || otherCreature == null)
         {
            return false;
         }

         
         return World.Current.Alligences.AreEnemies(meCreature.Alligence, otherCreature.Alligence);
      }

      public static T AsCreature<T>(this Entity entity, Func<CreatureComponent, T> isTrue, T otherwise = default(T))
      {
         var creature = entity.GetComponentOrDefault<CreatureComponent>();
         if (creature != null)
         {
            return isTrue(creature);
         }

         return otherwise;
      }
      public static Vector DistanceTo(this Entity self, Entity other)
      {
         return other.GetLocation() - self.GetLocation();
      }

      public static Loc GetLocation(this Entity entity)
      {
         return entity.Location;
      }

      public static void SetLocation(this Entity entity, Loc value)
      {
         entity.Location = value;
      }

      public static bool GetBlocks(this Entity entity)
      {
         return entity.Blocks;
      }

      public static void SetBlocks(this Entity entity, bool value)
      {
         entity.Blocks = value;
      }
   }
}
