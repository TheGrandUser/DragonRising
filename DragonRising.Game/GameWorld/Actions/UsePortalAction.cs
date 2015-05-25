using DraconicEngine.GameWorld.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld;
using DraconicEngine.GameWorld.EntitySystem;

namespace DragonRising.GameWorld.Actions
{
   public class UsePortalAction : RogueAction
   {
      public Entity User { get; }
      public UsePortalAction(Entity user)
      {
         User = user;
      }
   }

   public class BaseUsePortalRule : IActionRule<UsePortalAction>
   {
      public void Apply(UsePortalAction action)
      {
      }
   }
}
