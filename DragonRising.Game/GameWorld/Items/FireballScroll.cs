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

namespace DragonRising.Items
{
   class FireballScroll : IItemUsage
   {
      int damage;
      int range;

      public FireballScroll(int damage = 12, int radius = 3)
      {
         this.damage = damage;
         this.range = radius;
      }

      public bool Use(Entity user, Some<RequirementFulfillment> fulfillment)
      {
         if (fulfillment.Value is LocationFulfillment)
         {
            var locationFulfillment = (LocationFulfillment)fulfillment;
            var target = locationFulfillment.Location;
            var rangeSquared = range * range;

            var entitiesToDamage = Scene.CurrentScene.EntityStore.AllCreaturesSpecialFirst
               .Where(entity => entity.HasComponent<CombatantComponent>() && (entity.Location - target).LengthSquared <= rangeSquared)
               .ToList();

            foreach (var entity in entitiesToDamage)
            {
               MessageService.Current.PostMessage(string.Format("The {0} gets burned for {1} hit points", entity.Name, damage), RogueColors.Orange);
               entity.GetComponent<CombatantComponent>().TakeDamage(damage, user);
            }
            return true;
         }
         else
         {
            return false;
         }
      }

      public ActionRequirement Requirements => new LocationRequirement(isLimitedToFoV: true);
   }
}
