using DraconicEngine.GameWorld.EntitySystem;
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

      RequirementCreature creatureRequirement = new RequirementCreature();
      public RequirementCreature CreatureRequirement { get { return creatureRequirement; } }

      public override void Do(Entity initiator, ImmutableDictionary<Requirement, Fulfilment> fulfilments)
      {
         var fulfilment = fulfilments[this.creatureRequirement] as FulfilmentCreature;

         if(fulfilment == null)
         {
            throw new PowerException("Requirement not met");
         }

         this.creatureOutput.Pipe(EnumerableEx.Return(fulfilment.Value));
      }
   }

   public class ChooseLocationNode : PowerNode
   {
      LocationNodeOutput locationOutput = new LocationNodeOutput();
      public LocationNodeOutput LocationOutput { get { return locationOutput; } }

      NumberNodeInput rangeInput = new NumberNodeInput();
      public NumberNodeInput RangeInput { get { return rangeInput; } }

      RequirementLocation locationRequirement = new RequirementLocation();
      public RequirementLocation LocationRequirement { get { return locationRequirement; } }

      public override void Do(Entity initiator, ImmutableDictionary<Requirement, Fulfilment> fulfilments)
      {
         var fulfilment = fulfilments[this.locationRequirement] as FulfilmentLocation;

         if (fulfilment == null)
         {
            throw new PowerException("Requirement not met");
         }

         this.locationOutput.Pipe(EnumerableEx.Return(fulfilment.Value));
      }
   }
}