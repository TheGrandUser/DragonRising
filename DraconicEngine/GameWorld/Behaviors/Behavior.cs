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
   public abstract class Behavior
   {
      public abstract RogueAction PlanTurn(Entity owner);
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