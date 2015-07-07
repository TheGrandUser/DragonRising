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

namespace DragonRising.Commands
{
   public class DropItemCommand : ActionCommand
   {
      PlanRequirement requirement = new ItemRequirement("Select an item to drop", needsItemsFulfillment: false);
      public override PlanRequirement GetRequirement(Entity user) => requirement;
      public override string Name => "Drop Item";
      public DropItemCommand()
      {

      }

      public override Either<ActionTaken, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var item = (ItemFulfillment)fulfillment;

         return new DropItemAction(executer, item.Item);
      }
   }
}
