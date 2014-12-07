using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Actions.Requirements
{
   public abstract class AndMaybeRequirement : ActionRequirement
   {
      public ActionRequirement First { get; }
      public ActionRequirement Second { get; }
      protected AndMaybeRequirement(ActionRequirement first, ActionRequirement second, string message = "")
      {
         this.First = first;
         this.Second = second;
         this.Message = message;
      }

      public abstract bool IsSecondRequired(RequirementFulfillment fulfillment);
   }

   public class AndMaybeRequirement<TFirstFulfillment> : AndMaybeRequirement
      where TFirstFulfillment : RequirementFulfillment
   {
      Func<TFirstFulfillment, bool> predicate;

      public AndMaybeRequirement(ActionRequirement first, ActionRequirement second, Func<TFirstFulfillment, bool> predicate, string message = "")
         : base(first, second, message)
      {
         this.predicate = predicate;
      }

      public override bool IsSecondRequired(RequirementFulfillment fulfillment)
      {
         return predicate((TFirstFulfillment)fulfillment);
      }
   }

   public class AndMaybeFulfillment : RequirementFulfillment
   {
      public Some<RequirementFulfillment> First { get; }
      public Option<RequirementFulfillment> Second { get; }
      public AndMaybeFulfillment(Some<RequirementFulfillment> first, Option<RequirementFulfillment> second)
      {
         this.First = first;
         this.Second = second;
      }
   }
}
