using DraconicEngine;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Terminals.Input;
using DragonRising.GameWorld.Components;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Commands
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
