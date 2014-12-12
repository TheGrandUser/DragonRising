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
   public class InteractWithCreatureCommand: ActionCommand
   {
      ActionRequirement requirement = //new OrRequirement(
         new EntityRequirement();//,
                                 //new DirectionRequirement());

      public override ActionRequirement Requirement => requirement;
      public override string Name => "Interact With Creature";
      public InteractWithCreatureCommand() { }

      public override Either<RogueAction, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var other = (EntityFulfillment)fulfillment;

         if (executer.IsEnemy(other.Entity) && other.Entity.HasComponent<CombatantComponent>())
         {
            return new AlternateCommmand(new AttackEntityCommand(), new EntityFulfillment(other.Entity));
         }
         return RogueAction.Abort;
      }
   }
}
