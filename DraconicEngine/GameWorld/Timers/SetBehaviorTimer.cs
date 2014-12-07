using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Timers
{
   public class PopBehaviorTimer : TurnTimer
   {
      Entity target;

      public PopBehaviorTimer(int duration, Entity target)
         : base(duration)
      {
         this.target = target;
      }

      protected override void Trigger()
      {
         target.As<BehaviorComponent>(controler => controler.PopBehavior());
      }
   }
}
