using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Actions.Requirements
{
   public class ConfirmRequirement : ActionRequirement
   {
      public ConfirmRequirement(string message)
      {
         this.Message = message;
      }

      public override bool MeetsRequirement(RequirementFulfillment fulfillment)
      {
         return fulfillment is ConfirmFulfillment;
      }
   }

   public class ConfirmFulfillment : RequirementFulfillment
   {
      public bool Result { get; }
      public ConfirmFulfillment(bool result)
      {
         this.Result = result;
      }
   }
}
