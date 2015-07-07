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

namespace Tinkering
{
   class Program
   {
      static void Main(string[] args)
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
