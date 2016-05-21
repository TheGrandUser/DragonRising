using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising;
using DragonRising.GameWorld;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Alligences;
using DraconicEngine;
using DragonRising.Generators;
using System.IO;
using static System.Console;
using static DraconicEngine.Utilities.StringPresentationFormatter;
using static LanguageExt.Prelude;
using LanguageExt;

namespace Tinkering
{
   class Program
   {
      static void Main(string[] args)
      {
         Experiment2();
         WriteLine("Press any key to continue");
         ReadKey();
      }

      private static void Experiment2()
      {
         var speciesPath = Path.GetFullPath(@"..\..\..\DragonRising.Game\Data\DragonSpecies.json");
         var r = new Random();
         var speciesData = SpeciesGenerator.LoadSpeciesData(speciesPath);
         var dragons = SpeciesGenerator.GenerateDragons(r, 10, speciesData).ToList();

         foreach (var dragon in dragons)
         {
            PrintDragon(dragon);
            WriteLine();
         }
      }

      private static void PrintDragon(DragonSpecies d)
      {
         WriteLine(MakePresentable($"This {d.Size.Desc} species of dragon has {d.ScalePattern.Desc}, {d.HeadDecorations.Desc} upon their heads, and /a {d.Tail.Desc} tail"));
         WriteLine($"They tend to live in {d.Environment.Desc} and find {d.FavoredPrey.Desc} to be most delicious and healthy for them");
         WriteLine($"They have {d.Hearing.Desc}, {d.Sight.Desc}, and {d.Scent.Desc}");
         WriteLine($"For mobility, they have {d.Limbs.Desc} and {d.Wings.Desc} wings");
         WriteLine(MakePresentable($"They have a natural armament of {d.Claws.Desc}, {d.Teeth.Desc}, and /a {d.BreathWeapon.Name} breath weapon"));
         
         //d.Tail

         //d.Hearing
         //d.Scent
         //d.Sight

         //d.Limbs
         //d.Speed
         //d.Wings

         //magic

         WriteLine($"Their stat bonus are AWR {d.AwarenessBonus}, DTR {d.DeterminationBonus}, FIT {d.FitnessBonus}, GRC {d.GraceBonus}, GLE {d.GuileBonus}, PRC {d.PrecisionBonus}");
      }

      static void Expierment1()
      {
         Entity character = new Entity("Test character",
            new ComponentSet(
               new CreatureComponent(new Alligence(), 6),
               new CombatantComponent(),
               new InventoryComponent(),
               new EquipmentComponent(),
               new DrawnComponent()
               {
                  SeenCharacter = new Character(Glyph.At, RogueColors.White),
               }));

         var fireBreath = new PowerPerk()
         {
            Name = "Fire Breath",
            Power = new Power()
            {
               Name = "Fire Breath",
               Parameters = new List<ParameterInfo>()
               {
                  new ParameterInfo<Direction>() { Name = "Direction" }
               },
               Frequency = new RechargeFrequency() { Target = 5, Range = 6 },
               Cost = Cost.None,
               Procedure = new AreaSelector()
               {
                  Area = new ConeArea()
                  {
                     Length = 8,
                     Width = 3
                  },
                  Procedure = new SaveForHalf()
                  {
                     DCExpression = "8+ConMod+Prof",
                     Check = "Dexterity",
                     Damage = new MakeDamageEffect() { DamageExpression = "6d6", Tag = "Fire" },
                  }
               }
            }
         };

         var dragonRace = new Race(name: "Tamanith Dragon");
         dragonRace.Perks.Add(fireBreath);
      }
   }

   class Race
   {
      public Race(string name)
      {
         Name = name;
      }

      void ApplyTo(Entity entity)
      {
      }

      public string Name { get; }
      public List<Perk> Perks { get; } = new List<Perk>();
   }

   class NpcTemplate
   {

   }
}
