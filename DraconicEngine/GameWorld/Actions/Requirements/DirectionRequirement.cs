using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Actions.Requirements
{
   public class DirectionRequirement : ActionRequirement
   {
      public bool CardinalOnly { get; set; }
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
