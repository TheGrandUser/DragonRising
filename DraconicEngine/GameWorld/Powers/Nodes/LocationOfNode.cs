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
      public EntityNodeInput EntityInput { get { return entityInput; } }
      LocationNodeOutput locationOutput = new LocationNodeOutput();
      public LocationNodeOutput LocationOutput { get { return locationOutput; } }
      public override ActionRequirement Requirements => NoRequirement.None;
      public override void Do(Entity initiator, RequirementFulfillment fulfillment)
      {
         //locationOutput.Locations = entityInput.Value.Select(entity => entity.Location);
      }
   }
}