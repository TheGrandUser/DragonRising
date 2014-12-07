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
      IItemUsageTemplate Template { get; }
      ActionRequirement Requirements { get; }
      ItemUseResult PrepUse(Entity user, Some<RequirementFulfillment> fulfillment);
      void Use(Entity user);
   }
}
