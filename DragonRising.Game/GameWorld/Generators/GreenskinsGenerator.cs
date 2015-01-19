using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.Behaviors;
using DragonRising.GameWorld.Alligences;
using DragonRising.Storage;

namespace DragonRising.Generators
{
   public class GreenskinsGenerator : IPopulationGenerator
   {
      Random random = new Random();
      public Alligence GreenskinsAllignce { get; set; }
      public GreenskinsGenerator()
      {
         GreenskinsAllignce = AlligenceManager.Current.GetOrAddByName("Greenskins");
      }

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
            monsterTemplate = Library.Current.Entities.Get("Orc");
         }
         else
         {
            monsterTemplate = Library.Current.Entities.Get("Troll");
         }

         var monster = monsterTemplate.Clone();

         var alligence = 

         monster.AsCreature(c => c.Alligence = GreenskinsAllignce);

         return monster;
      }
   }
}