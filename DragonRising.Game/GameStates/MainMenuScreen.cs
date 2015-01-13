using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameStates;
using DraconicEngine.Generators;
using DragonRising.Generators;
using DraconicEngine.Input;
using DraconicEngine.Terminals;
using DraconicEngine.Input.CommandGestureFactory;
using DraconicEngine.Terminals.Input;
using DraconicEngine.Terminals.Input.Commands;
using LanguageExt;
using LanguageExt.Prelude;
using System.Threading;
using DraconicEngine.Storage;

namespace DragonRising.GameStates
{
   class MenuCommand
   {
      private Func<Task<TickResult>> command;
      private Func<bool> canExecute;

      public bool CanExecute()
      {
         return canExecute != null ? canExecute() : true;
      }

      public Task<TickResult> Execute()
      {
         return command();
      }

      public MenuCommand(string name, Func<Task<TickResult>> command, Func<bool> canExecute = null)
      {
         this.Name = name;
         this.command = command;
         this.canExecute = canExecute;
      }

      public string Name { get; private set; }
   }

   class MainMenuScreen : IGameState
   {
      int currentMenuItem = 0;

      List<MenuCommand> menuItems = new List<MenuCommand>();
      private ITerminal optionsTerminal;

      public MainMenuScreen()
      {
         this.menuItems.Add(new MenuCommand("Continue Game", this.ContinueGame, this.CanContinueGame));
         this.menuItems.Add(new MenuCommand("New Game", this.NewGame));
         this.menuItems.Add(new MenuCommand("Load Game", this.LoadGame, this.CanLoadGame));
         this.menuItems.Add(new MenuCommand("Exit", this.Exit));

         var height = this.menuItems.Count + 3 * 2;
         var width = this.menuItems.Max(mi => mi.Name.Length) + 3 * 2;

         var x = (DragonRisingGame.ScreenWidth - width) / 2;
         var y = DragonRisingGame.ScreenHeight - height - 5;

         this.currentMenuItem = menuItems.Select((mi, i) => new { mi, i }).First(mi => mi.mi.CanExecute()).i;

         this.optionsTerminal = DragonRisingGame.Current.RootTerminal[x, y, width, height];
      }

      public async Task<TickResult> Tick()
      {
         while (true)
         {
            var result = await InputSystem.Current.GetCommandAsync(this.Gestures, CancellationToken.None);
            var command = result.Command as ValueCommand<MenuCommands>;

            if (command.Value == MenuCommands.Select)
            {
               if (currentMenuItem >= 0 && currentMenuItem < menuItems.Count)
               {
                  if (menuItems[currentMenuItem].CanExecute())
                  {
                     return await menuItems[currentMenuItem].Execute();
                  }
               }
            }
            else if (command.Value == MenuCommands.Up)
            {
               var validItems = menuItems.Select((mi, i) => new { mi, i })
                  .Where(mi => mi.mi.CanExecute()).Select(me => me.i)
                  .ToList();

               var currentValidIndex = validItems.IndexOf(currentMenuItem);

               --currentValidIndex;
               if (currentValidIndex < 0)
               {
                  currentValidIndex = 0;
               }

               currentMenuItem = validItems[currentValidIndex];
               return TickResult.Continue;
            }
            else if (command.Value == MenuCommands.Down)
            {
               var validItems = menuItems.Select((mi, i) => new { mi, i })
                  .Where(mi => mi.mi.CanExecute()).Select(me => me.i)
                  .ToList();

               var currentValidIndex = validItems.IndexOf(currentMenuItem);

               ++currentValidIndex;
               if (currentValidIndex >= validItems.Count)
               {
                  currentValidIndex = validItems.Count - 1;
               }

               currentMenuItem = validItems[currentValidIndex];
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
         // Render the big title screen and the options;

         for (int i = 0; i < this.menuItems.Count; i++)
         {
            var menuItem = this.menuItems[i];
            var color = menuItem.CanExecute() ? (currentMenuItem == i ? RogueColors.White : RogueColors.LightGray) : RogueColors.Gray;
            this.optionsTerminal[3, 3 + i][color].Write(menuItem.Name);
         }

         return Task.FromResult(0);
      }

      public GameStateType Type { get { return GameStateType.Screen; } }

      async Task<TickResult> ContinueGame()
      {
         string lastSave = SaveManager.Current.LastSaveGame;
         var scene = await SaveManager.Current.LoadGame(lastSave);
         var playingState = new MyPlayingState(scene, lastSave);

         await RogueGame.Current.RunGameState(playingState);

         return TickResult.Continue;
      }

      bool CanContinueGame() => !string.IsNullOrEmpty(SaveManager.Current.LastSaveGame);

      async Task<TickResult> NewGame()
      {
         var newGameScreen = new NewGameScreen();

         await RogueGame.Current.RunGameState(newGameScreen);

         if (newGameScreen.Scene != null)
         {
            Scene scene = newGameScreen.Scene;
            var playingState = new MyPlayingState(scene, newGameScreen.GameName);

            await RogueGame.Current.RunGameState(playingState);
         }
         return TickResult.Continue;
      }

      async Task<TickResult> LoadGame()
      {
         var loadGameScreen = new LoadGameScreen();

         await RogueGame.Current.RunGameState(loadGameScreen);

         if (!string.IsNullOrEmpty(loadGameScreen.SelectedGame))
         {
            var scene = await SaveManager.Current.LoadGame(loadGameScreen.SelectedGame);

            var playingState = new MyPlayingState(scene, loadGameScreen.SelectedGame);

            await RogueGame.Current.RunGameState(playingState);
         }

         return TickResult.Continue;
      }

      bool CanLoadGame() => SaveManager.Current.GetSaveGames().Any();

      Task<TickResult> Exit() { return Task.FromResult(TickResult.Finished); }

      public void Start() { }

      public Option<IGameState> Finish() { return None; }

      enum MenuCommands
      {
         Up,
         Down,
         Select,
         MouseSelect,
         MousePoint,
      }

      CommandGesture upGesture = CreateGesture(MenuCommands.Up, GestureSet.Create(RogueKey.Up, RogueKey.NumPad8));
      CommandGesture downGesture = CreateGesture(MenuCommands.Down, GestureSet.Create(RogueKey.Down, RogueKey.NumPad2));
      CommandGesture selectGesture = CreateGesture(MenuCommands.Select, GestureSet.Create(RogueKey.Enter, RogueKey.Space));

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
   }
}
