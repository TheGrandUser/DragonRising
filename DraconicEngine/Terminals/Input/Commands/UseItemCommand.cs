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
      public override ActionRequirement Requirement { get; } = new ItemRequirement("Select an item to use", needsItemsFulfillment: true);
      public override string Name => "Use Item";
      public UseItemCommand()
      {
      }

      public override Either<RogueAction, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var itemFulfillment = (ItemFulfillment)fulfillment;
         var item = itemFulfillment.Item;
         var inventory = executer.GetComponent<InventoryComponent>();



         if (item != null)
         {
            var usageResult = item.PrepUse(executer, itemFulfillment.ItemsFulfillments);

            switch (usageResult)
            {
               case ItemUseResult.NotUsed:
                  return RogueAction.Abort;
               case ItemUseResult.Used:
                  return new UseItemAction(executer, item);
               case ItemUseResult.Destroyed:
                  return new UseItemAction(executer, item, removeItemOnUse: true);
            }
         }

         return RogueAction.Abort;
      }
   }
}
