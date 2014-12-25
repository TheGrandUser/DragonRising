using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.Items;

namespace DraconicEngine.Input
{
   public enum PlayerTurnResult
   {
      None,
      Idle,
      TurnAdvancing,
      Quit,
   }
   public interface IPlayerController
   {
      Task<PlayerTurnResult> GetInputAsync(TimeSpan timeout);
      RogueAction GetAction();
   }
}