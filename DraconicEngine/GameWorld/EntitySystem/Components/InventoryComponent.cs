using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   public class InventoryComponent : Component
   {
      public InventoryComponent(InventoryComponentTemplate template)
      {
         this.Template = template;
      }

      public bool TryPickUp(Entity item)
      {
         if (!item.HasComponent<ItemComponent>()) { return false; } // throw?
         if (this.Items.Count >= this.Template.Capacity)
         {
            return false;
         }

         this.Items.Add(item);

         return true;
      }

      public List<Entity> Items { get; } = new List<Entity>();

      public InventoryComponentTemplate Template { get; set; }
   }

   public class InventoryComponentTemplate : ComponentTemplate
   {
      public int Capacity { get; set; }

      public override Type ComponentType => typeof(InventoryComponent);

      public InventoryComponentTemplate(int capacity)
      {
         this.Capacity = capacity;
      }

      public override Component CreateComponent()
      {
         return new InventoryComponent(this);
      }
   }
}
