using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Actions;
using DragonRising.Rules.CombatRules;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Events
{
   public sealed class InflictDamageEvent : Fact
   {
      public InflictDamageEvent(Entity target, Damage damage, Entity initiator)
      {
         Initiator = initiator;
         Target = target;
         Damage = damage;
      }

      public Entity Initiator { get; }
      public Entity Target { get; }
      public Damage Damage { get; }

   }

   public sealed class AttackMissedEvent : Fact
   {
      public AttackEntityAction Attack { get; }

      public AttackMissedEvent(AttackEntityAction attack)
      {
         Attack = attack;
      }
   }

   public sealed class Damage
   {
      public Damage(int amount, string type = "Normal")
      {
         Amount = amount;
         Type = type;
      }

      public int Amount { get; }
      public string Type { get; }
   }
}
