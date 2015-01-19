using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;

namespace DragonRising.Generators
{
   public interface IPopulationGenerator
   {
      List<Entity> GenerarateMonsters(int min, int max);

      Entity GenerarateMonster();
   }
}
