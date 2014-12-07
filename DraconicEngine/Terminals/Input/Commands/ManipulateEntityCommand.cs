using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using DraconicEngine.Items;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.Actions.Requirements;

namespace DraconicEngine.Terminals.Input.Commands
{
   public class ManipulateEntityCommand : ActionCommand
   {
      ActionRequirement requirement = new AndMaybeRequirement<EntityFulfillment>(
         new EntityRequirement(1, typeof(ManipulatableComponent)),
         new ItemRequirement("Select an item to use on the object", needsItemsFulfillment: false),
         f => f.Entity.GetComponent<ManipulatableComponent>().RequiresItem);
      public override string Name => "Manipulate Entity";
      public override ActionRequirement Requirement => requirement;

      public ManipulateEntityCommand()
      {
      }

      public override Either<RogueAction, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var andFulfillment = (AndMaybeFulfillment)fulfillment;
         var entityFulfillment = (EntityFulfillment)andFulfillment.First.Value;
         var other = entityFulfillment.Entity;
         var optionalFulfillment = andFulfillment.Second;

         var m = other.GetComponent<ManipulatableComponent>();
         if (m.RequiresItem)
         {
            return optionalFulfillment.Match(
               Some: itemFulfillment => new ManipulateEntityAction(other, ((ItemFulfillment)itemFulfillment).Item),
               None: () => RogueAction.Abort);
         }
         return new ManipulateEntityAction(other);
      }
   }

}
