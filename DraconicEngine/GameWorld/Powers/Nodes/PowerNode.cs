using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Powers.Nodes
{
   public abstract class PowerNode
   {
   }

   public abstract class InputNode : PowerNode
   {
      public abstract ActionRequirement Requirements { get; }

      public abstract void Do(Entity initiator, RequirementFulfillment fulfillment);
   }
}