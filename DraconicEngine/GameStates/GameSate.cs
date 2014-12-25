using LanguageExt;
using LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameStates
{
   public enum GameStateType
   {
      Screen,
      Dialog,
      Tool,
      Effect,
   }

   public enum TickResult
   {
      Continue,
      Finished,
   }

   public enum OutOfTurnEvent
   {
      RefreshDisplay,
      DoesNothing,
      CloseState,
   }

   public enum MouseButton
   {
      Left,
      Right,
      Middle,
   }

   public interface IGameState
   {
      GameStateType Type { get; }

      Task<TickResult> Tick();
      Task Draw();

      Option<IGameState> Finish();

      void Start();
   }

   public interface IGameState<TData> : IGameState
   {
      TData Result { get; }
   }

   public abstract class GameState : IGameState
   {
      public abstract GameStateType Type { get; }

      public abstract Task<TickResult> Tick();

      public abstract Task Draw();

      public virtual Option<IGameState> Finish()
      {
         return None;
      }

      public virtual void Start()
      {
      }
   }
}
