﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using System.Diagnostics;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.Actions;
using LanguageExt.Prelude;

namespace DraconicEngine.Terminals.Input.Commands
{
   public class AttackEntityCommand : ActionCommand
   {
      public override string Name => "Attack";

      public override ActionRequirement GetRequirement(Entity user)
      {
         var range = user.GetComponentOrDefault<EquipmentComponent>();

         return new OrRequirement(
            new EntityRequirement(1, typeof(CombatantComponent)),
            new DirectionRequirement());
      }

      public AttackEntityCommand() { }

      public override Either<RogueAction, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         Entity targetEntity = null;
         if (fulfillment is DirectionFulfillment)
         {
            var dir = (DirectionFulfillment)fulfillment;
            var delta = Vector.FromDirection(dir.Direction);
            if (delta == Vector.Zero)
            {
               return RogueAction.Abort;
            }
            var targetLocation = executer.Location + delta;

            var scene = Scene.CurrentScene;

            targetEntity = scene.EntityStore.GetCreatureAt(targetLocation);
         }
         else if (fulfillment is EntityFulfillment)
         {
            targetEntity = ((EntityFulfillment)fulfillment).Entity;
         }

         if (targetEntity != null)
         {
            return new AttackEntityAction(targetEntity, None);
         }
         else
         {
            return RogueAction.Abort;
         }
      }
   }

}
