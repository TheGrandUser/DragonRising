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
         new EntityRequirement(1, typeof(CreatureComponent));//,
                                 //new DirectionRequirement());

      public override ActionRequirement GetRequirement(Entity user) => requirement;
      public override string Name => "Interact With Creature";
      public InteractWithCreatureCommand() { }

      public override Either<RogueAction, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var other = (EntityFulfillment)fulfillment;

         if (executer.IsEnemy(other.Entity) && other.Entity.HasComponent<CombatantComponent>())
         {
            return new AlternateCommmand(new AttackEntityCommand(), other);
         }
         // else talk to the creature
         return RogueAction.Abort;
      }
   }
}
