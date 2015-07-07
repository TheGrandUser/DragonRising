using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using LanguageExt;
using static LanguageExt.Prelude;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Events;

namespace DragonRising.Rules.CombatRules
{
   class EntityKilledRule : Rule<CreatureKilledEvent>
   {
      public override RuleResult Do(CreatureKilledEvent gameEvent)
      {
         return RuleResult.Empty;
      }
   }
}
