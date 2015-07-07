using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using System.Diagnostics;
using DraconicEngine.EntitySystem;
using DragonRising.Commands.Requirements;
using DraconicEngine.RulesSystem;
using static LanguageExt.Prelude;
using DraconicEngine.Terminals.Input;
using DraconicEngine;
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld;
using DragonRising.GameWorld.Alligences;

namespace DragonRising.Commands
{
   public class AttackEntityCommand : ActionCommand
   {
      public override string Name => "Attack";

      public override PlanRequirement GetRequirement(Entity user)
      {
         var range = user.GetComponentOrDefault<EquipmentComponent>();

         return new OrRequirement(
            EntityRequirement.AnyEnemyWithinRange(new SelectionRange(1)),
            new DirectionRequirement());
      }

      public AttackEntityCommand() { }

      public override Either<ActionTaken, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         Entity targetEntity = null;
         if (fulfillment is DirectionFulfillment)
         {
            var dir = (DirectionFulfillment)fulfillment;
            var delta = Vector.FromDirection(dir.Direction);
            if (delta == Vector.Zero)
            {
               return ActionTaken.Abort;
            }
            var targetLocation = executer.GetLocation() + delta;

            var world = World.Current;
            var scene = World.Current.Scene;

            targetEntity = world.EntityEngine.GetCreatureAt(targetLocation);
         }
         else if (fulfillment is EntityFulfillment)
         {
            targetEntity = ((EntityFulfillment)fulfillment).Entity;
         }

         if (targetEntity != null)
         {
            return new AttackEntityAction(executer, targetEntity, None);
         }
         else
         {
            return ActionTaken.Abort;
         }
      }
   }

}
