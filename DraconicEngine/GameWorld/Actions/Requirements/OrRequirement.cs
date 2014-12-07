using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Actions.Requirements
{
   public class OrRequirement : ActionRequirement
   {
      public ActionRequirement Prefered { get; private set; }
      public ActionRequirement Alternate { get; private set; }
      public OrRequirement(ActionRequirement prefered, ActionRequirement alternate, string message = "")
      {
         this.Prefered = prefered;
         this.Alternate = alternate;
         this.Message = message;
      }
   }
}
