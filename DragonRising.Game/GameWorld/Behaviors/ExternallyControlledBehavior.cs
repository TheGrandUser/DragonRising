using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.RulesSystem;
using DraconicEngine.EntitySystem;
using DraconicEngine.Input;
using Newtonsoft.Json;

namespace DragonRising.GameWorld.Behaviors
{
   public class ExternallyControlledBehavior : Behavior
   {
      [JsonIgnore]
      private ActionTaken nextAction;

      public ExternallyControlledBehavior()
         : base("Externally Controlled")
      {
      }

      public void SetNextAction(ActionTaken action)
      {
         this.nextAction = action;
      }

      public override ActionTaken PlanTurn(Entity owner)
      {
         return nextAction ?? ActionTaken.Idle;
      }

      protected ExternallyControlledBehavior(ExternallyControlledBehavior original)
         : base(original)
      {
      }

      protected override Behavior CloneCore()
      {
         return new ExternallyControlledBehavior(this);
      }
   }
}
