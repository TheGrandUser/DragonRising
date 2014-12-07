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
   public class HealingItem : Component, IItemUsage
   {
      public int HealAmount { get; set; }

      public HealingItem()
      {
         this.HealAmount = 4;
      }

      public void Use(Entity user)
      {
         user.GetComponent<CombatantComponent>().Heal(this.HealAmount);
      }

      public ItemUseResult PrepUse(Entity user, Some<RequirementFulfillment> fulfillment)
      {
         var combatantComponent = user.GetComponentOrDefault<CombatantComponent>();
         if (combatantComponent != null)
         {
            if (combatantComponent.HP < combatantComponent.MaxHP)
            {
               return ItemUseResult.Destroyed;
            }
         }

         return ItemUseResult.NotUsed;
      }

      public IItemUsageTemplate Template
      {
         get { throw new NotImplementedException(); }
      }

      public ActionRequirement Requirements => NoRequirement.None;
   }
}