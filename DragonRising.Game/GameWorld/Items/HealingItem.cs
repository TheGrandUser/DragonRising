using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.Actions.Requirements;
using LanguageExt;
using LanguageExt.Prelude;

namespace DragonRising.Entities.Items
{
   public class HealingItem : IItemUsage
   {
      public int HealAmount { get; set; } = 4;

      public HealingItem() { }

      public bool Use(Entity user, Some<RequirementFulfillment> fulfillment)
      {
         var combatantComponent = user.GetComponentOrDefault<CombatantComponent>();
         if (combatantComponent != null)
         {
            if (combatantComponent.HP < combatantComponent.MaxHP)
            {
               combatantComponent.Heal(this.HealAmount);
               return true;
            }
         }
         return false;
      }

      public ActionRequirement Requirements => NoRequirement.None;
   }
}