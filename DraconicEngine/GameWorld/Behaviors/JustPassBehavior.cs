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
   [Serializable]
   public class JustPassBehavior : IBehavior
   {
      public RogueAction PlanTurn(Entity owner) => RogueAction.Idle;

      public Item SelectInventoryItem(Entity owner) => null;

      public Loc? SelectTargetLocation(Entity owner, bool isLimitedToFoV = true) => null;

      public Entity SelectTargetCreature(Entity owner, int range = 0) => null;
   }
}
