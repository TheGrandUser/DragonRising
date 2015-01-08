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
   public class UseItemCommand : ActionCommand
   {
      ActionRequirement requirement = new ItemRequirement("Select an item to use", needsItemsFulfillment: true);
      public override ActionRequirement GetRequirement(Entity user) => requirement;
      public override string Name => "Use Item";
      public UseItemCommand()
      {
      }

      public override Either<RogueAction, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var itemFulfillment = (ItemFulfillment)fulfillment;
         var item = itemFulfillment.Item;

         return new UseItemAction(item, itemFulfillment.ItemsFulfillments);
      }
   }
}
