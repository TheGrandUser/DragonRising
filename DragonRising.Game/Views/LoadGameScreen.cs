using DraconicEngine;
using DraconicEngine.GameViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Terminals;
using DragonRising.Storage;
using DragonRising.GameWorld.Items;
using LanguageExt;
using static LanguageExt.Prelude;
using DraconicEngine.Input;
using DraconicEngine.Terminals.Input;
using System.Threading;
using static DraconicEngine.Input.CommandGestureFactory;

namespace DragonRising.GameStates
{
   class LoadGameScreen : IGameView
   {
      List<string> saveNames = new List<string>();
      int currentMenuItem = 0;
      private ITerminal optionsTerminal;

      public string message = "Please select a save game";

      public LoadGameScreen()
      {
         this.saveNames = SaveManager.Current.GetSaveGames().ToList();

         var height = this.saveNames.Count + 3 * 2 + 3;
         var width = this.saveNames.StartWith(message)
            .Max(name => name.Length) + 3 * 2;

         var x = (DragonRisingGame.ScreenWidth - width) / 2;
         var y = 10;

         this.currentMenuItem = 0;

         this.optionsTerminal = DragonRisingGame.Current.RootTerminal[x, y, width, height];
      }
      public GameViewType Type { get { return GameViewType.Screen; } }

      public string FilePath { get; private set; }

      enum MenuCommands
      {
         Up,
         Down,
         Select,
         Cancel,
         MouseSelect,
         MousePoint,
      }

      CommandGesture upGesture = CreateGesture(MenuCommands.Up, GestureSet.Create(RogueKey.Up, RogueKey.NumPad8));
      CommandGesture downGesture = CreateGesture(MenuCommands.Down, GestureSet.Create(RogueKey.Down, RogueKey.NumPad2));
      CommandGesture selectGesture = CreateGesture(MenuCommands.Select, GestureSet.Create(RogueKey.Enter, RogueKey.Space));
      CommandGesture cancelGesture = CreateGesture(MenuCommands.Select, GestureSet.Create(RogueKey.Escape));

      CommandGesture mouseSelectGesture = CreateGesture(MenuCommands.MouseSelect, GestureSet.Create(RogueMouseAction.LeftClick));
      CommandGesture mousePointGesture = CreateGesture(MenuCommands.MousePoint, GestureSet.Create(RogueMouseAction.Movement));

      IEnumerable<CommandGesture> Gestures
      {
         get
         {
            yield return upGesture;
            yield return downGesture;
            yield return selectGesture;
            //yield return mouseSelectGesture;
            //yield return mousePointGesture;
         }
      }

      public string SelectedGame { get; set; }

      public async Task<TickResult> Tick()
      {
         while (true)
         {
            var result = await InputSystem.Current.GetCommandAsync(this.Gestures, CancellationToken.None);
            var command = result.Command as ValueCommand<MenuCommands>;

            if (command.Value == MenuCommands.Select)
            {
               if (currentMenuItem >= 0 && currentMenuItem < saveNames.Count)
               {
                  this.SelectedGame = saveNames[currentMenuItem];

                  return TickResult.Finished;
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
               return TickResult.Continue;
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
               return TickResult.Continue;
            }
            else if (command.Value == MenuCommands.MouseSelect)
            {
               return TickResult.Continue;
            }
            else if (command.Value == MenuCommands.MousePoint)
            {
               return TickResult.Continue;
            }
         }
      }

      public Task Draw()
      {
         RogueGame.Current.RootTerminal.Clear();

         this.optionsTerminal[RogueColors.LightGray].DrawBox(DrawBoxOptions.DoubleLines);

         this.optionsTerminal[3, 2][RogueColors.LightGray].Write("Please select a save game");

         for (int i = 0; i < this.saveNames.Count; i++)
         {
            var name = this.saveNames[i];
            var color = (currentMenuItem == i ? RogueColors.White : RogueColors.LightGray);
            this.optionsTerminal[3, 6 + i][color].Write(name);
         }

         return Task.FromResult(0);
      }

      public Option<IGameView> Finish()
      {
         return None;
      }

      public void Start()
      {
      }


   }
}
