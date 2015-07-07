using DraconicEngine.RulesSystem;
using DragonRising.Commands.Requirements;
using DraconicEngine.EntitySystem;
using DraconicEngine.Terminals.Input;
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld.Components;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Commands
{
   public class RequiresItemCheck : MaybeCheck<EntityFulfillment>
   {
      public override bool Check(EntityFulfillment fulfillment)
      {
         return fulfillment.Entity.GetComponent<ManipulatableComponent>().RequiresItem;
      }
   }

   public class ManipulateEntityCommand : ActionCommand
   {
      PlanRequirement requirement = new AndMaybeRequirement<EntityFulfillment>(
         new EntityRequirement(new SelectionRange(1), true, null, typeof(ManipulatableComponent)),
         new ItemRequirement("Select an item to use on the object", needsItemsFulfillment: false),
         new RequiresItemCheck());
      public override string Name => "Manipulate Entity";
      public override PlanRequirement GetRequirement(Entity user) => requirement;

      public ManipulateEntityCommand()
      {
      }

      public override Either<ActionTaken, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var andFulfillment = (AndMaybeFulfillment)fulfillment;
         var entityFulfillment = (EntityFulfillment)andFulfillment.First.Value;
         var other = entityFulfillment.Entity;
         var optionalFulfillment = andFulfillment.Second;

         var m = other.GetComponent<ManipulatableComponent>();
         if (m.RequiresItem)
         {
            return optionalFulfillment.Match(
               Some: itemFulfillment => new ManipulateEntityAction(executer, other, ((ItemFulfillment)itemFulfillment).Item),
               None: () => ActionTaken.Abort);
         }
         return new ManipulateEntityAction(executer, other, None);
      }
   }

}
