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
using System.Threading.Tasks.Dataflow;
using System.Threading;

namespace DraconicEngine
{
   public abstract class RogueGame
   {
      class DrawMessage
      {
         public string Message { get; set; }
         public IGameView View { get; set; }
         public TaskCompletionSource<Unit> DrawFinished { get; set; }
      }

      ActionBlock<DrawMessage> drawAgent;
      class DrawMessageHandler
      {
         Stack<IGameView> gameStates = new Stack<IGameView>();
         IObserver<Unit> drawStarted;
         IObserver<Unit> drawFinished;

         public DrawMessageHandler(IObserver<Unit> drawStarted, IObserver<Unit> drawFinished)
         {
            this.drawStarted = drawStarted;
            this.drawFinished = drawFinished;
         }

         public async Task HandleMessage(DrawMessage msg)
         {
            if (msg.Message == "Draw")
            {
               drawStarted.OnNext(unit);

               var screen = gameStates.FirstOrDefault(state => state.Type == GameViewType.WholeScreen);
               if (screen != null)
               {
                  await screen.Draw();
               }
               foreach (var gameState in gameStates.TakeWhile(gs => gs.Type == GameViewType.PartialScreen).Reverse())
               {
                  await gameState.Draw();
               }

               Debug.WriteLine("Draw finished");
               msg.DrawFinished.SetResult(unit);
               drawFinished.OnNext(unit);
            }
            else if (msg.Message == "Push")
            {
               gameStates.Push(msg.View);
            }
            else if (msg.Message == "Pop")
            {
               gameStates.Pop();
            }
         }
      }

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

      Subject<IGameView> gameStateAdded = new Subject<IGameView>();
      Subject<IGameView> gameStateRemoved = new Subject<IGameView>();

      Subject<Unit> drawStarted = new Subject<Unit>();
      Subject<Unit> drawFinished = new Subject<Unit>();

      CancellationTokenSource cts = new CancellationTokenSource();

      public RogueGame()
      {
         var messageHandler = new DrawMessageHandler(drawStarted, drawFinished);
         this.drawAgent = new ActionBlock<DrawMessage>(messageHandler.HandleMessage);


      }

      public async Task RunGameState(Some<IGameView> gameState)
      {
         gameStates.Push(gameState.Value);
         gameStateAdded.OnNext(gameState.Value);

         await drawAgent.SendAsync(new DrawMessage() { Message = "Push", View = gameState.Value });

         if (gameStates.Count == 1)
         {
            StartDrawLoop(cts.Token);
         }

         await TurnLoop(gameState);

         await drawAgent.SendAsync(new DrawMessage() { Message = "Pop", View = gameState.Value });

         gameStates.Pop();
         gameStateRemoved.OnNext(gameState.Value);

         if (gameStates.Count == 0)
         {
            cts.Cancel();
            cts = new CancellationTokenSource();
         }
      }

      protected async void StartDrawLoop(CancellationToken ct)
      {
         try
         {
            var watch = new Stopwatch();

            while (!ct.IsCancellationRequested)
            {
               watch.Restart();

               var drawFinish = new TaskCompletionSource<Unit>();

               await drawAgent.SendAsync(new DrawMessage() { Message = "Draw", DrawFinished = drawFinish }, ct);
               await drawFinish.Task;
               Debug.WriteLine("Draw finished, now presenting");

               Present();

               watch.Stop();

               if (watch.Elapsed < frameTime)
               {
                  await Task.Delay(frameTime - watch.Elapsed, ct);
               }
            }
         }
         catch (TaskCanceledException)
         {
         }
      }

      async Task TurnLoop(Some<IGameView> gameState)
      {
         while (true)
         {
            var result = await gameState.Value.DoLogic();

            if (result == TickResult.Finished)
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
