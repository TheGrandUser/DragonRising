﻿using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Actions.Requirements
{
   public class AndRequirement : ActionRequirement
   {
      public ActionRequirement First { get; }
      public ActionRequirement Second { get; }
      public AndRequirement(ActionRequirement first, ActionRequirement second, string message = "")
      {
         this.First = first;
         this.Second = second;
         this.Message = message;
      }

      public override bool MeetsRequirement(RequirementFulfillment fulfillment)
      {
         var andFulfillment = fulfillment as AndFulfillment;
         if(andFulfillment != null)
         {
            return this.First.MeetsRequirement(andFulfillment.First) && this.Second.MeetsRequirement(andFulfillment.Second);
         }
         return false;
      }
   }

   public class AndFulfillment : RequirementFulfillment
   {
      public RequirementFulfillment First { get; }
      public RequirementFulfillment Second { get; }
      public AndFulfillment(RequirementFulfillment first, RequirementFulfillment second)
      {
         this.First = first;
         this.Second = second;
      }
   }
}
