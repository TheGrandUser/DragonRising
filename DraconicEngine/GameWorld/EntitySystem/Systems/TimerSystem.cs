using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.EntitySystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Systems
{
   public class TimerSystem : ListIteratingSystemSync<TimerNode>
   {
      List<TimerNode> toRemove = new List<TimerNode>();

      protected override void NodeUpdateFunction(TimerNode node, double time)
      {
         foreach(var timer in node.Timer.Timers)
         {
            timer.Tick();
         }

         node.Timer.Timers.RemoveAll(timer => timer.Duration == 0);

         if(node.Timer.Timers.Count == 0)
         {
            toRemove.Add(node);
         }
      }

      public override void Update(double time)
      {
         base.Update(time);

         foreach(var node in toRemove)
         {
            node.Entity.RemoveComponent<TimerComponent>();
         }
         toRemove.Clear();
      }
   }
}
