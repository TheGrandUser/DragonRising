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
   public abstract class PlayingScreen : IGameView<Unit>
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

      public async Task<Unit> DoLogic()
      {
         while (true)
         {
            if (this.IsUnderPlayerControl())
            {
               while (true)
               {
                  var playerTurn = await this.GetPlayerTurn(IdleUpdate);

                  if (playerTurn == PlayerTurnResult.TurnAdvancing)
                  {
                     await Task.Delay(ActionUpdate);
                     break;
                  }
                  else if (playerTurn == PlayerTurnResult.Quit)
                  {
                     this.Save();
                     OnFinished();
                     return unit;
                  }
               }
            }
            else
            {
               await Task.Delay(IdleUpdate);
            }

            Engine.Update(1.0, UpdateTrack.Game);

            if (IsGameEndState())
            {

               await RogueGame.Current.RunGameState(CreateEndScreen());

               return unit;
            }
         }
      }

      protected virtual void Save()
      {
      }

      protected abstract bool IsGameEndState();

      protected virtual void PreSceneDraw()
      {
      }

      public void Draw()
      {
         PreSceneDraw();

         foreach (var widget in this.Widgets)
         {
            widget.Draw();
         }

         this.Engine.Update(1.0, UpdateTrack.Render);

         PostSceneDraw();
      }

      protected virtual void PostSceneDraw()
      {
      }

      protected abstract IGameView<Unit> CreateEndScreen();

      public GameViewType Type { get { return GameViewType.WholeScreen; } }

      public virtual void OnStart()
      {
      }
      
      protected virtual void OnFinished()
      {
      }
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
