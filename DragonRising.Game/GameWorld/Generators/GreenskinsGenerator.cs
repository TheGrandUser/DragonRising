using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Alligences;
using DragonRising.Storage;
using DraconicEngine.Utilities;
using LanguageExt;
using static LanguageExt.Prelude;
using static DraconicEngine.Utilities.ItemSelection;

namespace DragonRising.Generators
{
   public class GreenskinsGenerator : IPopulationGenerator
   {
      public Alligence GreenskinsAllignce { get; set; }
      Dictionary<Entity, Either<double, IEnumerable<Tuple<double, int>>>> monsters;
      List<Tuple<int, int>> monstersPerRoomByLevel;

      public GreenskinsGenerator()
      {
         GreenskinsAllignce = AlligenceManager.Current.GetOrAddByName("Greenskins");

         monsters = new Dictionary<Entity, Either<double, IEnumerable<Tuple<double, int>>>>()
         {
            { Library.Current.Entities.Get("Orc"), Make(0.8) },
            { Library.Current.Entities.Get("Troll"), Make(Tuple(0.15, 3), Tuple(.30, 5), Tuple(.60, 7)) }
         };

         this.monstersPerRoomByLevel = new List<Tuple<int, int>>
         {
            Tuple(2,1),
            Tuple(3,4),
            Tuple(5,6),
         };
      }


      public List<Entity> GenerarateMonsters(int level)
      {
         var max = ItemSelection.FromDungeonLevel(level, monstersPerRoomByLevel);

         var count = max.Match(
            Some: m => RogueGame.Current.GameRandom.Next(m + 1),
            None: () => 0);
         
         var monsters = new List<Entity>();

         for (int i = 0; i < count; i++)
         {
            monsters.Add(GenerarateMonster(level));
         }

         return monsters;
      }

      public Entity GenerarateMonster(int level)
      {
         Entity monsterTemplate = ItemSelection.RandomChoice(level, this.monsters);
         
         var monster = monsterTemplate.Clone();

         monster.AsCreature(c => c.Alligence = GreenskinsAllignce);

         return monster;
      }
   }
}