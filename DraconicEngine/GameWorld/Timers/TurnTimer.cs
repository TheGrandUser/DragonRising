using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Timers
{
   [Serializable]
   public abstract class TurnTimer
   {
      int duration;

      public TurnTimer(int duration)
      {
         this.duration = duration;
      }

      protected TurnTimer(TurnTimer original)
      {
         this.duration = original.duration;
      }

      public void Tick()
      {
         --duration;
         if (duration == 0)
         {
            Trigger();
         }
      }
      public int Duration { get { return duration; } }

      public TurnTimer Clone()
      {
         return CloneCore();
      }

      protected abstract TurnTimer CloneCore();
      protected abstract void Trigger();
   }
}
