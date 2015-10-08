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
   
   public interface IGameView
   {
      GameViewType Type { get; }

      Task<TickResult> DoLogic();
      Task Draw();
   }
}
