using DraconicEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Commands.Requirements
{
   public class DirectionRequirement : PlanRequirement
   {
      public DirectionLimit Limits { get; set; }

      public override bool MeetsRequirement(RequirementFulfillment fulfillment)
      {
         return fulfillment is DirectionFulfillment;
      }
   }

   public class DirectionFulfillment : RequirementFulfillment
   {
      public Direction Direction { get; }
      public DirectionFulfillment(Direction direction)
      {
         this.Direction = direction;
      }
   }
}
