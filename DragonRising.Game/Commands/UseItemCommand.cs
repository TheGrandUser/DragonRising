using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using DraconicEngine.EntitySystem;
using DragonRising.Commands.Requirements;
using DraconicEngine.RulesSystem;
using DraconicEngine.Terminals.Input;
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld.Components;

namespace DragonRising.Commands
{
   public class UseItemCommand : ActionCommand
   {
      PlanRequirement requirement = new ItemRequirement("Select an item to use", needsItemsFulfillment: true);
      public override PlanRequirement GetRequirement(Entity user) => requirement;
      public override string Name => "Use Item";
      public UseItemCommand()
      {
      }

      public override Either<ActionTaken, AlternateCommmand> PrepareAction(Entity user, RequirementFulfillment fulfillment)
      {
         var itemFulfillment = (ItemFulfillment)fulfillment;
         var item = itemFulfillment.Item;

         return itemFulfillment.FinalizedPlan.Match(
            Some: plan => new UseItemAction(user, item, plan),
            None: () => { throw new ArgumentException("Fulfillment does not have a finalized plan"); });
      }
   }
}
