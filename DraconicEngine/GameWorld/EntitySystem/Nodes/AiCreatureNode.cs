using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Nodes
{
   public class AiCreatureNode : Node
   {
      public BehaviorComponent AI { get; set; }
      public CreatureComponent Creature { get; set; }
      public DecisionComponent Decision { get; set; }

      public override void ClearComponents()
      {
         this.AI = null;
         this.Creature = null;
         this.Decision = null;
      }

      public override void SetComponents(Entity entity)
      {
         this.AI = entity.GetComponent<BehaviorComponent>();
         this.Creature = entity.GetComponent<CreatureComponent>();
         this.Decision = entity.GetComponent<DecisionComponent>();
      }
   }
}