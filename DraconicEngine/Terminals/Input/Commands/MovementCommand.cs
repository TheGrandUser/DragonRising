using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.Actions;

namespace DraconicEngine.Terminals.Input.Commands
{
   public class MovementCommand : ActionCommand
   {
      Direction? direction;

      ActionRequirement requirement = new DirectionRequirement();
      public override ActionRequirement Requirement => direction == null ? requirement : NoRequirement.None;
      public override string Name => "Move";
      public MovementCommand(Direction? direction = null)
      {
         this.direction = direction;
      }

      public override Either<RogueAction, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var dirFulfillment = fulfillment as DirectionFulfillment;
         var direction = this.direction ?? dirFulfillment?.Direction ?? Direction.None;
         if(direction == Direction.None)
         {
            return RogueAction.Abort;
         }

         var scene = Scene.CurrentScene;

         var delta = Vector.FromDirection(direction);

         var newLocation = executer.Location + delta;

         var blockage = scene.IsBlocked(newLocation, ignoreWhere: entity => entity == executer);
         if (blockage == Blockage.None)
         {
            return new MoveToAction(newLocation);
         }
         else if (blockage == Blockage.Entity)
         {
            var other = scene.EntityStore.GetEntitiesAt(newLocation).Single(e => e.Blocks);

            if (other.HasComponent<ManipulatableComponent>())
            {
               return new AlternateCommmand(new ManipulateEntityCommand(), new EntityFulfillment(other));
            }
            else if (other.HasComponent<CreatureComponent>())
            {
               return new AlternateCommmand(new InteractWithCreatureCommand(), new EntityFulfillment(other));
            }
            else
            {
               return RogueAction.Abort;
            }
         }
         return RogueAction.Abort;
      }
   }
}
