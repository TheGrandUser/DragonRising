using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Timers
{
   public abstract class TurnTimer
   {
      int duration;

      public TurnTimer(int duration)
      {
         this.duration = duration;
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

      protected abstract void Trigger();
   }
}
