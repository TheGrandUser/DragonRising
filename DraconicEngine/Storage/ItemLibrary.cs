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
      void Add(EntityTemplate itemTemplate);
      EntityTemplate Get(string name);
      bool Contains(string name);
   }
}
