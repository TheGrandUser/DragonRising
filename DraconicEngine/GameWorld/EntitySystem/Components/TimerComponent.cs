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
      public List<TurnTimer> Timers { get; } = new List<TurnTimer>();

      public void AttachTimer(TurnTimer timer) => this.Timers.Add(timer);
   }
}
