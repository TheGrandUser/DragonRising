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
   public class LocationOfNode : PowerNode
   {
      EntityNodeInput entityInput = new EntityNodeInput();
      LocationNodeOutput locationOutput = new LocationNodeOutput();

      public EntityNodeInput EntityInput { get { return entityInput; } }
      public LocationNodeOutput LocationOutput { get { return locationOutput; } }
   }
}