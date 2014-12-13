using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using System.Diagnostics;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using System.Diagnostics.Contracts;

namespace DraconicEngine.GameWorld.Actions
{
   public class AttackEntityAction : RogueAction
   {
      public Entity Target { get; set; }

      public AttackEntityAction(Entity target)
      {
         Contract.Requires(target != null);
         this.Target = target;
      }

      public override void Do(Entity executer)
      {
         var attack = new Attack()
         {
            Attacker = executer,
            Target = Target,
            Weapon = null,
         };

         var resolver = Target.GetActionResolver<IAttackResolver>();
      }
   }

   public class Attack
   {
      public Entity Target { get; set; }
      public Entity Attacker { get; set; }
      public Entity Weapon { get; set; }
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
