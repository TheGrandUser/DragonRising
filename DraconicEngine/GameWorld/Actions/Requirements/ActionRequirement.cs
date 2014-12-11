﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Actions.Requirements
{
   [Serializable]
   public abstract class ActionRequirement
   {
      public string Message { get; protected set; }

      public abstract bool MeetsRequirement(RequirementFulfillment fulfillment);
   }

   public abstract class RequirementFulfillment
   {
   }

   public class NoRequirement : ActionRequirement
   {
      private NoRequirement()
      {
         this.Message = string.Empty;
      }

      public static readonly NoRequirement None = new NoRequirement();

      public override bool MeetsRequirement(RequirementFulfillment fulfillment) => true;
   }

   public class NoFulfillment : RequirementFulfillment
   {
      private NoFulfillment() { }

      public static readonly NoFulfillment None = new NoFulfillment();
   }
}