﻿using DragonRising.Facts.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.RulesSystem;

namespace DragonRising.Rules.MagicRules
{
   class UseASpellRule : Rule<CastSpellFact>
   {
      public override RuleResult Do(CastSpellFact gameEvent)
      {
         // any special rules about spells, here
         return gameEvent.Plan.GetFacts(gameEvent.User);
      }
   }
}