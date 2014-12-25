using DraconicEngine.GameWorld.EntitySystem.Nodes;
using DraconicEngine.GameWorld.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Systems
{
   public class AIDecisionSystem : ListIteratingSystemSync<BehaviorNode>
   {
      ActionManagementNode actionsNode;

      public AIDecisionSystem()
      {
      }

      public override void AddToEngine(Engine engine)
      {
         actionsNode = engine.GetNodes<ActionManagementNode>().Single();

         base.AddToEngine(engine);
      }

      protected override void NodeUpdateFunction(BehaviorNode node, double time)
      {
         var readyAction = node.Behavior.CurrentBehavior.PlanTurn(node.Entity);
         actionsNode.Actions.AddAction(node.Entity, readyAction);
      }
   }
}