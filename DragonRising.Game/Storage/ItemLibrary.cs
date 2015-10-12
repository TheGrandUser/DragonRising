using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Storage
{
   public interface IItemLibrary
   {
      Entity Get(string name);
      bool Contains(string name);
      void Add(Entity entity);

      Dictionary<string, Entity> Templates { get; }
   }
}
