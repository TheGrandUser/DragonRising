using DraconicEngine;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Terminals.Input;
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
      public override ActionRequirement GetRequirement(Entity user) => NoRequirement.None;
      public override string Name => "Pickup";
      public PickUpItemCommand()
      {
      }

      public override Either<RogueAction, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var scene = Scene.CurrentScene;

         var itemToPick = Scene.CurrentScene.EntityStore
            .GetItemsAt(executer.GetLocation())
            .FirstOrDefault();

         if (itemToPick != null)
         {
            var inventory = executer.GetComponentOrDefault<InventoryComponent>();
            if (inventory != null && inventory.Capacity > inventory.Items.Count)
            {
               MessageService.Current.PostMessage("You picked up a " + itemToPick.Name + ".", RogueColors.Green);
               return new PickUpItemAction(itemToPick);
            }
            else
            {
               MessageService.Current.PostMessage("Your inventory is full, cannot pick up " + itemToPick.Name + ".", RogueColors.Red);
               return RogueAction.Abort;
            }
         }
         return RogueAction.Abort;
      }
   }

}
