﻿using DraconicEngine.GameWorld.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.Actions.Requirements;

namespace DragonRising.GameWorld.Powers.Nodes
{
   public class GetEntitiesWithinNode : PowerNode
   {
      public int Radius { get; set; }

      public bool LineOfEffect { get; set; }

      EntityNodeOutput entitiesOutput = new EntityNodeOutput();
      public EntityNodeOutput EntitiesOutput { get { return entitiesOutput; } }

      LocationNodeInput locationInput = new LocationNodeInput();
      public LocationNodeInput LocationInput { get { return locationInput; } }
   }
}