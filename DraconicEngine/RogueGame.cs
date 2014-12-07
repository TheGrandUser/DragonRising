using DraconicEngine.GameStates;
using DraconicEngine.Terminals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Prelude;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

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
      
      Stack<IGameState> gameStates = new Stack<IGameState>();

      public async Task RunGameState(Some<IGameState> gameState)
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

      private bool ShowsPrior(GameStateType type)
      {
         return type == GameStateType.Dialog || type == GameStateType.Effect || type == GameStateType.Tool;
      }

      Subject<IGameState> gameStateAdded = new Subject<IGameState>();
      Subject<IGameState> gameStateRemoved = new Subject<IGameState>();

      Subject<Unit> drawStarted = new Subject<Unit>();
      Subject<Unit> drawFinished = new Subject<Unit>();



      protected async Task StartDrawLoop()
      {
         while (gameStates.Count > 0)
         {
            drawStarted.OnNext(unit);
            var screen = gameStates.FirstOrDefault(state => state.Type == GameStateType.Screen);
            if (screen != null)
            {
               screen.Draw();
            }
            foreach (var gameState in gameStates.TakeWhile(gs => ShowsPrior(gs.Type)).Reverse())
            {
               gameState.Draw();
            }

            Present();
            drawFinished.OnNext(unit);
            await Task.Delay(frameTime);
         }
      }

      async Task TurnLoop(Some<IGameState> gameState)
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

      public IGameState CurrentGameState { get; set; }

      Random randomer = new Random();
      public Random GameRandom { get { return randomer; } }
   }
}
