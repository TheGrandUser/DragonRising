using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Storage;
using DraconicEngine.Items;
using DraconicEngine.GameWorld.EntitySystem;

namespace DragonRising.Libraries
{
   class SimpleItemLibrary : IItemLibrary
   {
      Dictionary<string, EntityTemplate> templates = new Dictionary<string, EntityTemplate>(StringComparer.InvariantCultureIgnoreCase);

      public bool Contains(string name)
      {
         return templates.ContainsKey(name);
      }

      public EntityTemplate Get(string name) => this.templates[name];

      public void Add(EntityTemplate itemTemplate)
      {
         this.templates.Add(itemTemplate.Name, itemTemplate);
      }
   }
}
