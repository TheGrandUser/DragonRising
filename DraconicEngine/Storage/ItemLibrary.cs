using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Storage
{
   public class ItemLibrary
   {
      Dictionary<string, ItemTemplate> templates = new Dictionary<string, ItemTemplate>(StringComparer.InvariantCultureIgnoreCase);

      public bool Contains(string name)
      {
         return templates.ContainsKey(name);
      }

      public ItemTemplate this[string name] => this.templates[name];

      public void Add(ItemTemplate itemTemplate)
      {
         this.templates.Add(itemTemplate.Name, itemTemplate);
      }
   }
}
