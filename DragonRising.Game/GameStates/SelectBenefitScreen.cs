using DraconicEngine.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace DragonRising.GameStates
{
   public class SelectBenefitScreen : GameState
   {
      public override GameStateType Type => GameStateType.Screen;

      public override Task Draw()
      {

         return Task.FromResult(0);
      }

      public override Task<TickResult> Tick()
      {
         return Task.FromResult(TickResult.Finished);
      }
   }

   public enum Benefit
   {
      Constitution,
      Strength,
      Agility,
   }
}
