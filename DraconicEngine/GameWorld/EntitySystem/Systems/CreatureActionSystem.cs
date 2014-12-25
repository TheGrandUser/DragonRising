using DraconicEngine.GameWorld.EntitySystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Systems
{
   public class CreatureActionSystem : GameSystemSync
   {
      ActionManagementNode actionsNode;

      public override void AddToEngine(Engine engine)
      {
         this.actionsNode = engine.GetNodes<ActionManagementNode>().Single();

         base.AddToEngine(engine);

      }

      public override void Update(double time)
      {
         var actionsToDo = actionsNode.Actions.GetAndClearActions();

         foreach(var tuple in actionsToDo)
         {
            tuple.With((entity, action) => action.Do(entity));
         }
      }
   }
}
