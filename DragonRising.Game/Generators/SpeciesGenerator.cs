using DraconicEngine.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DraconicEngine.Utilities.StringPresentationFormatter;
using static LanguageExt.Prelude;
using LanguageExt;

namespace DragonRising.Generators
{
   public class Range { public int Min { get; set; } public int Max { get; set; } }

   public class Trait
   {
      public string Name { get; set; }
      public string Desc { get; set; }
      public override string ToString() => $"Trait {Name}, {Desc}";

      public List<Trait> SubTraits { get; set; }
   }

   public class ValueTrait : Trait
   {
      public int Value { get; set; }
      public override string ToString() => $"Trait {Name}, {Desc} with value {Value}";
   }

   public class BreathWeaponTemplate : Trait
   {
      public string Material { get; set; }
      public string Shape { get; set; }
      public int Range { get; set; }
      public int? Radius { get; set; }
      public int Power { get; set; }
      public override string ToString() => $"Breath weapon {Name}, {Desc}";
   }

   public class Species
   {
      public string Name { get; set; }
      public ValueTrait Size { get; set; }
      public int Speed { get; set; }

      public int AwarenessBonus { get; set; }
      public int DeterminationBonus { get; set; }
      public int FitnessBonus { get; set; }
      public int GraceBonus { get; set; }
      public int GuileBonus { get; set; }
      public int PrecisionBonus { get; set; }

      public ImmutableList<Trait> Traits { get; set; }
   }

   public class DragonSpecies : Species
   {
      public BreathWeaponTemplate BreathWeapon { get; set; }
      public ValueTrait Claws { get; set; }
      public ValueTrait Teeth { get; set; }
      public Trait Limbs { get; set; }
      public Trait Wings { get; set; }
      public Trait Tail { get; set; }
      public Trait Sight { get; set; }
      public Trait Hearing { get; set; }
      public Trait Scent { get; set; }

      public ValueTrait MagicFlow { get; set; }
      public ValueTrait MagicGeneration { get; set; }
      public ValueTrait MagicDraw { get; set; }
      public ValueTrait MagicManipulator { get; set; }

      public Trait ScalePattern { get; set; }
      public Trait HeadDecorations { get; set; }
      public Trait Environment { get; set; }
      public Trait FavoredPrey { get; set; }
   }

   class StatBonuses
   {
      public int AwarenessBonus { get; set; }
      public int DeterminationBonus { get; set; }
      public int FitnessBonus { get; set; }
      public int GraceBonus { get; set; }
      public int GuileBonus { get; set; }
      public int PrecisionBonus { get; set; }
   }

   class PhysiolgyTraits
   {
      public ValueTrait Size { get; set; }
      public int Speed { get; set; }

      public ValueTrait Claws { get; set; }
      public ValueTrait Teeth { get; set; }
      public Trait Limbs { get; set; }
      public Trait Wings { get; set; }
      public Trait Tail { get; set; }
      public Trait Sight { get; set; }
      public Trait Hearing { get; set; }
      public Trait Scent { get; set; }
   }

   class MagicTraits
   {
      public ValueTrait MagicFlow { get; set; }
      public ValueTrait MagicGeneration { get; set; }
      public ValueTrait MagicDraw { get; set; }
      public ValueTrait MagicManipulator { get; set; }
   }

   class CosmeticTraits
   {
      public Trait ScalePattern { get; set; }
      public Trait HeadDecorations { get; set; }
      public Trait Environment { get; set; }
      public Trait FavoredPrey { get; set; }
   }

   public static class SpeciesGenerator
   {
      public static IEnumerable<DragonSpecies> GenerateDragons(Random r, int count, string path)
      {
         var speciesDataJson = JObject.Parse(File.ReadAllText(path));
         var speciesData = speciesDataJson.ToObject<SpeciesData>();

         for (int i = 0; i < count; i++)
         {
            var bonuses = GenerateStatBonuses(r);
            var physiolgyTraits = GeneratePhysiolgyTraits(r, speciesData, bonuses);
            var magicalTraits = GenerateMagicTraits(r, speciesData, bonuses);
            var cosmeticTraits = GenerateCosmeticTraits(r, speciesData, bonuses, physiolgyTraits, magicalTraits);
            var breathWeapon = GenerateBreathWeapon(r, speciesData, bonuses, physiolgyTraits, magicalTraits, cosmeticTraits);

            var species = new DragonSpecies()
            {
               Name = "",

               AwarenessBonus = bonuses.AwarenessBonus,
               DeterminationBonus = bonuses.DeterminationBonus,
               FitnessBonus = bonuses.FitnessBonus,
               GraceBonus = bonuses.GraceBonus,
               GuileBonus = bonuses.GuileBonus,
               PrecisionBonus = bonuses.PrecisionBonus,

               BreathWeapon = breathWeapon,

               Size = physiolgyTraits.Size,
               Speed = physiolgyTraits.Speed,
               Claws = physiolgyTraits.Claws,
               Teeth = physiolgyTraits.Teeth,
               Limbs = physiolgyTraits.Limbs,
               Wings = physiolgyTraits.Wings,
               Tail = physiolgyTraits.Tail,
               Sight = physiolgyTraits.Sight,
               Hearing = physiolgyTraits.Hearing,
               Scent = physiolgyTraits.Scent,

               MagicDraw = magicalTraits.MagicDraw,
               MagicGeneration = magicalTraits.MagicGeneration,
               MagicFlow = magicalTraits.MagicFlow,
               MagicManipulator = magicalTraits.MagicManipulator,

               Environment = cosmeticTraits.Environment,
               FavoredPrey = cosmeticTraits.FavoredPrey,
               HeadDecorations = cosmeticTraits.HeadDecorations,
               ScalePattern = cosmeticTraits.ScalePattern,

            };

            yield return species;
         }
      }

      private static PhysiolgyTraits GeneratePhysiolgyTraits(Random r, SpeciesData speciesData, StatBonuses bonuses)
      {
         return new PhysiolgyTraits()
         {
            Claws = GenerateClaws(r, speciesData, bonuses),
            Hearing = GenerateHearing(r, speciesData, bonuses),
            Limbs = GenerateLimbs(r, speciesData, bonuses),
            Scent = GenerateScent(r, speciesData, bonuses),
            Sight = GenerateSight(r, speciesData, bonuses),
            Size = GenerateSize(r, speciesData, bonuses),
            Speed = GenerateSpeed(r, speciesData, bonuses),
            Tail = GenerateTail(r, speciesData, bonuses),
            Teeth = GenerateTeeth(r, speciesData, bonuses),
            Wings = GenerateWings(r, speciesData, bonuses),
         };
      }

      private static ValueTrait GenerateClaws(Random r, SpeciesData speciesData, StatBonuses bonuses)
      {
         var item = speciesData.Claws.Draw(r, i => i.Weight);
         var power = item.Power.RandomRange(r);

         return !string.IsNullOrEmpty(item.Note) && item.Note != "No special" ?
            MakeTrait("Claws", $"{item.Type} claws", power, MakeTrait("Note", item.Note)) :
            MakeTrait("Claws", $"{item.Type} claws", power);
      }

      private static Trait GenerateHearing(Random r, SpeciesData speciesData, StatBonuses bonuses)
      {
         var item = speciesData.Hearing.Draw(r, i => i.Weight);

         return MakeTrait("Hearing", item.Type);
      }

      private static Trait GenerateLimbs(Random r, SpeciesData speciesData, StatBonuses bonuses)
      {
         var limbs = new[] { Tuple(4, 10), Tuple(2, 3) };
         var item = limbs.Draw(r, i => i.Item2);

         return MakeTrait("Limbs", $"{item.Item1} limbs");
      }

      private static Trait GenerateScent(Random r, SpeciesData speciesData, StatBonuses bonuses)
      {
         var item = speciesData.Scent.Draw(r, i => i.Weight);

         return MakeTrait("Scent", item.Type);
      }

      private static Trait GenerateSight(Random r, SpeciesData speciesData, StatBonuses bonuses)
      {
         var item = speciesData.Sight.Draw(r, i => i.Weight);

         return MakeTrait("Sight", item.Type);
      }

      private static ValueTrait GenerateSize(Random r, SpeciesData speciesData, StatBonuses bonuses)
      {
         var item = speciesData.Size.Draw(r, i => i.Weight);

         return MakeTrait("Size", item.Label, item.Value);
      }

      private static int GenerateSpeed(Random r, SpeciesData speciesData, StatBonuses bonuses)
      {
         var speeds = new[] { Tuple(40, 10), Tuple(50, 3), Tuple(30, 1), Tuple(60, 1) };
         var item = speeds.Draw(r, i => i.Item2);

         return item.Item1;
      }

      private static Trait GenerateTail(Random r, SpeciesData speciesData, StatBonuses bonuses)
      {
         var item = speciesData.Tail.Draw(r, i => i.Weight);

         return MakeTrait("Tail", item.Type);
      }

      private static ValueTrait GenerateTeeth(Random r, SpeciesData speciesData, StatBonuses bonuses)
      {
         var item = speciesData.Teeth.Draw(r, i => i.Weight);
         var power = item.Power.RandomRange(r);

         var desc = item.Type == "fangs" || item.Type == "tusks" ? item.Type : item.Type + " teeth";

         return !string.IsNullOrEmpty(item.Note) ?
            MakeTrait("Teeth", desc, power, MakeTrait("Note", item.Note)) :
            MakeTrait("Teeth", desc, power);
      }

      private static Trait GenerateWings(Random r, SpeciesData speciesData, StatBonuses bonuses)
      {
         var item = speciesData.Wings.Draw(r, i => i.Weight);

         return MakeTrait("Wings", item.Type);
      }

      private static MagicTraits GenerateMagicTraits(Random r, SpeciesData speciesData, StatBonuses bonuses)
      {
         var range = fun<int, int, int, Tuple<Range, int>>((min, max, w) => Tuple(Range(min, max), w));

         var draw = new[] { range(1, 22, 1), range(23, 34, 3), range(35, 42, 5), range(43, 51, 1) }.Draw(r, i => i.Item2).Item1;
         var flow = new[] { range(4, 25, 1), range(26, 37, 3), range(38, 55, 5), range(56, 66, 1) }.Draw(r, i => i.Item2).Item1;
         var gene = new[] { range(1, 11, 1), range(12, 16, 5), range(17, 31, 3), range(32, 44, 1) }.Draw(r, i => i.Item2).Item1;
         var mani = new[] { range(1, 7, 1), range(8, 17, 3), range(18, 27, 5), range(28, 36, 1) }.Draw(r, i => i.Item2).Item1;

         return new MagicTraits()
         {
            MagicDraw = MakeTrait("Magic draw", "", draw.RandomRange(r)),
            MagicFlow = MakeTrait("Magic flow", "", flow.RandomRange(r)),
            MagicGeneration = MakeTrait("Magic generation", "", gene.RandomRange(r)),
            MagicManipulator = MakeTrait("Magic manipulator", "", mani.RandomRange(r)),
         };
      }

      static string[] colors = new[] { "red", "green", "blue", "white", "black", "brown", "yellow", "purple", "orange", "golden", "silvery" };

      static CosmeticTraits GenerateCosmeticTraits(Random r, SpeciesData speciesData, StatBonuses bonuses, PhysiolgyTraits physiolgyTraits, MagicTraits magicalTraits)
      {
         var envi = new[] { Tuple("forests", 5), Tuple("plains", 4), Tuple("swamps", 2), Tuple("mountains", 1), Tuple("desert", 2) }.Draw(r, i => i.Item2).Item1;
         var prey = new[] { Tuple("deer", 6), Tuple("cattle", 5), Tuple("elves", 2), Tuple("fish", 3) }.Draw(r, i => i.Item2).Item1;
         var head = new[] { Tuple("two horns", 6), Tuple("four horns", 6), Tuple("many horns", 3), Tuple("horns and frill", 2) }.Draw(r, i => i.Item2).Item1;

         var pattern =
            new[] 
            {
               fun(() => $"solid {colors.Draw(r)} scales"),
               fun(() => $"{colors.Draw(r)} scales with {colors.Draw(r)} strips"),
               fun(() => $"{colors.Draw(r)} scales with {colors.Draw(r)} back bands"),
               fun(() => MakePresentable($"{colors.Draw(r)} scales with /a {colors.Draw(r)} belly")),
            }.Draw(r)();
         
         return new CosmeticTraits()
         {
            Environment = MakeTrait("Prefered Environment", envi),
            FavoredPrey = MakeTrait("Favored Prey", prey),
            HeadDecorations = MakeTrait("Head Decorations", head),
            ScalePattern = MakeTrait("Scale Pattern", pattern)
         };
      }

      static int RandomRange(this JToken minMax, Random r) => r.Next(minMax["min"].ToObject<int>(), minMax["max"].ToObject<int>());
      static int RandomRange(this Range minMax, Random r) => r.Next(minMax.Min, minMax.Max);
      static Range Range(int min, int max) => new Range() { Min = min, Max = max };

      public static Trait MakeTrait(string name, string description, params Trait[] subtraits) => new Trait() { Name = name, Desc = description, SubTraits = subtraits.ToList() };
      public static ValueTrait MakeTrait(string name, string description, int value, params Trait[] subtraits) => new ValueTrait() { Name = name, Desc = description, Value = value, SubTraits = subtraits.ToList() };

      static StatBonuses GenerateStatBonuses(Random r)
      {
         var majorBonus = new Range() { Min = 4, Max = 6 };
         var minorBonus = new Range() { Min = 1, Max = 3 };
         var minorPenalty = new Range() { Min = 1, Max = 2 };
         var awarenessRange = new Range() { Max = 4 };

         var major1 = majorBonus.RandomRange(r) + 1;
         var major2 = majorBonus.RandomRange(r);
         var minor1 = minorBonus.RandomRange(r);
         var minor2 = minorBonus.RandomRange(r);
         var middle = minorBonus.RandomRange(r) - 2;
         var penalty = -minorPenalty.RandomRange(r);

         var bonuses = new[] { major1, major2, minor1, minor2, middle, penalty }.InPlaceShuffle(r);

         return new StatBonuses()
         {
            AwarenessBonus = bonuses[0],
            DeterminationBonus = bonuses[1],
            FitnessBonus = bonuses[2],
            GraceBonus = bonuses[3],
            GuileBonus = bonuses[4],
            PrecisionBonus = bonuses[5],
         };
      }

      static BreathWeaponTemplate GenerateBreathWeapon(Random r, SpeciesData speciesData, StatBonuses bonuses, PhysiolgyTraits physiolgyTraits, MagicTraits magicalTraits, CosmeticTraits cosmeticTraits)
      {
         var material = speciesData.BreathWeaponMaterial.Draw(r);
         var shapeDesc = speciesData.BreathWeaponShapes.Draw(r);
         var shape = shapeDesc.Shape;
         var range = shapeDesc.Range.RandomRange(r);
         var radius = shapeDesc?.Radius?.RandomRange(r);
         var power = r.Next(4, 7);
         
         var name = $"{material} {shape}";
         var description = MakePresentable($@"/A {shape} of {material} that goes as far as {range} dragon lengths for {power} damage");

         return new BreathWeaponTemplate()
         {
            Material = material,
            Shape = shape,
            Range = range,
            Radius = radius,
            Power = power,
            Name = name,
            Desc = description,
         };
      }
   }

   public class SpeciesData
   {
      public string[] BreathWeaponMaterial { get; set; }
      public BreathweaponShape[] BreathWeaponShapes { get; set; }
      public Claws[] Claws { get; set; }
      public Teeth[] Teeth { get; set; }
      public Wings[] Wings { get; set; }
      public Tail[] Tail { get; set; }
      public Sight[] Sight { get; set; }
      public Hearing[] Hearing { get; set; }
      public Scent[] Scent { get; set; }
      public Size[] Size { get; set; }
   }

   public class BreathweaponShape
   {
      public string Shape { get; set; }
      public Range Range { get; set; }
      public Range power { get; set; }
      public Range Radius { get; set; }
   }

   public class Claws
   {
      public string Type { get; set; }
      public int Weight { get; set; }
      public string Note { get; set; }
      public Range Power { get; set; }
   }

   public class Teeth
   {
      public string Type { get; set; }
      public int Weight { get; set; }
      public string Note { get; set; }
      public Range Power { get; set; }
   }

   public class Wings
   {
      public string Type { get; set; }
      public int Weight { get; set; }
   }

   public class Tail
   {
      public string Type { get; set; }
      public int Weight { get; set; }
   }

   public class Sight
   {
      public string Type { get; set; }
      public int Weight { get; set; }
   }

   public class Hearing
   {
      public string Type { get; set; }
      public int Weight { get; set; }
   }

   public class Scent
   {
      public string Type { get; set; }
      public int Weight { get; set; }
   }

   public class Size
   {
      public string Label { get; set; }
      public int Value { get; set; }
      public int Weight { get; set; }
   }

}
