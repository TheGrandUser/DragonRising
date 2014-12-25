using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Behaviors
{
   [Serializable]
   public abstract class Behavior
   {
      public abstract RogueAction PlanTurn(Entity owner);
      public Behavior Clone()
      {
         return CloneCore();
      }

      protected abstract Behavior CloneCore();

      protected Behavior()
      {
      }

      protected Behavior(Behavior original)
      {
         // Nothing to copy right now
      }
   }
}