using DraconicEngine;
using DraconicEngine.GameWorld;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DragonRising.GameWorld.Components;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Actions
{
   public class AttackEntityAction : RogueAction
   {
      public Entity Target { get; set; }
      public Option<Entity> Weapon { get; set; }

      public AttackEntityAction(Entity target, Option<Entity> weapon)
      {
         Contract.Requires(target != null);
         Contract.Requires(weapon.ForAll(w => w.HasComponent<ItemComponent>() && w.GetComponent<ItemComponent>().WeaponUse != null));
         this.Target = target;
         this.Weapon = weapon;

         
      }

      public override void Do(Entity executer)
      {
         Debug.Assert(Weapon.Match(
            Some: w => executer.DistanceTo(Target).KingLength <= w.GetComponent<ItemComponent>().WeaponUse.Range,
            None: () => executer.IsAdjacent(Target)), "Target is not within attacker's range");

         var attack = new Attack()
         {
            Attacker = executer,
            Target = Target,
            Weapon = Weapon,
         };

         var resolver = Target.GetActionResolver<IAttackResolver>();

         resolver.Resolve(attack);
      }
   }

   public class Attack
   {
      public Entity Target { get; set; }
      public Entity Attacker { get; set; }
      public Option<Entity> Weapon { get; set; }
   }

   public class AttackResult
   {
      public bool Negated { get; set; }
      public int InitialDamage { get; set; }
      public int DamageDealt { get; set; }
   }

   public interface IAttackResolver : IActionResolver
   {
      AttackResult Resolve(Attack attack);
   }

   [DefaultResolverAttribute(typeof(Attack))]
   public class DefaultAttackResolver : IAttackResolver
   {
      public Type ActionType => typeof(Attack);
      public Type ResultType => typeof(AttackResult);
      public AttackResult Resolve(Attack attack)
      {
         var me = attack.Attacker.GetComponent<CombatantComponent>();
         var them = attack.Target.GetComponent<CombatantComponent>();

         var damage = me.Power - them.Defense;

         if (damage > 0)
         {
            MessageService.Current.PostMessage(attack.Attacker.Name + " attacks " + attack.Target.Name + " for " + damage + " hit points.");
            them.TakeDamage(damage, attack.Attacker);
         }
         else
         {
            MessageService.Current.PostMessage(attack.Attacker.Name + " attacks " + attack.Target.Name + " but it has no effect");
         }

         return new AttackResult()
         {
            InitialDamage = me.Power,
            DamageDealt = damage,
            Negated = damage <= 0
         };
      }
   }
}
