using DraconicEngine;
using DraconicEngine.RulesSystem;
using DragonRising.Commands.Requirements;
using DraconicEngine.EntitySystem;
using DraconicEngine.Terminals.Input;
using DragonRising.GameWorld.Components;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.GameWorld.Alligences;

namespace DragonRising.Commands
{
   public class InteractWithCreatureCommand: ActionCommand
   {
      PlanRequirement requirement =
         new EntityRequirement(new SelectionRange(1), true, null, typeof(CreatureComponent));

      public override PlanRequirement GetRequirement(Entity user) => requirement;
      public override string Name => "Interact With Creature";
      public InteractWithCreatureCommand() { }

      public override Either<ActionTaken, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var other = (EntityFulfillment)fulfillment;
         
         if (executer.IsEnemy(other.Entity) && other.Entity.HasComponent<CombatantComponent>())
         {
            return new AlternateCommmand(new AttackEntityCommand(), other);
         }
         // else talk to the creature
         return ActionTaken.Abort;
      }
   }
}
