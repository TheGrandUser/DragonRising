using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using DraconicEngine.Items;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.Actions;

namespace DraconicEngine.Terminals.Input.Commands
{
   public class DropItemCommand : ActionCommand
   {
      ActionRequirement requirement = new ItemRequirement("Select an item to drop", needsItemsFulfillment: false);
      public override ActionRequirement Requirement => requirement;
      public override string Name => "Drop Item";
      public DropItemCommand()
      {

      }

      public override Either<RogueAction, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var item = (ItemFulfillment)fulfillment;

         return new DropItemAction(item.Item);
      }
   }

}
