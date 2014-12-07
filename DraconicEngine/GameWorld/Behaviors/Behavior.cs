using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Behaviors
{
   public interface IBehaviorTemplate
   {
      IBehavior Create();
   }

   public interface IBehavior
   {
      RogueAction PlanTurn(Entity owner);

      Item SelectInventoryItem(Entity owner);

      Loc? SelectTargetLocation(Entity owner, bool isLimitedToFoV = true);

      Entity SelectTargetCreature(Entity owner, int range = 0);
   }

}
