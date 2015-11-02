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
   class MainMenuScreen : IGameView<Unit>
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

      public async Task<Unit> DoLogic()
      {
         while (true)
         {
            var command = await this.menu.Tick();

            var result = await command();

            if(result == TickResult.Finished)
            {
               return unit;
            }
         }
      }

      public void Draw()
      {
         this.menu.Draw();
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
         var newGameScreen = new NewGameScreen(new SceneGenerator());

         var world = await RogueGame.Current.RunGameState(newGameScreen);

         await world.MatchAsync(
            Some: w => RogueGame.Current.RunGameState(new MyPlayingScreen(w, saveManager)),
            None: () => unit);

         return TickResult.Continue;
      }

      async Task<TickResult> LoadGame()
      {
         var loadGameScreen = new LoadGameScreen(saveManager);

         var selectedGame = await RogueGame.Current.RunGameState(loadGameScreen);

         if (!string.IsNullOrEmpty(selectedGame))
         {
            await PlayLoadedGame(selectedGame);
         }

         return TickResult.Continue;
      }

      bool CanLoadGame() => saveManager.GetSaveGames().Any();

      Task<TickResult> Exit() { return Task.FromResult(TickResult.Finished); }

      async Task PlayLoadedGame(string gameName)
      {
         var world = await saveManager.LoadGame(gameName);

         var playingState = new MyPlayingScreen(world, saveManager);

         await RogueGame.Current.RunGameState(playingState);
      }
   }
}
