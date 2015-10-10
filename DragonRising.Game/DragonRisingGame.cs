using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DragonRising.Views;
using DraconicEngine.Input;
using DragonRising.Storage;

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
         await RunGameState(loadDataState);

         var mainMenu = new MainMenuScreen(SaveManager.Current);
         await RunGameState(mainMenu);
      }
   }
}