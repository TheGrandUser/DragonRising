using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Behaviors
{
   [Serializable]
   public class JustPassBehavior : Behavior
   {
      public override RogueAction PlanTurn(Entity owner) => RogueAction.Idle;

      public JustPassBehavior()
         : base("Just Pass")
      {

      }

      protected JustPassBehavior(JustPassBehavior original) : base(original)
      {

      }

      protected override Behavior CloneCore()
      {
         return new JustPassBehavior(this);
      }
   }
}
