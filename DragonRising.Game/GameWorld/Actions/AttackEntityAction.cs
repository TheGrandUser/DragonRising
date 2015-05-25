using DraconicEngine;
using DraconicEngine.GameWorld;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.Effects;
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
      public Entity Attacker { get; set; }
      public Entity Target { get; set; }
      public Option<Entity> Weapon { get; set; }

      public AttackEntityAction(Entity attacker, Entity target, Option<Entity> weapon)
      {
         Contract.Requires(target != null);
         Contract.Requires(weapon.ForAll(w => w.HasComponent<ItemComponent>() && w.GetComponent<ItemComponent>().WeaponUse != null));
         Attacker = attacker;
         Target = target;
         Weapon = weapon;
      }
   }
   
   public class AttackResult: IEffect
   {
      public bool Negated { get; set; }
      public int InitialDamage { get; set; }
      public int DamageDealt { get; set; }

      public void Do()
      {

      }
   }

   public class BaseAttackRule : IActionRule<AttackEntityAction, AttackResult>
   {
      public AttackResult Apply(AttackEntityAction attack)
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
