using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Storage
{
   public interface IItemLibrary
   {
      Entity Get(string name);
      bool Contains(string name);
      void Add(Entity entity);

      Dictionary<string, Entity> Templates { get; }
   }

   public interface IItemUsageLibrary
   {
      IItemUsage Get(string name);
      bool Contains(string name);
      void Add(string name, IItemUsage usage);
      string NameForUsage(IItemUsage usage);
   }
}
