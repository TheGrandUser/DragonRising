using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.Terminals;
using DragonRising.GameStates;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.Terminals.Input;

namespace DragonRising.Commands
{
   public class LookCommand : AsyncCommand
   {

      public LookCommand()
      {
      }

      public override async Task Do()
      {
         MyPlayingScreen playingState = MyPlayingScreen.Current;

         var lookTool = new LookTool(playingState.World.Scene.FocusEntity.GetLocation(), playingState.PlayerController);
         
         await RogueGame.Current.RunGameState(lookTool);
      }
   }

   public class LookAtCommand : RogueCommand
   {
      public Loc? Location { get; }

      public LookAtCommand(Loc? location)
      {
         this.Location = location;
      }
   }
}
