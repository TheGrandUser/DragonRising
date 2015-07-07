using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Actions;
using DraconicEngine;
using DragonRising.GameWorld.Components;

namespace DragonRising.GameWorld.Behaviors
{
   public class ConfusedBehavior : Behavior
   {
      public ConfusedBehavior()
         : base("Confused")
      {
      }

      protected ConfusedBehavior(ConfusedBehavior original)
         : base(original)
      {
      }

      public override ActionTaken PlanTurn(Entity owner)
      {
         var result = RogueGame.Current.GameRandom.Next(100);

         if (result < 70) // Move
         {
            var direction = (Direction)RogueGame.Current.GameRandom.Next(9);
            var loc = owner.Location + Vector.FromDirection(direction);
            if (World.Current.Scene.IsBlocked(loc) == Blockage.None)
            {
               return new MoveToAction(owner, loc);
            }
            else
            {
               return ActionTaken.Idle;
            }
         }
         else
         {
            return ActionTaken.Idle;
         }
      }

      protected override Behavior CloneCore()
      {
         return new ConfusedBehavior(this);
      }
   }
}
