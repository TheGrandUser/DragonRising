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
            var result = await InputSystem.Current.GetCommandAsync(this.Gestures);
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

      public void Draw()
      {
         RogueGame.Current.RootTerminal.Clear();
         // Render the big title screen and the options;

         for (int i = 0; i < this.menuItems.Count; i++)
         {
            var menuItem = this.menuItems[i];
            var color = menuItem.CanExecute() ? (currentMenuItem == i ? RogueColors.White : RogueColors.LightGray) : RogueColors.Gray;
            this.optionsTerminal[3, 3 + i][color].Write(menuItem.Name);
         }
      }

      public GameStateType Type { get { return GameStateType.Screen; } }

      async Task<TickResult> ContinueGame()
      {
         var playingState = MyPlayingState.Load("");

         await RogueGame.Current.RunGameState(playingState);

         return TickResult.Continue;
      }

      bool CanContinueGame() { return false; }

      async Task<TickResult> NewGame()
      {
         

         var playingState = MyPlayingState.CreateNew();

         await RogueGame.Current.RunGameState(playingState);

         return TickResult.Continue;
      }

      async Task<TickResult> LoadGame()
      {
         var loadGameScreen = new LoadGameScreen();

         await RogueGame.Current.RunGameState(loadGameScreen);

         if (!string.IsNullOrEmpty(loadGameScreen.FilePath))
         {
            var playingState = MyPlayingState.Load(loadGameScreen.FilePath);

            await RogueGame.Current.RunGameState(playingState);
         }

         return TickResult.Continue;
      }

      bool CanLoadGame() { return false; }

      Task<TickResult> Exit() { return Task.FromResult(TickResult.Finished); }

      public void Start()
      {
      }

      public Option<IGameState> Finish()
      {
         return None;
      }

      enum MenuCommands
      {
         Up,
         Down,
         Select,
         MouseSelect,
         MousePoint,
      }

      CommandGesture upGesture = Create(MenuCommands.Up, GestureSet.Create(RogueKey.Up, RogueKey.NumPad8));
      CommandGesture downGesture = Create(MenuCommands.Down, GestureSet.Create(RogueKey.Down, RogueKey.NumPad2));
      CommandGesture selectGesture = Create(MenuCommands.Select, GestureSet.Create(RogueKey.Enter, RogueKey.Space));

      CommandGesture mouseSelectGesture = Create(MenuCommands.MouseSelect, GestureSet.Create(RogueMouseAction.LeftClick));
      CommandGesture mousePointGesture = Create(MenuCommands.MousePoint, GestureSet.Create(RogueMouseAction.Movement));

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
