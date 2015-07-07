using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Conditions
{
   class ConfusedCondition : ICondition
   {
      public string Name => "Confused";

      public string AppliedMessage(Entity target)  => $"The eyes of the {target.Name} look vacant, as he starts to stumble around.";
      public string RemovedMessage(Entity target) => $"The {target.Name} is no longer confused.";

      ConfusedCondition()
      {
      }

      static readonly ConfusedCondition instance = new ConfusedCondition();
      public static ConfusedCondition Instance => instance;
   }
}
