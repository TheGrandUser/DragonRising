﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.Terminals.Input;
using DraconicEngine;
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld;

namespace DragonRising.Commands
{
   public class TeleportCommand : ActionCommand
   {
      ActionRequirement requirement = new LocationRequirement();
      public override ActionRequirement GetRequirement(Entity user) => requirement;
      public override string Name => "Teleport";
      public TeleportCommand()
      {
      }

      public override Either<RogueAction, AlternateCommmand> PrepareAction(Entity executer, RequirementFulfillment fulfillment)
      {
         var destFulfillment = (LocationFulfillment)fulfillment;
         var destination = destFulfillment.Location;
         var scene = World.Current.Scene;

         var blockage = scene.IsBlocked(destination, ignoreWhere: entity => entity == executer);

         if (blockage != Blockage.None)
         {
            return RogueAction.Abort;
         }
         return new MoveToAction(destination);
      }
   }
}