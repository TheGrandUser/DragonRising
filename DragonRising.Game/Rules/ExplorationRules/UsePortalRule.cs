using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.EntitySystem;
using DragonRising.Rules;
using DragonRising.GameWorld.Actions;

namespace DragonRising.Rules.ExplorationRules
{
   public class UsePortalRule : Rule<UsePortalAction>
   {
      public override RuleResult Do(UsePortalAction action)
      {
         return RuleResult.Empty;
      }
   }
}
