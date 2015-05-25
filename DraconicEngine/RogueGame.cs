using DraconicEngine.GameViews;
using DraconicEngine.Terminals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Diagnostics;

namespace DraconicEngine
{
   public abstract class RogueGame
   {
      public static readonly int ScreenWidth = 80;
      public static readonly int ScreenHeight = 36;

      public abstract Task Start();

      public Action Present { get; set; }

      Terminal rootTerminal = new Terminal(ScreenWidth, ScreenHeight);

      public Terminal RootTerminal
      {
         get { return rootTerminal; }
         protected set { rootTerminal = value; }
      }
      
      Stack<IGameView> gameStates = new Stack<IGameView>();

      public async Task RunGameState(Some<IGameView> gameState)
      {
         gameStates.Push(gameState.Value);
         gameStateAdded.OnNext(gameState.Value);

         Task drawTask = gameStates.Count == 1 ? StartDrawLoop() : Task.FromResult(0);

         gameState.Value.Start();

         await TurnLoop(gameState);

         var nextState = gameState.Value.Finish();
         gameStates.Pop();
         gameStateRemoved.OnNext(gameState.Value);


         await drawTask;
      }

      private bool ShowsPrior(GameViewType type)
      {
         return type == GameViewType.Dialog || type == GameViewType.Effect || type == GameViewType.Tool;
      }

      Subject<IGameView> gameStateAdded = new Subject<IGameView>();
      Subject<IGameView> gameStateRemoved = new Subject<IGameView>();

      Subject<Unit> drawStarted = new Subject<Unit>();
      Subject<Unit> drawFinished = new Subject<Unit>();



      protected async Task StartDrawLoop()
      {
         var watch = new Stopwatch();

         while (gameStates.Count > 0)
         {
            drawStarted.OnNext(unit);

            watch.Restart();

            var screen = gameStates.FirstOrDefault(state => state.Type == GameViewType.Screen);
            if (screen != null)
            {
               await screen.Draw();
            }
            foreach (var gameState in gameStates.TakeWhile(gs => ShowsPrior(gs.Type)).Reverse())
            {
               await gameState.Draw();
            }

            Present();
            drawFinished.OnNext(unit);

            watch.Stop();

            if (watch.Elapsed < frameTime)
            {
               await Task.Delay(frameTime - watch.Elapsed);
            }
         }
      }

      async Task TurnLoop(Some<IGameView> gameState)
      {
         while (true)
         {
            var result = await gameState.Value.Tick();
            
            if(result == TickResult.Finished)
            {
               break;
            }
            else
            {
               await drawFinished.FirstAsync();
            }
         }
      }
      
      static RogueGame game;
      public static RogueGame Current { get { return game; } }
      public static void SetCurrentGame(RogueGame game)
      {
         RogueGame.game = game;
      }

      static readonly TimeSpan frameTime = TimeSpan.FromSeconds(1.0 / 15.0);

      public IGameView CurrentGameState { get; set; }

      Random randomer = new Random();
      public Random GameRandom { get { return randomer; } }
   }
}
