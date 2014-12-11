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
      [NonSerialized]
      RogueAction actionToDo = null;

      public RogueAction ActionToDo { get { return actionToDo; } set { actionToDo = value; } }
   }
}
