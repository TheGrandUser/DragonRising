using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using DraconicEngine.EntitySystem;
using DraconicEngine;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Components;
using DragonRising.Rules;

namespace DragonRising.GameWorld.Actions
{
   public class MoveInDirectionAction : ActionTaken
   {
      public Entity Mover { get; }
      public Direction Dir { get; }

      public MoveInDirectionAction(Entity mover, Direction dir)
      {
         Mover = mover;
         this.Dir = dir;
      }
   }

   public class MoveToAction : ActionTaken
   {
      public Entity Mover { get; }
      public Loc NewLocation { get; }
      public MoveToAction(Entity mover, Loc newLocation)
      {
         Mover = mover;
         this.NewLocation = newLocation;
      }
   }

}
