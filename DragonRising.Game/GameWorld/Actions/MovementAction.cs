using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine;
using DraconicEngine.GameWorld;
using DraconicEngine.GameWorld.Actions;
using DragonRising.GameWorld.Components;

namespace DragonRising.GameWorld.Actions
{
   public class MoveInDirectionAction : RogueAction
   {
      public Entity Mover { get; }
      public Direction Dir { get; }

      public MoveInDirectionAction(Entity mover, Direction dir)
      {
         Mover = mover;
         this.Dir = dir;
      }
   }

   public class MoveToAction : RogueAction
   {
      public Entity Mover { get; }
      public Loc NewLocation { get; }
      public MoveToAction(Entity mover, Loc newLocation)
      {
         Mover = mover;
         this.NewLocation = newLocation;
      }
   }

   public class MoveInDirectionRule : IActionRule<MoveInDirectionAction>
   {
      public void Apply(MoveInDirectionAction action)
      {
         var locComp = action.Mover.GetComponent<LocationComponent>();
         var newLocation = locComp.Location + Vector.FromDirection(action.Dir);
         if (World.Current.Scene.IsBlocked(newLocation) == Blockage.None)
         {
            locComp.Location = newLocation;
         }
      }
   }

   public class MoveToRule : IActionRule<MoveToAction>
   {
      public void Apply(MoveToAction action)
      {
         if (World.Current.Scene.IsBlocked(action.NewLocation) == Blockage.None)
         {
            action.Mover.GetComponent<LocationComponent>().Location = action.NewLocation;
         }
      }
   }
}
