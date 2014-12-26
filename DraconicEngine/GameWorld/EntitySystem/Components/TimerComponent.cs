using DraconicEngine.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   public class TimerComponent : Component
   {
      public TimerComponent()
      {

      }

      protected TimerComponent(TimerComponent original, bool fresh)
         : base(original, fresh)
      {
         // Never copy timers, even when not asking for a fresh clone

         // This might be an indication that entities should not be the containers for timers
      }
      public List<TurnTimer> Timers { get; } = new List<TurnTimer>();

      public void AttachTimer(TurnTimer timer) => this.Timers.Add(timer);

      protected override Component CloneCore(bool fresh)
      {
         return new TimerComponent(this, fresh);
      }
   }
}
