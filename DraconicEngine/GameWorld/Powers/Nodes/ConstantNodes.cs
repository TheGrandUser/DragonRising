using DraconicEngine.GameWorld.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.Actions.Requirements;

namespace DraconicEngine.Powers.Nodes
{
   public class NumberConstantNode : PowerNode
   {
      int value = 0;
      public int Value { get { return value; } set { this.value = value; } }

      NumberNodeOutput valueOutput = new NumberNodeOutput();
      public NumberNodeOutput ValueOutput { get { return valueOutput; } }
      public override ActionRequirement Requirements => NoRequirement.None;

      public override void Do(Entity initiator, RequirementFulfillment fulfillment)
      {
      }
   }
}
