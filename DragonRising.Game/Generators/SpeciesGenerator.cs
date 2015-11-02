using DraconicEngine.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DraconicEngine.Utilities.StringPresentationFormatter;

namespace DragonRising.Generators
{
   public class Species
   {
      public string Name { get; set; }
      public int Size { get; set; }
      public int Speed { get; set; }

      public int AwarenessBonus { get; set; }
      public int DeterminationBonus { get; set; }
      public int FitnessBonus { get; set; }
      public int GraceBonus { get; set; }
      public int GuileBonus { get; set; }
      public int PrecisionBonus { get; set; }

      public ImmutableList<Trait> Traits { get; set; }
   }

   public class Trait
   {
      public string Name { get; set; }
      public string Description { get; set; }
   }

   public class DragonSpecies : Species
   {
      public Trait BreathWeapon { get; set; }
      public Trait Claws { get; set; }
      public Trait Teeth { get; set; }
      public Trait Limbs { get; set; }
      public Trait Wings { get; set; }
      public Trait Tail { get; set; }
      public Trait Sight { get; set; }
      public Trait Hearing { get; set; }
      public Trait Scent { get; set; }

      public Trait MagicFlow { get; set; }
      public Trait MagicGeneration { get; set; }
      public Trait MagicDraw { get; set; }
      public Trait MagicManipulator { get; set; }

      public Trait ScalePattern { get; set; }
      public Trait HeadDecorations { get; set; }
      public Trait Environment { get; set; }
      public Trait FavoredPRey { get; set; }
   }

   public static class SpeciesGenerator
   {
      public static IEnumerable<DragonSpecies> GenerateDragons(Random r)
      {
         var breathWeapon = GenerateBreathWeapon(r);

         yield break;
      }

      public static Trait MakeTrait(string name, string description) => new Trait() { Name = name, Description = description };

      #region Breath Weapon

      static ImmutableList<string> BreathWeaponMaterial = ImmutableList.Create(
         "fire",
         "frost",
         "lightning",
         "ice",
         "light");

      static ImmutableList<JObject> BreathWeaponShapes = ImmutableList.Create(
         new JObject() {["shape"] = "line",["range"] = new JObject() {["min"] = 4,["max"] = 8 } },
         new JObject() {["shape"] = "cone",["range"] = new JObject() {["min"] = 2,["max"] = 5 } },
         new JObject() {["shape"] = "ball",["range"] = new JObject() {["min"] = 3,["max"] = 6 },["radius"] = new JObject() {["min"] = 1,["max"] = 3 } });

      static int RandomRange(this JToken minMax, Random r)
      {
         var min = minMax["min"].ToObject<int>();
         var max = minMax["min"].ToObject<int>();
         var range = r.Next(min, max);

         return range;
      }

      static JObject GenerateBreathWeapon(Random r)
      {
         var material = BreathWeaponMaterial.Draw(r);
         var shapeDesc = BreathWeaponShapes.Draw(r);
         var shape = shapeDesc["shape"].ToString();
         var range = shapeDesc["range"].RandomRange(r);
         var radius = shapeDesc["radius"]?.RandomRange(r);
         var power = r.Next(4, 7);

         var name = $"{material} {shape}";
         var description = MakePresentable($@"\A {shape} of {material} that goes as far as {range} dragon lengths for {power} damage");

         return new JObject()
         {
            ["type"] = "BreathWeapon",
            ["material"] = material,
            ["shape"] = shape,
            ["range"] = range,
            ["radius"] = radius,
            ["power"] = power,
            ["name"] = name,
            ["description"] = description,
         };
      }

      #endregion

   }
}
