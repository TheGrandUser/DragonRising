using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DragonRising.GameWorld.Items;

namespace DragonRising.Generators
{
   public interface IItemGenerator
   {
      Entity GenerateItem(int level);
   }
}
