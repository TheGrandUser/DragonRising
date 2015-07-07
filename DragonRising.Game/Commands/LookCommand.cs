using DraconicEngine;
using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using DraconicEngine.Terminals;
using DragonRising.Views;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.Commands.Requirements;
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
