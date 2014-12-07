using DraconicEngine.GameWorld.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   public class DecisionComponent : Component
   {
      public RogueAction ActionToDo { get; set; } = null;
   }
}
