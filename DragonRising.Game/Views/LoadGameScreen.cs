using DraconicEngine;
using DraconicEngine.GameViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Terminals;
using DragonRising.Storage;
using LanguageExt;
using static LanguageExt.Prelude;
using DraconicEngine.Input;
using DraconicEngine.Terminals.Input;
using System.Threading;
using static DraconicEngine.Input.CommandGestureFactory;

namespace DragonRising.Views
{
   class LoadGameScreen : IGameView<string>
   {
      List<string> saveNames = new List<string>();
      int currentMenuItem = 0;
      private ITerminal optionsTerminal;

      public string message = "Please select a save game";

      public LoadGameScreen(ISaveManager saveManager)
      {
         this.saveNames = saveManager.GetSaveGames().ToList();

         var height = this.saveNames.Count + 3 * 2 + 3;
         var width = this.saveNames.StartWith(message)
            .Max(name => name.Length) + 3 * 2;

         var x = (DragonRisingGame.ScreenWidth - width) / 2;
         var y = 10;

         this.currentMenuItem = 0;

         this.optionsTerminal = DragonRisingGame.Current.RootTerminal[x, y, width, height];
      }
      public GameViewType Type { get { return GameViewType.WholeScreen; } }

      public string FilePath { get; private set; }

      enum MenuCommands
      {
         Up,
         Down,
         Select,
         Cancel,
      }

      CommandGesture upGesture = CreateGesture(MenuCommands.Up, GestureSet.Create(RogueKey.Up, RogueKey.NumPad8));
      CommandGesture downGesture = CreateGesture(MenuCommands.Down, GestureSet.Create(RogueKey.Down, RogueKey.NumPad2));
      CommandGesture selectGesture = CreateGesture(MenuCommands.Select, GestureSet.Create(RogueKey.Enter, RogueKey.Space));
      CommandGesture cancelGesture = CreateGesture(MenuCommands.Cancel, GestureSet.Create(RogueKey.Escape));
      
      IEnumerable<CommandGesture> Gestures
      {
         get
         {
            yield return upGesture;
            yield return downGesture;
            yield return selectGesture;
            yield return cancelGesture;
         }
      }
      
      public async Task<string> DoLogic()
      {
         while (true)
         {
            var result = await InputSystem.Current.GetCommandAsync(this.Gestures, CancellationToken.None);
            var command = result.Command as ValueCommand<MenuCommands>;

            if (command.Value == MenuCommands.Select)
            {
               if (currentMenuItem >= 0 && currentMenuItem < saveNames.Count)
               {
                  return saveNames[currentMenuItem];
               }
            }
            else if (command.Value == MenuCommands.Up)
            {
               var currentValidIndex = currentMenuItem;

               --currentValidIndex;
               if (currentValidIndex < 0)
               {
                  currentValidIndex = 0;
               }

               currentMenuItem = currentValidIndex;
            }
            else if (command.Value == MenuCommands.Down)
            {
               var currentValidIndex = currentMenuItem;

               ++currentValidIndex;
               if (currentValidIndex >= saveNames.Count)
               {
                  currentValidIndex = saveNames.Count - 1;
               }

               currentMenuItem = currentValidIndex;
            }
            else if (command.Value == MenuCommands.Cancel)
            {
               return null;
            }
         }
      }

      public void Draw()
      {
         this.optionsTerminal[RogueColors.LightGray].DrawBox(DrawBoxOptions.DoubleLines);

         this.optionsTerminal[3, 2][RogueColors.LightGray].Write("Please select a save game");

         for (int i = 0; i < this.saveNames.Count; i++)
         {
            var name = this.saveNames[i];
            var color = (currentMenuItem == i ? RogueColors.White : RogueColors.LightGray);
            this.optionsTerminal[3, 6 + i][color].Write(name);
         }
      }
   }
}
