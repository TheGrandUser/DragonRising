using DragonRising.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.GameWorld.Powers;
using DragonRising.Plans;
using DragonRising.GameWorld.Powers.Spells;
using DragonRising.Plans.Queries;
using DragonRising.GameWorld.Effects;
using DragonRising.GameWorld.Conditions;
using DragonRising.Plans.EntityFilters;
using DragonRising.Plans.Targeters;
using DraconicEngine.RulesSystem;
using DragonRising.Plans.Effects;
using DragonRising.GameWorld.Events.SensoryEvents;
using DragonRising.GameWorld.Events;
using DraconicEngine;

namespace DragonRising.Libraries
{
   class SimpleSpellLibrary : ISpellLibrary
   {
      Dictionary<string, Spell> spells = new Dictionary<string, Spell>(StringComparer.InvariantCultureIgnoreCase);

      public SimpleSpellLibrary()
      {
         Add(new Spell("Fireball", EffectPlan.CreatePower("Fireball").Add(LocationInRangeTargeter.Build(new SelectionRange(null, RangeLimits.LineOfEffect))
               .Add(new AffectAllInRangeQuery(3,
                  OnlyCreaturesFilter.Instance,
                  new DamageEffect(new Damage(12, "Fire"))))
               .Add(new SensoryEffect("A great ball of fire bursts.",
                  new Sensed(Sense.Sound, "Explosion", "Fiery", 12),
                  new Sensed(Sense.Sight, "Flame", "Bright", 12))).Finish()).Finish()));

         Add(new Spell("Lightning Jolt", EffectPlan.CreatePower("Lightning bolt").Add(new EntityInRangeTargeter(
            new SelectionRange(5, RangeLimits.LineOfSight | RangeLimits.LineOfEffect),
            "A lightning bolt strikes the {target} with a loud thunder!", RogueColors.LightBlue,
            Enumerable.Empty<IFromLocationTargeter>(),
            Enumerable.Empty<IFromLocationQuery>(),
            EnumerableEx.Return(new DamageEffect(new Damage(20, "Lightning"))))).Finish()));

         Add(new Spell("Cure Minor Wounds", EffectPlan.CreatePower("Cure Minor Wounds").Add(new HealEffect(4)).Finish()));

         Add(new Spell("Confusion", EffectPlan.CreatePower("Confuse")
            .Add(new SelectClosestCreatureQuery(8, new ApplyTemporaryCondition(ConfusedCondition.Instance, 10), OnlyEnemiesFilter.Instance))
            .Finish()));
      }

      public void Add(Spell spell)
      {
         spells.Add(spell.Name, spell);
      }

      public bool Contains(string name) => spells.ContainsKey(name);

      public Spell Get(string name) => spells[name];
   }
}
