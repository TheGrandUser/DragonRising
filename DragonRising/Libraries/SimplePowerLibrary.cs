using DragonRising.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.GameWorld.Powers;
using DragonRising.Plans;
using DragonRising.GameWorld.Powers.Spells;

namespace DragonRising.Libraries
{
   class SimplePowerLibrary : IPowerLibrary
   {
      Dictionary<string, EffectPlan> usages = new Dictionary<string, EffectPlan>(StringComparer.InvariantCultureIgnoreCase);

      public void Add(EffectPlan power) => usages.Add(power.Name, power);

      public bool Contains(string name) => usages.ContainsKey(name);

      public EffectPlan Get(string name) => usages[name];
   }

   class SimpleSpellLibrary : ISpellLibrary
   {
      Dictionary<string, Spell> spells = new Dictionary<string, Spell>(StringComparer.InvariantCultureIgnoreCase);

      public SimpleSpellLibrary()
      {
         Add(new Spell("Fireball", new FireballPower()));
         Add(new Spell("Lightning Jolt", new LightningPower()));
         Add(new Spell("Cure Minor Wounds", new CureMinorWoundsPower()));
         Add(new Spell("Confusion", new ConfuseNearestPower()));
      }

      public void Add(Spell spell)
      {
         spells.Add(spell.Name, spell);
      }

      public bool Contains(string name) => spells.ContainsKey(name);

      public Spell Get(string name) => spells[name];
   }
}
