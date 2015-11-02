using DraconicEngine;
using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Events.SensoryEvents
{
   public class SensoryEvent : Fact
   {
      public SensoryEvent(Loc location, params Sensed[] effects)
      {
         this.Location = location;
         this.Senses = effects.ToImmutableList();
      }

      public ImmutableList<Sensed> Senses { get; }
      public Loc Location { get; private set; }
   }

   public class Sensed
   {
      public Sense Sense { get; }
      public string Type { get; }
      public string Quality { get; }
      public int Strength { get; }

      public Sensed(Sense sense, string type, string quality, int strength)
      {
         Sense = sense;
         Type = type;
         Quality = quality;
         Strength = strength;
      }
   }

   public enum Sense
   {
      Sight,
      Sound,
      Touch,
      Smell,
      Taste,
      Feeling
   }
}
