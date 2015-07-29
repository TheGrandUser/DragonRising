﻿using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DragonRising.Commands.Requirements;
using LanguageExt;
using static LanguageExt.Prelude;
using DragonRising.GameWorld.Components;
using System.Collections.Immutable;
using DragonRising.GameWorld.Alligences;
using DragonRising.GameWorld.Events;
using DraconicEngine.Services;
using DragonRising.GameWorld.Conditions;

namespace DragonRising.GameWorld.Effects
{
   class ApplyTemporaryCondition : IEntityEffect
   {
      ICondition condition;
      int duration;

      public ApplyTemporaryCondition(ICondition condition, int duration)
      {
         this.condition = condition;
         this.duration = duration;
      }

      public IEnumerable<Fact> GetFacts(Entity initiator, Entity target)
      {
         yield return new AddConditionEvent(target, condition, duration);
      }
   }
}