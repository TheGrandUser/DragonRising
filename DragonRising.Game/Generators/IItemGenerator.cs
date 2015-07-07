using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;

namespace DragonRising.Generators
{
   public interface IItemGenerator
   {
      Entity GenerateItem(int level);
   }
}
