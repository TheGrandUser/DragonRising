﻿using DraconicEngine.GameWorld.Actions.Requirements;
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
      public override ActionRequirement Requirements => NoRequirement.None;
      public override void Do(Entity initiator, RequirementFulfillment fulfillment)
      {
         //entityOutput.Entities = EnumerableEx.Return(initiator);
      }
   }
}