﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Behaviors;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.Effects;
using DragonRising.GameWorld.Components;

namespace DragonRising.GameWorld.Effects
{
   public class RemoveBehaviorEffect : IEffect
   {
      Behavior behavior;
      Entity target;

      public RemoveBehaviorEffect(Behavior behavior, Entity target)
      {
         this.behavior = behavior;
         this.target = target;
      }

      public void Do()
      {
         target.As<BehaviorComponent>(bc => bc.RemoveBehavior(behavior));
      }
   }
}