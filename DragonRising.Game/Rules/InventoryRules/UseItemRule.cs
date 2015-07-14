using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using DraconicEngine.EntitySystem;
using DragonRising.Commands.Requirements;
using DraconicEngine;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Components;
using DragonRising.Rules;
using DragonRising.GameWorld.Actions;

namespace DragonRising.Rules.InventoryRules
{
   public class UseItemRule : Rule<UseItemAction>
   {
      public override RuleResult Do(UseItemAction action)
      {
         var itemComponent = action.Item.GetComponent<ItemComponent>();
         var usable = itemComponent.Usable;
         
         var fact = action.ItemFact;
         
         if (usable.IsCharged)
         {
            usable.Charges--;

            if (usable.Charges <= 0)
            {
               action.User.As<InventoryComponent>(inventory => inventory.Items.Remove(action.Item));
            }
         }

         return fact;
      }
   }
}
