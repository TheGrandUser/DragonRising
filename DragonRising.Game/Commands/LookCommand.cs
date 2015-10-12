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
      Entity player;
      ITerminal scenePanel;
      PlayerController playerController;

      public LookCommand(Entity player, ITerminal scenePanel, PlayerController playerController)
      {
         this.player = player;
         this.scenePanel = scenePanel;
         this.playerController = playerController;
      }

      public override async Task Do()
      {
         var lookTool = new LookTool(player.GetLocation(), scenePanel, playerController);
         
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
