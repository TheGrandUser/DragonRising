using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.EntitySystem;
using DraconicEngine.GameViews;
using DragonRising.Generators;
using DraconicEngine.Input;
using DraconicEngine.Terminals;
using static DraconicEngine.Input.CommandGestureFactory;
using DraconicEngine.Terminals.Input;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading;
using DragonRising.Storage;
using DragonRising.Widgets;

namespace DragonRising.Views
{
   class MainMenuScreen : IGameView
   {
      ISaveManager saveManager;
      delegate Task<TickResult> MenuCommand();

      MenuWidget<MenuCommand> menu;

      public MainMenuScreen(ISaveManager saveManager)
      {
         this.saveManager = saveManager;

         var menuItems = new[] {
            new MenuItem<MenuCommand>("Continue Game", this.ContinueGame, this.CanContinueGame),
            new MenuItem<MenuCommand>("New Game", this.NewGame),
            new MenuItem<MenuCommand>("Load Game", this.LoadGame, this.CanLoadGame),
            new MenuItem<MenuCommand>("Exit", this.Exit)};

         var height = menuItems.Length + 3 * 2 + 1;
         var width = menuItems.Max(mi => mi.Name.Length) + 3 * 2;

         var x = (DragonRisingGame.ScreenWidth - width) / 2;
         var y = DragonRisingGame.ScreenHeight - height - 5;

         menu = new MenuWidget<MenuCommand>(DragonRisingGame.Current.RootTerminal[x, y, width, height], "", false, menuItems);
      }

      public async Task<TickResult> DoLogic()
      {
         var result = await this.menu.Tick();

         return await result.Match(
            Some: async command =>
            {
               await command();
               return TickResult.Finished;
            },
            None: () => Task.FromResult(TickResult.Continue));
      }

      public Task Draw()
      {
         RogueGame.Current.RootTerminal.Clear();

         this.menu.Draw();

         return Task.FromResult(0);
      }

      public GameViewType Type { get { return GameViewType.WholeScreen; } }

      async Task<TickResult> ContinueGame()
      {
         string lastSave = saveManager.LastSaveGame;
         await PlayLoadedGame(lastSave);

         return TickResult.Continue;
      }

      bool CanContinueGame() => !string.IsNullOrEmpty(saveManager.LastSaveGame);

      async Task<TickResult> NewGame()
      {
         var newGameScreen = new NewGameScreen();

         await RogueGame.Current.RunGameState(newGameScreen);

         if (newGameScreen.World != null)
         {
            var world = newGameScreen.World;
            var playingState = new MyPlayingScreen(world, newGameScreen.GameName, saveManager);

            await RogueGame.Current.RunGameState(playingState);
         }
         return TickResult.Continue;
      }

      async Task<TickResult> LoadGame()
      {
         var loadGameScreen = new LoadGameScreen(saveManager);

         await RogueGame.Current.RunGameState(loadGameScreen);

         if (!string.IsNullOrEmpty(loadGameScreen.SelectedGame))
         {
            await PlayLoadedGame(loadGameScreen.SelectedGame);
         }

         return TickResult.Continue;
      }

      bool CanLoadGame() => saveManager.GetSaveGames().Any();

      Task<TickResult> Exit() { return Task.FromResult(TickResult.Finished); }

      async Task PlayLoadedGame(string gameName)
      {
         var world = await saveManager.LoadGame(gameName);
         world.Scene.ClearFoV();

         var playingState = new MyPlayingScreen(world, gameName, saveManager);

         await RogueGame.Current.RunGameState(playingState);
      }
      
      //enum MenuCommands
      //{
      //   Up,
      //   Down,
      //   Select,
      //   MouseSelect,
      //   MousePoint,
      //}

      //CommandGesture upGesture = CreateGesture(MenuCommands.Up, GestureSet.Create(RogueKey.Up, RogueKey.NumPad8));
      //CommandGesture downGesture = CreateGesture(MenuCommands.Down, GestureSet.Create(RogueKey.Down, RogueKey.NumPad2));
      //CommandGesture selectGesture = CreateGesture(MenuCommands.Select, GestureSet.Create(RogueKey.Enter, RogueKey.Space));

      //CommandGesture mouseSelectGesture = CreateGesture(MenuCommands.MouseSelect, GestureSet.Create(RogueMouseAction.LeftClick));
      //CommandGesture mousePointGesture = CreateGesture(MenuCommands.MousePoint, GestureSet.Create(RogueMouseAction.Movement));

      //IEnumerable<CommandGesture> Gestures
      //{
      //   get
      //   {
      //      yield return upGesture;
      //      yield return downGesture;
      //      yield return selectGesture;
      //      //yield return mouseSelectGesture;
      //      //yield return mousePointGesture;
      //   }
      //}
   }
}
