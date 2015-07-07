using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Nodes;
using LanguageExt;
using static LanguageExt.Prelude;
using DragonRising.Services;
using DragonRising.Rules;

namespace DragonRising.GameWorld.Systems
{
   public class ActionSystem : ListIteratingSystemSync<BehaviorNode>
   {
      IRulesManager rulesManager;
      Dictionary<Entity, ActionTaken> actionsStore = new Dictionary<Entity, ActionTaken>();

      public ActionSystem(IRulesManager rulesManager)
      {
         this.rulesManager = rulesManager;
      }
        
      public override void Update(double time)
      {
         base.Update(time);

         var actionsToDo = actionsStore.Select(kvp => kvp.Value).ToList();
         actionsStore.Clear();

         foreach (var action in actionsToDo)
         {
            rulesManager.ProcessFact(action);
         }
      }

      protected override void NodeUpdateFunction(BehaviorNode node, double time)
      {
         var readyAction = node.Behavior.CurrentBehavior.PlanTurn(node.Entity);
         this.actionsStore.Add(node.Entity, readyAction);
      }
   }
}