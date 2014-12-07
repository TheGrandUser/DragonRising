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

namespace DraconicEngine.GameWorld.Actions
{
   public class UseItemAction : RogueAction
   {
      Entity executer;
      Item item;
      bool removeItemOnUse;

      public UseItemAction(Entity executer, Item item, bool removeItemOnUse = false)
      {
         this.removeItemOnUse = removeItemOnUse;
         this.executer = executer;
         this.item = item;
      }

      public override void Do(Entity executer)
      {
         item.Use(executer);
         if (this.removeItemOnUse)
         {
            executer.As<InventoryComponent>(inventory => inventory.Items.Remove(item));
         }
      }
   }
}
