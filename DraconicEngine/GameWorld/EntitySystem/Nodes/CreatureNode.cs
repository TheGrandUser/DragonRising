using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Nodes
{
   public class CreatureNode : Node
   {
      public CreatureComponent Creature { get; set; }
      public InventoryComponent Inventory { get; set; }

      public override void ClearComponents()
      {
         this.Creature = null;
         this.Inventory = null;
      }

      public override void SetComponents(Entity entity)
      {
         this.Creature = entity.GetComponent<CreatureComponent>();
         this.Inventory = entity.GetComponent<InventoryComponent>();
      }
   }
}
