using DraconicEngine.GameWorld.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.Actions.Requirements;

namespace DragonRising.GameWorld.Powers.Nodes
{
   public class NumberConstantNode : PowerNode
   {
      int value = 0;
      public int Value { get { return value; } set { this.value = value; } }

      NumberNodeOutput valueOutput = new NumberNodeOutput();
      public NumberNodeOutput ValueOutput { get { return valueOutput; } }
   }
}
