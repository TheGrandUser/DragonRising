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
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld;

namespace DragonRising.Rules.ExplorationRules
{
   //public class MoveInDirectionRule : Rule<MoveInDirectionAction>
   //{
   //   public override RuleResult Do(MoveInDirectionAction action)
   //   {
   //      var newLocation = action.Mover.Location + Vector.FromDirection(action.Dir);
   //      if (World.Current.Scene.IsBlocked(newLocation) == Blockage.None)
   //      {
   //         action.Mover.Location = newLocation;
   //      }

   //      return RuleResult.Empty;
   //   }
   //}

   public class MoveToRule : Rule<MoveToAction>
   {
      public override RuleResult Do(MoveToAction action, Scene scene)
      {
         if (World.Current.Scene.IsBlocked(action.NewLocation) == Blockage.None)
         {
            action.Mover.Location = action.NewLocation;
         }

         return RuleResult.Empty;
      }
   }
}