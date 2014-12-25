using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Generators;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.Behaviors;
using DraconicEngine.GameWorld.Alligences;
using DraconicEngine.Storage;

namespace DragonRising.Generators
{
   public class GreenskinsGenerator : IPopulationGenerator
   {
      Random random = new Random();
      Alligence greenskins = new Alligence() { Name = "Greenskins" };

      public List<Entity> GenerarateMonsters(int min, int max)
      {
         var count = random.Next(min, max + 1);

         var monsters = new List<Entity>();

         for (int i = 0; i < count; i++)
         {
            monsters.Add(GenerarateMonster());
         }

         return monsters;
      }

      public Entity GenerarateMonster()
      {
         Entity monsterTemplate;
         var value = random.NextDouble();
         if (value < 0.8)
         {
            monsterTemplate = EntityLibrary.Current.Get("Orc");
         }
         else
         {
            monsterTemplate = EntityLibrary.Current.Get("Troll");
         }

         return monsterTemplate.Clone();
      }
   }
}