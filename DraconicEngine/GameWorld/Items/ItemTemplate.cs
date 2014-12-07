using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Items
{
   public class ItemTemplate
   {
      public Glyph Glyph { get; set; }
      public RogueColor Color { get; set; }
      public IItemUsage Usage { get; set; }
      public int MaxCharges { get; set; }
      public bool IsCharged { get; set; }

      public string Name { get; set; }

      public ItemTemplate(string name, Glyph glyph, RogueColor color)
      {
         this.Name = name;
         this.Glyph = glyph;
         this.Color = color;
      }

      protected virtual Item CreateEntityCore()
      {
         return new Item(Name, this);
      }

      public Item Create()
      {
         return (Item)CreateEntityCore();
      }
   }
}
