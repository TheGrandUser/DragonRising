using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Powers.Nodes
{
   public class ChooseCreatureNode : PowerNode
   {
      CreatureNodeOutput creatureOutput = new CreatureNodeOutput();
      public CreatureNodeOutput CreatureOutput { get { return creatureOutput; } }

      NumberNodeInput rangeInput = new NumberNodeInput();
      public NumberNodeInput RangeInput { get { return rangeInput; } }

      public override ActionRequirement Requirements { get; } = new EntityRequirement(null, typeof(CreatureComponent));

      public override void Do(Entity initiator, RequirementFulfillment fulfillment)
      {
         var creatureFulfillment = (EntityFulfillment)fulfillment;

         this.creatureOutput.Pipe(EnumerableEx.Return(creatureFulfillment.Entity));
      }
   }

   public class ChooseLocationNode : PowerNode
   {
      LocationNodeOutput locationOutput = new LocationNodeOutput();
      public LocationNodeOutput LocationOutput { get { return locationOutput; } }

      NumberNodeInput rangeInput = new NumberNodeInput();
      public NumberNodeInput RangeInput { get { return rangeInput; } }
      public override ActionRequirement Requirements { get; } = new LocationRequirement();

      public override void Do(Entity initiator, RequirementFulfillment fulfillment)
      {
         var locationFulfillment = (LocationFulfillment)fulfillment;
         this.locationOutput.Pipe(EnumerableEx.Return(locationFulfillment.Location));
      }
   }
}