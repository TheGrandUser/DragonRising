using DraconicEngine.GameWorld.EntitySystem.Components;
using DragonRising.GameWorld.Items;
using DraconicEngine.GameWorld.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using DragonRising.GameWorld.Actions;
using DraconicEngine;
using DraconicEngine.GameWorld;
using DraconicEngine.GameWorld.Behaviors;
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

      public override RogueAction PlanTurn(Entity owner)
      {
         var result = RogueGame.Current.GameRandom.Next(100);

         if (result < 70) // Move
         {
            var direction = (Direction)RogueGame.Current.GameRandom.Next(9);
            var loc = owner.GetComponent<LocationComponent>().Location + Vector.FromDirection(direction);
            if (World.Current.Scene.IsBlocked(loc) == Blockage.None)
            {
               return new MoveToAction(owner, loc);
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
