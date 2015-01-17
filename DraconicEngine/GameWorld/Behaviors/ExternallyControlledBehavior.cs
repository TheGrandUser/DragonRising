using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Input;
using Newtonsoft.Json;

namespace DraconicEngine.GameWorld.Behaviors
{
   public class ExternallyControlledBehavior : Behavior
   {
      [JsonIgnore]
      private RogueAction nextAction;

      public ExternallyControlledBehavior()
         : base("Externally Controlled")
      {
      }

      public void SetNextAction(RogueAction action)
      {
         this.nextAction = action;
      }

      public override RogueAction PlanTurn(Entity owner)
      {
         return nextAction ?? RogueAction.Idle;
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
