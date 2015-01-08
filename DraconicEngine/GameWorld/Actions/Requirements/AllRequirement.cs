using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Actions.Requirements
{
   public class AllRequirement : ActionRequirement
   {
      public IImmutableList<ActionRequirement> Requirements { get; }
      public AllRequirement(IEnumerable<ActionRequirement> requirements, string message = "")
      {
         this.Requirements = requirements.ToImmutableList();

         if (Requirements.Count < 2)
         {
            throw new ArgumentException("There must be at least two requirements");
         }
         this.Message = message;
      }

      public override bool MeetsRequirement(RequirementFulfillment fulfillment)
      {
         var allFulfillment = fulfillment as AllFulfillment;
         if (allFulfillment != null)
         {
            if (allFulfillment.Fulfillments.Count != Requirements.Count)
            {
               return false;
            }
            return this.Requirements.Zip(allFulfillment.Fulfillments, (r, f) => r.MeetsRequirement(f)).All(f => f);
         }
         return false;
      }
   }

   public class AllFulfillment : RequirementFulfillment
   {
      public IImmutableList<RequirementFulfillment> Fulfillments { get; }
      public AllFulfillment(IEnumerable<RequirementFulfillment> fulfillments)
      {
         this.Fulfillments = fulfillments.ToImmutableList();
      }
   }
}
