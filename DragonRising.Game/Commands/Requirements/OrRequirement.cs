using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Commands.Requirements
{
   public class OrRequirement : PlanRequirement
   {
      public PlanRequirement Prefered { get; private set; }
      public PlanRequirement Alternate { get; private set; }
      public OrRequirement(PlanRequirement prefered, PlanRequirement alternate, string message = "")
      {
         this.Prefered = prefered;
         this.Alternate = alternate;
         this.Message = message;
      }

      public override bool MeetsRequirement(RequirementFulfillment fulfillment)
      {
         return Prefered.MeetsRequirement(fulfillment) || Alternate.MeetsRequirement(fulfillment);
      }
   }
}
