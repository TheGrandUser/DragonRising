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
         Entity monster = null;
         var value = random.NextDouble();
         if (value < 0.8)
         {
            monster = new Entity("Orc", Glyph.OLower, RogueColors.LightGreen, blocks: true);
            monster.AddComponent(new CombatantComponent(hp: 10, defense: 0, power: 3));
         }
         else
         {
            monster = new Entity("Troll", Glyph.TUpper, RogueColors.LightGreen, blocks: true);
            monster.AddComponent(new CombatantComponent(hp: 16, defense: 1, power: 4));
         }

         monster.AddComponent(new DecisionComponent());
         monster.AddComponent(new BehaviorComponent(new BasicMonsterBehavior()));
         monster.AddComponent(new CreatureComponent() { Alligence = greenskins });

         return monster;
      }
   }
}