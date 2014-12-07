using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Items;

namespace DraconicEngine.Generators
{
   public interface IItemGenerator
   {
      Item GenerateItem();
      Entity GenerateEntityItem();
   }
}
