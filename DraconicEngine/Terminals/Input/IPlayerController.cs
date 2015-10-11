using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.RulesSystem;

namespace DraconicEngine.Input
{
   public enum PlayerTurnResult
   {
      None,
      TurnAdvancing,
      Quit,
   }
   public interface IPlayerController
   {
      Task<PlayerTurnResult> GetInputAsync(TimeSpan timeout);
   }
}