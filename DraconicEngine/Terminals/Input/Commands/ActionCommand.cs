using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.Actions.Requirements;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using System.Diagnostics.Contracts;

namespace DraconicEngine.Terminals.Input.Commands
{
   public abstract class ActionCommand : RogueCommand
   {
      public abstract string Name { get; }
      public abstract ActionRequirement GetRequirement(Entity user);
      public abstract Either<RogueAction, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment);

      public static async Task<RogueAction> GetFinalActionAsync(
         ActionCommand action, Entity entity,
         Func<ActionRequirement, Task<RequirementFulfillment>> getFulfillment,
         RequirementFulfillment preFulfillment = null)
      {
         var requirement = action.GetRequirement(entity);

         RequirementFulfillment fulfillment = requirement is NoRequirement ? NoFulfillment.None :
            preFulfillment ?? await getFulfillment(requirement);

         if(fulfillment is NoFulfillment && !(requirement is NoRequirement))
         {
            return RogueAction.Abort;
         }

         var result = await action.PrepareAction(entity, fulfillment).Match(
               taken => Task.FromResult(taken),
               another => GetFinalActionAsync(another.Action, entity, getFulfillment, another.PreFulfillment));

         return result;
      }
   }

   public class AlternateCommmand
   {
      public ActionCommand Action { get; }
      public RequirementFulfillment PreFulfillment { get; }

      public AlternateCommmand(ActionCommand action, RequirementFulfillment fulfillment = null)
      {
         this.Action = action;
         this.PreFulfillment = fulfillment;
      }

      public static implicit operator AlternateCommmand(ActionCommand action)
      {
         return new AlternateCommmand(action);
      }
   }

}
