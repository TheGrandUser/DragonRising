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
      public abstract void Do(Entity initiator, ImmutableDictionary<Requirement, Fulfilment> fulfilments);
   }

}