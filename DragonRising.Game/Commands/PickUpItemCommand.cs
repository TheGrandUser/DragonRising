using DraconicEngine;
using DraconicEngine.RulesSystem;
using DragonRising.Commands.Requirements;
using DraconicEngine.EntitySystem;
using DraconicEngine.Terminals.Input;
using DragonRising.GameWorld;
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld.Components;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Commands
{
   public class PickUpItemCommand : ActionCommand
   {
      public override PlanRequirement GetRequirement(Entity user) => NoRequirement.None;
      public override string Name => "Pickup";
      public PickUpItemCommand()
      {
      }

      public override Either<ActionTaken, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var scene = World.Current.Scene;

         var itemToPick = World.Current.Scene.EntityStore
            .GetItemsAt(executer.GetLocation())
            .FirstOrDefault();

         if (itemToPick != null)
         {
            var inventory = executer.GetComponentOrDefault<InventoryComponent>();
            if (inventory != null && inventory.Capacity > inventory.Items.Count)
            {
               MessageService.Current.PostMessage("You picked up a " + itemToPick.Name + ".", RogueColors.Green);
               return new PickUpItemAction(executer, itemToPick);
            }
            else
            {
               MessageService.Current.PostMessage("Your inventory is full, cannot pick up " + itemToPick.Name + ".", RogueColors.Red);
               return ActionTaken.Abort;
            }
         }
         return ActionTaken.Abort;
      }
   }

}
