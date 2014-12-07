using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Nodes
{
   public class CreatureActionNode : Node
   {
      public CreatureComponent Creature { get; set; }
      public DecisionComponent Decision { get; set; }

      public override void ClearComponents()
      {
         this.Creature = null;
         this.Decision = null;
      }

      public override void SetComponents(Entity entity)
      {
         this.Creature = entity.GetComponent<CreatureComponent>();
         this.Decision = entity.GetComponent<DecisionComponent>();
      }
   }
}
