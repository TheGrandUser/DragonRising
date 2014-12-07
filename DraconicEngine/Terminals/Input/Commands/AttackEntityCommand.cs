using System;
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

namespace DraconicEngine.Terminals.Input.Commands
{
   public class AttackEntityCommand : ActionCommand
   {
      public override string Name => "Attack";

      ActionRequirement requirement = new OrRequirement(
         new EntityRequirement(),
         new DirectionRequirement());
      public override ActionRequirement Requirement => requirement;

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

         var target = targetEntity?.GetComponentOrDefault<CombatantComponent>();
         return new AttackEntityAction(target);
      }
   }

}
