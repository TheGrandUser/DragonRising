using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DragonRising.Commands.Requirements;
using System.Collections.Immutable;
using DragonRising.GameWorld.Events.SensoryEvents;
using DraconicEngine;

namespace DragonRising.Plans.Effects
{
   class SensoryEffect : ILocationEffect
   {
      private Sensed[] sensedThings;

      public SensoryEffect(params Sensed[] sensedThings)
      {
         this.sensedThings = sensedThings;
      }

      public int Loudness { get; }

      public IEnumerable<Fact> GetFacts(Entity initiator, Loc target)
      {
         yield return new SensoryEvent(target, this.sensedThings);
      }
   }
}
