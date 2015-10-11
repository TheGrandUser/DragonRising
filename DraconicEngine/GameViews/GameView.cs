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
      WholeScreen,
      PartialScreen,
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
   
   public interface IGameView<T>
   {
      Task<T> DoLogic();

      void Draw();
      GameViewType Type { get; }
   }

   public class DrawInfo
   {
      public DrawInfo(Action draw, GameViewType type)
      {
         Draw = draw;
         Type = type;
      }

      public Action Draw { get; }
      public GameViewType Type { get; }
   }
}
