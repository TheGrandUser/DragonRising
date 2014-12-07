using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Nodes
{
   public class TimerNode : Node
   {
      public TimerComponent Timer { get; set; }

      public override void ClearComponents()
      {
         this.Timer = null;
      }

      public override void SetComponents(Entity entity)
      {
         this.Timer = entity.GetComponent<TimerComponent>();
      }
   }
}
