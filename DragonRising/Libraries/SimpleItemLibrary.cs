using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.Storage;
using DraconicEngine.EntitySystem;

namespace DragonRising.Libraries
{
   class SimpleItemLibrary : IItemLibrary
   {
      Dictionary<string, Entity> templates = new Dictionary<string, Entity>(StringComparer.InvariantCultureIgnoreCase);

      public Dictionary<string, Entity> Templates => templates;

      public bool Contains(string name)
      {
         return templates.ContainsKey(name);
      }

      public Entity Get(string name) => this.templates[name];

      public void Add(Entity itemTemplate)
      {
         this.templates.Add(itemTemplate.Name, itemTemplate);
      }
   }
}
