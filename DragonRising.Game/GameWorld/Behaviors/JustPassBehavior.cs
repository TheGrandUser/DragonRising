using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Behaviors
{
   [Serializable]
   public class JustPassBehavior : Behavior
   {
      public override ActionTaken PlanTurn(Entity owner) => ActionTaken.Idle;

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
