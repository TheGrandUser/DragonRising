using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Nodes
{
   public class CombatNode : Node
   {
      public CombatantComponent Combat { get; set; }

      public override void ClearComponents()
      {
         Combat = null;
      }

      public override void SetComponents(Entity entity)
      {
         Combat = entity.GetComponent<CombatantComponent>();
      }
   }
}
