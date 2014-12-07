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
      CombatantComponent target;

      public AttackEntityAction(CombatantComponent target)
      {
         Contract.Requires(target != null);
         this.target = target;
      }

      public override void Do(Entity executer)
      {
         var me = executer.GetComponent<CombatantComponent>();

         var damage = me.Power - target.Defense;

         if (damage > 0)
         {
            MessageService.Current.PostMessage(executer.Name + " attacks " + target.Owner.Name + " for " + damage + " hit points.");
            target.TakeDamage(damage, executer);
         }
         else
         {
            MessageService.Current.PostMessage(executer.Name + " attacks " + target.Owner.Name + " but it has no effect");
         }
      }
   }
}
