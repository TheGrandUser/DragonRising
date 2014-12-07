using DraconicEngine.GameWorld.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Powers.Nodes
{
   public class SelfNode : PowerNode
   {
      EntityNodeOutput entityOutput = new EntityNodeOutput();
      public EntityNodeOutput EntityOutput { get { return entityOutput; } }

      public override void Do(Entity initiator, ImmutableDictionary<Requirement, Fulfilment> fulfilments)
      {
         //entityOutput.Entities = EnumerableEx.Return(initiator);
      }
   }
}