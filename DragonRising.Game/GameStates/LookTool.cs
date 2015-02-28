using DraconicEngine;
using DraconicEngine.GameStates;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Input;
using static DraconicEngine.Input.CommandGestureFactory;
using DraconicEngine.Terminals;
using DraconicEngine.Terminals.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading;

namespace DragonRising.GameStates
{
   class LookTool : IGameState
   {
      public GameStateType Type { get { return GameStateType.Tool; } }

      PlayerController playerController;

      public LookTool(Loc startPoint, PlayerController playerController)
      {
         this.lastPoint = startPoint;
         this.playerController = playerController;
      }

      enum LookActions
      {
         MoveCursor,
         End
      }

      static IEnumerable<CommandGesture> Gestures => EnumerableEx.Return(moveCursorGesture);

      static readonly CommandGesture2D moveCursorGesture = CreateMouseKey2D(LookActions.MoveCursor);

      static readonly CommandGesture endGesture = CreateGesture(LookActions.End, GestureSet.Create(RogueKey.Escape, RogueKey.Enter, RogueKey.Space));

      Loc lastPoint;
      public async Task<TickResult> Tick()
      {
         var input = await InputSystem.Current.GetCommandAsync(Gestures, CancellationToken.None);

         var command = (ValueCommand<LookActions>)input.Command;

         if (command.Value == LookActions.MoveCursor)
         {
            var point =
               lastPoint + input.As2D().Delta;
            lastPoint = point;

            Loc? localPoint = MyPlayingState.Current.ScenePanel.RootVecToLocalVec(point);

            this.playerController.SetLookAt(localPoint);

            return TickResult.Continue;
         }
         else // End
         {
            return TickResult.Finished;
         }
      }

      private Task<RequirementFulfillment> GetFulfillment(Action arg)
      {
         throw new NotImplementedException();
      }

      public Task Draw()
      {
         return Task.FromResult(0);
      }

      public Option<IGameState> Finish()
      {
         this.playerController.SetLookAt(null);
         return None;
      }

      public void Start()
      {
      }
   }
}