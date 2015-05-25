using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameViews
{
   public enum GameViewType
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

   public interface IGameView
   {
      GameViewType Type { get; }

      Task<TickResult> Tick();
      Task Draw();

      Option<IGameView> Finish();

      void Start();
   }

   public interface IGameView<TData> : IGameView
   {
      TData Result { get; }
   }

   public abstract class GameView : IGameView
   {
      public abstract GameViewType Type { get; }

      public abstract Task<TickResult> Tick();

      public abstract Task Draw();

      public virtual Option<IGameView> Finish()
      {
         return None;
      }

      public virtual void Start()
      {
      }
   }
}
