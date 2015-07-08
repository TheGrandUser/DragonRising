using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using DraconicEngine.RulesSystem;
using DraconicEngine;
using DragonRising.GameWorld.Events.SensoryEvents;

namespace DragonRising.Rules.ExplorationRules
{
   class ReportSensoryEffectRule : Rule<SensoryEvent>
   {
      public override RuleResult Do(SensoryEvent gameEvent)
      {
         throw new NotImplementedException();
      }
   }
}
