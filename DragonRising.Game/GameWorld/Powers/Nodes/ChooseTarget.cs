using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DragonRising.GameWorld.Components;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Powers.Nodes
{
   public class ChooseCreatureNode : InputNode
   {
      CreatureNodeOutput creatureOutput = new CreatureNodeOutput();
      NumberNodeInput rangeInput = new NumberNodeInput();

      public CreatureNodeOutput CreatureOutput { get { return creatureOutput; } }
      public NumberNodeInput RangeInput { get { return rangeInput; } }

      public override ActionRequirement Requirements => new EntityRequirement(rangeInput.Value, typeof(CreatureComponent));

      public override void Do(Entity initiator, RequirementFulfillment fulfillment)
      {
         var creatureFulfillment = (EntityFulfillment)fulfillment;

         this.creatureOutput.Pipe(EnumerableEx.Return(creatureFulfillment.Entity));
      }
   }

   public class ChooseLocationNode : InputNode
   {
      LocationNodeOutput locationOutput = new LocationNodeOutput();
      public LocationNodeOutput LocationOutput { get { return locationOutput; } }

      NumberNodeInput rangeInput = new NumberNodeInput();
      public NumberNodeInput RangeInput { get { return rangeInput; } }
      public override ActionRequirement Requirements { get; } = new LocationRequirement();

      public override void Do(Entity initiator, RequirementFulfillment fulfillment)
      {
         var locationFulfillment = (LocationFulfillment)fulfillment;
         var locs = EnumerableEx.Return(locationFulfillment.Location);
         this.locationOutput.Pipe(locs);
      }
   }
}