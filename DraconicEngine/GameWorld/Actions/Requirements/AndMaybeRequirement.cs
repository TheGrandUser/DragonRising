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
      public override bool MeetsRequirement(RequirementFulfillment fulfillment)
      {
         if (fulfillment is AndMaybeFulfillment)
         {
            var andMaybe = (AndMaybeFulfillment)fulfillment;

            if (this.First.MeetsRequirement(andMaybe.First))
            {
               if (this.IsSecondRequired(andMaybe.First))
               {
                  return andMaybe.Second.Match(
                     Some: second => this.Second.MeetsRequirement(second),
                     None: () => false);
               }
               return true;
            }
         }
         return false;
      }
   }

   [Serializable]
   public abstract class MaybeCheck<TFirstFulfillment>
      where TFirstFulfillment : RequirementFulfillment
   {
      public abstract bool Check(TFirstFulfillment fulfillment);
   }

   public class AndMaybeRequirement<TFirstFulfillment> : AndMaybeRequirement
      where TFirstFulfillment : RequirementFulfillment
   {
      MaybeCheck<TFirstFulfillment> checker;

      public AndMaybeRequirement(ActionRequirement first, ActionRequirement second, MaybeCheck<TFirstFulfillment> checker, string message = "")
         : base(first, second, message)
      {
         this.checker = checker;
      }

      public override bool IsSecondRequired(RequirementFulfillment fulfillment)
      {
         return checker.Check((TFirstFulfillment)fulfillment);
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
