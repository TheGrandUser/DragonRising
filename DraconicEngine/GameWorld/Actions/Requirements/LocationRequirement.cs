using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Actions.Requirements
{
   public class LocationRequirement : ActionRequirement
   {
      public bool IsLimitedToFoV { get; }
      public int? MaxRange { get; }

      public LocationRequirement(bool isLimitedToFoV = true, int? maxRange = null)
      {
         this.IsLimitedToFoV = isLimitedToFoV;
         this.MaxRange = maxRange;
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
