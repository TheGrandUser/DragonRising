using DragonRising.GameWorld.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld;
using DraconicEngine;

namespace DragonRising.GameWorld.Components
{
   public class InventoryComponent : Component
   {
      public int Capacity { get; set; }

      public InventoryComponent()
      {
      }

      protected InventoryComponent(InventoryComponent original, bool fresh)
         : base(original, fresh)
      {
         Capacity = original.Capacity;

         if (!fresh)
         {
            foreach(var item in original.Items)
            {
               this.Items.Add(item.Clone(fresh: false));
            }
         }
      }

      public bool TryPickUp(Entity item)
      {
         if (!item.HasComponent<ItemComponent>()) { return false; } // throw?
         if (this.Items.Count >= this.Capacity)
         {
            return false;
         }

         this.Items.Add(item);

         return true;
      }

      public List<Entity> Items { get; } = new List<Entity>();

      protected override Component CloneCore(bool fresh)
      {
         return new InventoryComponent(this,fresh);
      }
   }

   //public class InventoryComponentTemplate : ComponentTemplate
   //{
   //   public int Capacity { get; set; }

   //   public override Type ComponentType => typeof(InventoryComponent);

   //   public InventoryComponentTemplate(int capacity)
   //   {
   //      this.Capacity = capacity;
   //   }

   //   public override Component CreateComponent()
   //   {
   //      return new InventoryComponent(this);
   //   }
   //}
}
