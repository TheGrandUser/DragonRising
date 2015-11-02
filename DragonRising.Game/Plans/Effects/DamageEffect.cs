using DraconicEngine;
using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using DragonRising.Commands.Requirements;
using DragonRising.GameWorld.Alligences;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Events;
using DragonRising.Rules.CombatRules;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Plans.Effects
{
   public class DamageEffect : IEntityEffect
   {
      public DamageEffect(Damage damage)
      {
         Damage = damage;
      }

      public Damage Damage { get; }
      
      public IEnumerable<Fact> GetFacts(Entity initiator, Entity target, Scene scene)
      {
         yield return new InflictDamageEvent(target, Damage, initiator);
      }
   }
}
