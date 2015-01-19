using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.EntitySystem;
using DragonRising.GameWorld.Items;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Items
{
   public interface IItemUsage
   {
      ActionRequirement Requirements { get; }
      bool Use(Entity user, Some<RequirementFulfillment> fulfillment);
   }
}
