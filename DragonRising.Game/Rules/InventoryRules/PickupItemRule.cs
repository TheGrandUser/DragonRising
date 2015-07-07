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
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld;

namespace DragonRising.Rules.InventoryRules
{
   public class PickupItemRule : Rule<PickUpItemAction>
   {
      public override RuleResult Do(PickUpItemAction action)
      {
         var inventory = action.Picker.GetComponent<InventoryComponent>();
         var scene = World.Current.Scene;

         var itemComponent = action.ItemToPick.GetComponent<ItemComponent>();

         if (inventory.TryPickUp(action.ItemToPick))
         {
            scene.EntityStore.RemoveEntity(action.ItemToPick);
         }

         return RuleResult.Empty;
      }
   }
}
