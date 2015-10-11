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
         public DrawInfo DrawInfo { get; set; }
         public TaskCompletionSource<Unit> DrawFinished { get; set; }
      }

      ActionBlock<DrawMessage> drawAgent;
      class DrawMessageHandler
      {
         Stack<DrawInfo> gameStates = new Stack<DrawInfo>();
         IObserver<Unit> drawStarted;
         IObserver<Unit> drawFinished;

         public DrawMessageHandler(IObserver<Unit> drawStarted, IObserver<Unit> drawFinished)
         {
            this.drawStarted = drawStarted;
            this.drawFinished = drawFinished;
         }

         public void HandleMessage(DrawMessage msg)
         {
            if (msg.Message == "Draw")
            {
               drawStarted.OnNext(unit);

               RogueGame.Current.RootTerminal.Clear();
               
               var screen = gameStates.FirstOrDefault(state => state.Type == GameViewType.WholeScreen);
               if (screen != null)
               {
                  screen.Draw();
               }
               foreach (var gameState in gameStates.TakeWhile(gs => gs.Type == GameViewType.PartialScreen).Reverse())
               {
                  gameState.Draw();
               }
               
               msg.DrawFinished.SetResult(unit);
               drawFinished.OnNext(unit);
            }
            else if (msg.Message == "Push")
            {
               gameStates.Push(msg.DrawInfo);
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

      Stack<DrawInfo> gameStates = new Stack<DrawInfo>();
      
      Subject<Unit> drawStarted = new Subject<Unit>();
      Subject<Unit> drawFinished = new Subject<Unit>();

      CancellationTokenSource cts = new CancellationTokenSource();

      public RogueGame()
      {
         var messageHandler = new DrawMessageHandler(drawStarted, drawFinished);
         
         this.drawAgent = new ActionBlock<DrawMessage>(new Action<DrawMessage>(messageHandler.HandleMessage));


      }

      public async Task<T> RunGameState<T>(IGameView<T> gameState)
      {
         var drawInfo = new DrawInfo(gameState.Draw, gameState.Type);
         gameStates.Push(drawInfo);

         await drawAgent.SendAsync(new DrawMessage() { Message = "Push", DrawInfo = drawInfo });

         if (gameStates.Count == 1)
         {
            StartDrawLoop(cts.Token);
         }

         var result = await TurnLoop(gameState);

         await drawAgent.SendAsync(new DrawMessage() { Message = "Pop", DrawInfo = drawInfo });

         gameStates.Pop();

         if (gameStates.Count == 0)
         {
            cts.Cancel();
            cts = new CancellationTokenSource();
         }

         return result;
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

               await drawAgent.SendAsync(new DrawMessage() { Message = "Draw", DrawFinished = drawFinish });
               await drawFinish.Task;

               Present();

               watch.Stop();

               if (watch.Elapsed < frameTime)
               {
                  await Task.Delay(frameTime - watch.Elapsed);
               }
            }
         }
         catch (TaskCanceledException)
         {
         }
      }

      async Task<T> TurnLoop<T>(IGameView<T> gameState)
      {
         var result = await gameState.DoLogic();
         await drawFinished.FirstAsync();
         return result;
      }

      static RogueGame game;
      public static RogueGame Current { get { return game; } }
      public static void SetCurrentGame(RogueGame game)
      {
         RogueGame.game = game;
      }

      static readonly TimeSpan frameTime = TimeSpan.FromSeconds(1.0 / 15.0);
      
      Random randomer = new Random();
      public Random GameRandom { get { return randomer; } }
   }
}
