using DraconicEngine;
using DraconicEngine.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Terminals;
using DraconicEngine.Storage;
using DraconicEngine.Items;
using LanguageExt;
using LanguageExt.Prelude;

namespace DragonRising.GameStates
{
   class LoadGameScreen : IGameState
   {
      public LoadGameScreen()
      {
      }
      public GameStateType Type { get { return GameStateType.Screen; } }

      public string FilePath { get; private set; }

      public Task<TickResult> Tick()
      {
         throw new NotImplementedException();
      }

      public Task Draw()
      {
         return Task.FromResult(0);
      }

      public Option<IGameState> Finish()
      {
         return None;
      }

      public void Start()
      {
      }


   }
}
