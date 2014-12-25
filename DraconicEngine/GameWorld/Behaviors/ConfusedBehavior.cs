using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Items;
using DraconicEngine.GameWorld.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;

namespace DraconicEngine.GameWorld.Behaviors
{
   public class ConfusedBehavior : Behavior
   {
      public ConfusedBehavior()
      {
      }

      protected ConfusedBehavior(ConfusedBehavior original)
         : base(original)
      {
      }

      public override RogueAction PlanTurn(Entity owner)
      {
         var result = RogueGame.Current.GameRandom.Next(100);

         if (result < 70) // Move
         {
            var direction = (Direction)RogueGame.Current.GameRandom.Next(9);
            var loc = owner.GetComponent<LocationComponent>().Location + Vector.FromDirection(direction);
            if (Scene.CurrentScene.IsBlocked(loc) == Blockage.None)
            {
               return new MoveToAction(loc);
            }
            else
            {
               return RogueAction.Idle;
            }
         }
         else
         {
            return RogueAction.Idle;
         }
      }

      protected override Behavior CloneCore()
      {
         return new ConfusedBehavior(this);
      }
   }
}
