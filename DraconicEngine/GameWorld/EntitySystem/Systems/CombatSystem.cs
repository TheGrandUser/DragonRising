using DraconicEngine.GameWorld.EntitySystem.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Systems
{
   public class CombatSystem : ListIteratingSystemSync<CombatNode>
   {
      protected override void NodeUpdateFunction(CombatNode node, double time)
      {
      }
   }
}
