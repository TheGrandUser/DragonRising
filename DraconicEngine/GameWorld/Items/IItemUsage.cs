using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Items;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Items
{
   public interface IItemUsage
   {
      ActionRequirement Requirements { get; }
      bool Use(Entity user, Some<RequirementFulfillment> fulfillment);
   }
}
