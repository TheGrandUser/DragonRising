using DraconicEngine.GameWorld.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DragonRising.GameWorld.Nodes;

namespace DragonRising.GameWorld.Systems
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