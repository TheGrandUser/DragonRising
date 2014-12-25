using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Input;

namespace DraconicEngine.GameWorld.Behaviors
{
   public class PlayerControlledBehavior : Behavior
   {
      IPlayerController playerController;

      public PlayerControlledBehavior(IPlayerController playerController)
      {
         this.playerController = playerController;
      }

      public override RogueAction PlanTurn(Entity owner)
      {
         return playerController.GetAction();
      }

      protected PlayerControlledBehavior(PlayerControlledBehavior original)
         :base(original)
      {
         this.playerController = original.playerController;
      }

      protected override Behavior CloneCore()
      {
         return new PlayerControlledBehavior(this);
      }
   }
}
