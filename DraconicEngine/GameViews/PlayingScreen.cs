using DraconicEngine.RulesSystem;
using DraconicEngine.EntitySystem;
using DraconicEngine.Input;
using DraconicEngine.Widgets;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameViews
{
   public abstract class PlayingScreen : IGameView
   {
      protected abstract EntityEngine Engine { get; }

      public List<Widget> Widgets { get; } = new List<Widget>();

      public PlayingScreen()
      {
      }

      public TimeSpan IdleUpdate { get; set; } = TimeSpan.FromSeconds(0.3);
      public TimeSpan ActionUpdate { get; set; } = TimeSpan.FromSeconds(1.0 / 30.0);

      protected abstract bool IsUnderPlayerControl();
      protected abstract Task<PlayerTurnResult> GetPlayerTurn(TimeSpan timeout);

      public abstract void AddAsyncInterruption(IAsyncInterruption interruption);

      public async Task<TickResult> Tick()
      {
         if (this.IsUnderPlayerControl())
         {
            var playerTurn = await this.GetPlayerTurn(IdleUpdate);

            if (playerTurn == PlayerTurnResult.None)
            {
               return TickResult.Continue;
            }
            else if (playerTurn == PlayerTurnResult.TurnAdvancing)
            {
               await Task.Delay(ActionUpdate);
            }
            else if (playerTurn == PlayerTurnResult.Quit)
            {
               this.Save();
               return TickResult.Finished;
            }
            else // Idle, already timed out
            {
            }
         }
         else
         {
            await Task.Delay(IdleUpdate);
         }

         await Engine.Update(1.0, UpdateTrack.Game);

         if (IsGameEndState())
         {

            await RogueGame.Current.RunGameState(CreateEndScreen());

            return TickResult.Finished;
         }

         return TickResult.Continue;
      }

      protected virtual void Save()
      {
      }

      protected abstract bool IsGameEndState();

      protected virtual void PreSceneDraw()
      {
      }

      public async Task Draw()
      {
         RogueGame.Current.RootTerminal.Clear();

         PreSceneDraw();

         foreach (var widget in this.Widgets)
         {
            widget.Draw();
         }

         await this.Engine.Update(1.0, UpdateTrack.Render);

         PostSceneDraw();
      }

      protected virtual void PostSceneDraw()
      {
      }

      protected abstract Some<IGameView> CreateEndScreen();

      public GameViewType Type { get { return GameViewType.Screen; } }

      public virtual void Start()
      {
         Current = this;
      }

      public Option<IGameView> Finish()
      {
         Current = null;

         return OnFinished();
      }

      protected virtual Option<IGameView> OnFinished()
      {
         return None;
      }

      public static PlayingScreen Current { get; private set; }
   }

   public interface IAsyncInterruption
   {
      bool StillApplies();
      Task Run();
   }

   public class DelegateAsyncInterruption : IAsyncInterruption
   {
      Func<bool> stillApplies;
      Func<Task> run;

      public DelegateAsyncInterruption(Func<Task> run, Func<bool> stillApplies = null)
      {
         this.run = run;
         this.stillApplies = stillApplies;
      }

      public Task Run()
      {
         return run();
      }

      public bool StillApplies()
      {
         return stillApplies?.Invoke() ?? true;
      }
   }

   public static class PlayingStateExtensions
   {
      public static void AddAsyncInterruption(this PlayingScreen playingState, Func<Task> run, Func<bool> stillApplies = null)
      {
         playingState.AddAsyncInterruption(new DelegateAsyncInterruption(run, stillApplies));
      }
   }
}
