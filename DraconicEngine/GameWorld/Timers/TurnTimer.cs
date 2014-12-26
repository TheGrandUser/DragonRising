using DraconicEngine.GameWorld.Effects;
using DraconicEngine.GameWorld.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Timers
{
   [Serializable]
   public sealed class TurnTimer
   {
      int duration;
      List<IEffect> effects;
      Entity initiator;

      public TurnTimer(int duration, Entity initiator, IEffect effect, params IEffect[] additionalEffects)
      {
         this.duration = duration;
         this.effects = additionalEffects.StartWith(effect).ToList();
         this.initiator = initiator;
      }

      //protected TurnTimer(TurnTimer original)
      //{
      //   this.duration = original.duration;
      //   this.effects = original.effects.ToList();
      //   this.initiator = original.initiator;
      //}

      public void Tick()
      {
         --duration;
         if (duration == 0)
         {
            foreach (var effect in effects)
            {
               effect.Do(initiator);
            }
         }
      }
      public int Duration { get { return duration; } }

      //public TurnTimer Clone()
      //{
      //   return new TurnTimer(this);
      //}
   }
}
