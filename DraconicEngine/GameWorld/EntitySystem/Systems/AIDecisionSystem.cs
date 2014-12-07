using DraconicEngine.GameWorld.EntitySystem.Nodes;
using DraconicEngine.GameWorld.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Systems
{
   public class AIDecisionSystem : ListIteratingSystemSync<AiCreatureNode>
   {
      protected override void NodeUpdateFunction(AiCreatureNode node, double time)
      {
         if (!node.AI.IsDirectlyControlled)
         {
            var readyAction = node.AI.CurrentBehavior.PlanTurn(node.Entity);
            node.Decision.ActionToDo = readyAction;
         }
      }
   }
}