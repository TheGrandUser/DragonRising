using DraconicEngine.GameWorld.EntitySystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Systems
{
   public class CreatureActionSystem : ListIteratingSystemSync<CreatureActionNode>
   {
      Dictionary<Entity, Action> actionsToDo = new Dictionary<Entity, Action>();

      protected override void NodeUpdateFunction(CreatureActionNode node, double time)
      {
         var decision = node.Decision;

         decision.ActionToDo.Do(node.Entity);
      }

      public override void Update(double time)
      {
         base.Update(time);

         actionsToDo.Clear();
      }
   }
}
