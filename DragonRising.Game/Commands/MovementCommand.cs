using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using DraconicEngine.EntitySystem;
using DragonRising.Commands.Requirements;
using DraconicEngine.RulesSystem;
using DraconicEngine.Terminals.Input;
using DraconicEngine;
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld;

namespace DragonRising.Commands
{
   public class MovementCommand : ActionCommand
   {
      Vector delta;
      
      public MovementCommand(Vector delta)
      {
         this.delta = delta.Unitize();
      }

      public override PlanRequirement GetRequirement(Entity user) => NoRequirement.None;
      public override string Name => "Move";

      public override Either<ActionTaken, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var dirFulfillment = fulfillment as DirectionFulfillment;
         if(delta == Vector.Zero)
         {
            return ActionTaken.Abort;
         }
         
         var scene = World.Current.Scene;
         
         var newLocation = executer.GetLocation() + delta;

         var blockage = scene.IsBlocked(newLocation, ignoreWhere: entity => entity == executer);
         if (blockage == Blockage.None)
         {
            return new MoveToAction(executer, newLocation);
         }
         else if (blockage == Blockage.Entity)
         {
            var other = scene.EntityStore.GetEntitiesAt(newLocation).Single(e => e.GetBlocks());

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
               return ActionTaken.Abort;
            }
         }
         return ActionTaken.Abort;
      }
   }
}
