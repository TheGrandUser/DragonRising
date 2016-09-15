using DraconicEngine.RulesSystem;
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
using DraconicEngine;

namespace DragonRising.GameWorld.Effects
{
   public class ApplyTemporaryCondition : IEntityEffect
   {
      readonly ICondition condition;
      readonly int duration;

      public ApplyTemporaryCondition(ICondition condition, int duration)
      {
         this.condition = condition;
         this.duration = duration;
      }

      public IEnumerable<Fact> GetFacts(Entity initiator, Entity target, Scene scene)
      {
         yield return new AddConditionEvent(target, condition, duration);
      }
   }
}
