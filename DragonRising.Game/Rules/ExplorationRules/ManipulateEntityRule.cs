using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld.Components;
using DraconicEngine.EntitySystem;

namespace DragonRising.Rules.ExplorationRules
{
   class ManipulateEntityRule : Rule<ManipulateEntityAction>
   {
      public override RuleResult Do(ManipulateEntityAction action, Scene scene)
      {
         var m = action.Target.Value.GetComponent<ManipulatableComponent>();
         if (m.Use(action.ItemToUse))
         {
            // success!
         }
         else
         {
            // failed!
         }

         return RuleResult.Empty;
      }
   }
}
