using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Conditions
{
   public interface ICondition
   {
      string Name { get; }

      string AppliedMessage(Entity entity);
      string RemovedMessage(Entity entity);
   }
}
