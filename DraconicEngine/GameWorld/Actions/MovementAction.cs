using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.Actions.Requirements;

namespace DraconicEngine.GameWorld.Actions
{
   public class MoveInDirectionAction : RogueAction
   {
      Direction dir;

      public MoveInDirectionAction(Direction dir)
      {
         this.dir = dir;
      }

      public override void Do(Entity executer)
      {
         var locComp = executer.GetComponent<LocationComponent>();
         var newLocation = locComp.Location + Vector.FromDirection(dir);
         if (Scene.CurrentScene.IsBlocked(newLocation) == Blockage.None)
         {
            locComp.Location = newLocation;
         }
      }
   }

   public class MoveToAction : RogueAction
   {
      Loc newLocation;
      public MoveToAction(Loc newLocation)
      {
         this.newLocation = newLocation;
      }

      public override void Do(Entity executer)
      {
         if (Scene.CurrentScene.IsBlocked(newLocation) == Blockage.None)
         {
            executer.GetComponent<LocationComponent>().Location = newLocation;
         }
      }
   }
}
