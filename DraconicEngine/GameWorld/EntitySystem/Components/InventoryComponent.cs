using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   public class InventoryComponent : Component
   {
      public InventoryComponent(int capacity)
      {
         this.Capacity = capacity;
      }

      public bool TryPickUp(Item item)
      {
         if (this.Items.Count >= this.Capacity)
         {
            return false;
         }

         this.Items.Add(item);

         return true;
      }

      public List<Item> Items { get; } = new List<Item>();

      public int Capacity { get; set; }
   }
}
