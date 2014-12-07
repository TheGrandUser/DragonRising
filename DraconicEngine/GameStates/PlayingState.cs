using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.EntitySystem.Systems;
using DraconicEngine.Input;
using DraconicEngine.Widgets;
using LanguageExt;
using LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameStates
{
   public abstract class PlayingState : IGameState
   {
      
      IPlayerController playerController;
      private IDisposable subscriptions;

      public IPlayerController PlayerController
      {
         get { return playerController; }
         set { playerController = value; }
      }

      public Scene Scene { get; set; }

      protected Engine Engine { get; } = new Engine();

      public List<Widget> Widgets { get; } = new List<Widget>();

      public PlayingState(Scene scene)
      {
         this.Scene = scene;

         this.Engine.AddSystem(new AIDecisionSystem(), 1);
         this.Engine.AddSystem(new CreatureActionSystem(), 2);
         this.Engine.AddSystem(new TimerSystem(), 3);

         this.subscriptions = this.Engine.ObserveStore(this.Scene.EntityStore);
      }

      public TimeSpan IdleUpdate { get; set; } = TimeSpan.FromSeconds(0.3);
      public TimeSpan ActionUpdate { get; set; } = TimeSpan.FromSeconds(0.05);

      public async Task<TickResult> Tick()
      {
         if (this.playerController != null)
         {
            var playerTurn = await this.playerController.CheckInputAsync();

            if (playerTurn == PlayerTurnResult.None)
            {
               return TickResult.Continue;
            }
            else if (playerTurn == PlayerTurnResult.RealTimeIdle)
            {
               await Task.Delay(IdleUpdate);
            }
            else if (playerTurn == PlayerTurnResult.Quit)
            {
               this.Save();
               return TickResult.Finished;
            }
            else
            {
               await Task.Delay(ActionUpdate);
            }
         }
         else
         {
            await Task.Delay(IdleUpdate);
         }

         await Engine.Update(1.0);

         if (IsGameEndState())
         {
            
            await RogueGame.Current.RunGameState(CreateEndScreen());

            return TickResult.Finished;
         }

         return TickResult.Continue;
      }

      protected virtual void Save()
      {
         //var filePath = "saveGame.drgame";


      }

      protected abstract bool IsGameEndState();

      protected virtual void PreSceneDraw()
      {
      }

      public void Draw()
      {
         RogueGame.Current.RootTerminal.Clear();

         PreSceneDraw();

         foreach (var widget in this.Widgets)
         {
            widget.Draw();
         }

         PostSceneDraw();
      }

      protected virtual void PostSceneDraw()
      {
      }

      protected abstract Some<IGameState> CreateEndScreen();

      public GameStateType Type { get { return GameStateType.Screen; } }

      public virtual void Start()
      {
         Current = this;
      }

      public Option<IGameState> Finish()
      {
         this.subscriptions.Dispose();
         Current = null;

         return OnFinished();
      }

      protected virtual Option<IGameState> OnFinished()
      {
         return None;
      }

      public static PlayingState Current { get; private set; }
   }
}
