using DraconicEngine;
using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Commands.Requirements
{
   public class LocationRequirement : PlanRequirement
   {
      public SelectionRange SelectionRange { get; }

      public LocationRequirement(bool isLimitedToFoV = true, int? maxRange = null)
      {
         this.SelectionRange = new SelectionRange(maxRange,
            isLimitedToFoV ? RangeLimits.LineOfSight : RangeLimits.None);
      }

      public override bool MeetsRequirement(RequirementFulfillment fulfillment)
      {
         return fulfillment is LocationFulfillment;
      }
   }

   public class LocationFulfillment : RequirementFulfillment
   {
      public Loc Location { get; }
      public LocationFulfillment(Loc location)
      {
         this.Location = location;
      }
   }
}
