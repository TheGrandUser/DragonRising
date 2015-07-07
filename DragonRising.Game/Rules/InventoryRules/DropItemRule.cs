using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Actions;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld;
using DragonRising.GameWorld.Components;

namespace DragonRising.Rules.InventoryRules
{
   class DropItemRule : Rule<DropItemAction>
   {
      public override RuleResult Do(DropItemAction action)
      {
         var scene = World.Current.Scene;

         action.Dropper.GetComponent<InventoryComponent>().Items.Remove(action.Item);
         action.Item.Location = action.Dropper.Location;
         scene.EntityStore.AddEntity(action.Item);

         return RuleResult.Empty;
      }
   }
}
