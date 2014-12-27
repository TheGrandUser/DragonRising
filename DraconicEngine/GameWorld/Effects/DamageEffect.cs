using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Powers.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Effects
{
   public class DamageEffect : IEffect
   {
      Entity initiator;

      public DamageEffect(Entity initiator)
      {
         this.initiator = initiator;
      }

      CreatureNodeInput targetInput = new CreatureNodeInput();
      public CreatureNodeInput TargetInput { get { return targetInput; } }
      
      NumberNodeInput damageInput = new NumberNodeInput();
      public NumberNodeInput DamageInput { get { return damageInput; } }

      public void Do()
      {
         var targets = this.targetInput.Value;
         foreach (var target in targets)
         {
            target.GetComponent<CombatantComponent>().TakeDamage(this.damageInput.Value, initiator);
         }
      }
   }
}