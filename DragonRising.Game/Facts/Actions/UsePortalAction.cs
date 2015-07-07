using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.EntitySystem;
using DragonRising.Rules;

namespace DragonRising.GameWorld.Actions
{
   public class UsePortalAction : ActionTaken
   {
      public Entity User { get; }
      public UsePortalAction(Entity user)
      {
         User = user;
      }
   }

}
