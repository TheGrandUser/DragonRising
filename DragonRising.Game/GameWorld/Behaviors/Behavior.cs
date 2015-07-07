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
   public abstract class Behavior
   {
      public abstract ActionTaken PlanTurn(Entity owner);
      public Behavior Clone()
      {
         return CloneCore();
      }

      protected abstract Behavior CloneCore();

      protected Behavior(string name)
      {
         this.Name = name;
      }

      protected Behavior(Behavior original)
      {
         this.Name = original.Name;
      }

      public string Name { get; set; }
   }
}