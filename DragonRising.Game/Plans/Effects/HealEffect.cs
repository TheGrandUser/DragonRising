﻿using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DragonRising.Commands.Requirements;
using System.Collections.Immutable;
using DragonRising.GameWorld.Components;
using DragonRising.Facts.Events;
using DraconicEngine;

namespace DragonRising.Plans.Effects
{
   public class HealEffect : IEntityEffect
   {
      int amount;

      public HealEffect(int amount)
      {
         this.amount = amount;
      }
      
      public IEnumerable<Fact> GetFacts(Entity initiator, Entity target, Scene scene)
      {
         yield return new CreatureHealedEvent(initiator, target, amount);
      }
   }
}
