using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DragonRising.Views;
using DraconicEngine.Input;

namespace DragonRising
{
   public class DragonRisingGame : RogueGame
   {
      public DragonRisingGame()
      {
      }

      public override async Task Start()
      {
         var loadDataState = new LoadGeneralDataScreen();
         await RogueGame.Current.RunGameState(loadDataState);

         var mainMenu = new MainMenuScreen();
         await RogueGame.Current.RunGameState(mainMenu);
      }
   }
}