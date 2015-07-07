using DraconicEngine;
using DraconicEngine.RulesSystem;
using DragonRising.Commands.Requirements;
using DraconicEngine.EntitySystem;
using DraconicEngine.Terminals.Input;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Commands
{
   public abstract class ActionCommand : RogueCommand
   {
      public abstract string Name { get; }
      public abstract PlanRequirement GetRequirement(Entity user);
      public abstract Either<ActionTaken, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment);

      public static async Task<ActionTaken> GetFinalActionAsync(
         ActionCommand action, Entity entity,
         Func<PlanRequirement, Task<RequirementFulfillment>> getFulfillment,
         RequirementFulfillment preFulfillment = null)
      {
         var requirement = action.GetRequirement(entity);

         RequirementFulfillment fulfillment = requirement is NoRequirement ? NoFulfillment.None :
            preFulfillment ?? await getFulfillment(requirement);

         if(fulfillment is NoFulfillment && !(requirement is NoRequirement) || !requirement.MeetsRequirement(fulfillment))
         {
            return ActionTaken.Abort;
         }

         var result = await action.PrepareAction(entity, fulfillment).Match(
               Left: taken => Task.FromResult(taken),
               Right: another => GetFinalActionAsync(another.Action, entity, getFulfillment, another.PreFulfillment));

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
