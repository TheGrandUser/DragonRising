using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DragonRising.GameWorld.Items;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.Actions.Requirements;
using LanguageExt;
using LanguageExt.Prelude;
using DragonRising.GameWorld.Components;

namespace DragonRising.GameWorld.Items
{
   public class LightningScroll : IItemUsage
   {
      int damage;
      int range;
      public LightningScroll(int damage = 20, int range = 5)
      {
         this.damage = damage;
         this.range = range;
      }

      public bool Use(Entity user, Some<RequirementFulfillment> fulfillment)
      {
         if (fulfillment.Value is EntityFulfillment)
         {
            var entityFulfillment = (EntityFulfillment)fulfillment;
            var target = entityFulfillment.Entity;
            MessageService.Current.PostMessage("A lightning bolt strikes the " + target.Name + " with a loud thunder! The damage is "
            + damage + " hit points.", RogueColors.LightBlue);
            target.GetComponent<CombatantComponent>().TakeDamage(this.damage, user);
            target = null;

            return true;
         }
         else
         {
            return false;
         }
      }

      public ActionRequirement Requirements => new EntityRequirement(range, typeof(CreatureComponent), typeof(CombatantComponent));
   }
}