using DraconicEngine.GameWorld.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DragonRising.GameWorld.Nodes;
using LanguageExt;
using static LanguageExt.Prelude;

namespace DragonRising.GameWorld.Systems
{
   public class ActionSystem : ListIteratingSystemSync<BehaviorNode>
   {
      Dictionary<Entity, RogueAction> actionsSTore = new Dictionary<Entity, RogueAction>();
      
      public override void Update(double time)
      {
         base.Update(time);

         var actionsToDo = actionsSTore.Select(kvp => tuple(kvp.Key, kvp.Value)).ToList();
         actionsSTore.Clear();

         foreach (var tuple in actionsToDo)
         {
            tuple.With((entity, action) => action.Do(entity));
         }
      }

      protected override void NodeUpdateFunction(BehaviorNode node, double time)
      {
         var readyAction = node.Behavior.CurrentBehavior.PlanTurn(node.Entity);
         this.actionsSTore.Add(node.Entity, readyAction);
      }
   }
}