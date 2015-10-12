using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld.Components;
using DraconicEngine;
using DraconicEngine.EntitySystem;
using System.Collections.Immutable;
using DragonRising.GameWorld.Events;

namespace DragonRising.Rules.CombatRules
{
   class AttackRule : Rule<AttackEntityAction>
   {
      public override RuleResult Do(AttackEntityAction action, Scene scene)
      {
         var me = action.Attacker.GetComponent<CombatantComponent>();
         var them = action.Target.GetComponent<CombatantComponent>();

         var roll = RogueGame.Current.GameRandom.Next(20) + 1;

         var hitBy = roll + me.Power - (10 + them.Defense);
         
         if (hitBy > 0)
         {
            MessageService.Current.PostMessage(action.Attacker.Name + " attacks " + action.Target.Name);// + " for " + damage + " hit points.");

            return action.Weapon.Match(
               Some: w =>
               {
                  var item = w.GetComponent<ItemComponent>();

                  var damage = me.Power + item.WeaponUse.Power;
                  var damageType = item.WeaponUse.DamageType;

                  return new InflictDamageEvent(action.Target, new Damage(damage, damageType), action.Attacker);
               },
               None: () => new InflictDamageEvent(
                  action.Target,
                  new Damage(me.Power), action.Attacker));

         }
         else
         {
            MessageService.Current.PostMessage(action.Attacker.Name + " attacks " + action.Target.Name + " but missed");

            return new AttackMissedEvent(action);
         }
      }
   }



}
