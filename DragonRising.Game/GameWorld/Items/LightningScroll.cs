using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Items;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.Actions.Requirements;
using LanguageExt;
using LanguageExt.Prelude;

namespace DragonRising.Entities.Items
{
   public class LightningScroll : Component, IItemUsage
   {
      int damage;
      int range;
      private Entity target;
      public LightningScroll(int damage = 20, int range = 5)
      {
         this.damage = damage;
         this.range = range;
      }

      public void Use(Entity user)
      {
         MessageService.Current.PostMessage("A lightning bolt strikes the " + this.target.Name + " with a loud thunder! The damage is "
            + damage + " hit points.", RogueColors.LightBlue);
         this.target.GetComponent<CombatantComponent>().TakeDamage(this.damage, user);
         this.target = null;
      }

      public ItemUseResult PrepUse(Entity user, Some<RequirementFulfillment> fulfillment)
      {
         if(fulfillment.Value is EntityFulfillment)
         {
            var entityFulfillment = (EntityFulfillment)fulfillment;
            this.target = entityFulfillment.Entity;
            return ItemUseResult.Destroyed;
         }
         else
         {
            return ItemUseResult.NotUsed;
         }
      }

      public IItemUsageTemplate Template
      {
         get { throw new NotImplementedException(); }
      }

      public ActionRequirement Requirements => new EntityRequirement(range, typeof(CreatureComponent), typeof(CombatantComponent));
   }
}