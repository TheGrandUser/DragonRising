using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using DraconicEngine.EntitySystem;
using DragonRising.Commands.Requirements;
using DraconicEngine;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Components;
using DragonRising.Rules;

namespace DragonRising.GameWorld.Actions
{
   public class UseItemAction : ActionTaken
   {
      public Entity User { get; }
      public Entity Item { get; }
      public FinalizedPlan FinalizedPlan { get; }

      public UseItemAction(Some<Entity> user, Some<Entity> item, Some<FinalizedPlan> itemReqFulfillment)
      {
         User = user;
         this.FinalizedPlan = itemReqFulfillment;
         this.Item = item;
      }
   }

}
