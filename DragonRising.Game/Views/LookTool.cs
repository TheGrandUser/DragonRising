using DraconicEngine;
using DraconicEngine.GameViews;
using DragonRising.Commands.Requirements;
using DraconicEngine.EntitySystem;
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

namespace DragonRising.Views
{
   class LookTool : IGameView
   {
      public GameViewType Type { get { return GameViewType.Tool; } }

      ITerminal scenePanel;
      PlayerController playerController;

      public LookTool(Loc startPoint, ITerminal scenePanel, PlayerController playerController)
      {
         this.lastPoint = startPoint;
         this.scenePanel = scenePanel;
         this.playerController = playerController;
      }

      enum LookActions
      {
         MoveCursor,
         End
      }

      static IEnumerable<CommandGesture> Gestures => EnumerableEx.Return(moveCursorGesture);

      static readonly CommandGesture2D moveCursorGesture = CreateEightWay(LookActions.MoveCursor);

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

            Loc? localPoint = scenePanel.RootVecToLocalVec(point);

            this.playerController.SetLookAt(localPoint);

            return TickResult.Continue;
         }
         else // End
         {
            this.playerController.SetLookAt(null);
            return TickResult.Finished;
         }
      }
      
      public Task Draw()
      {
         return Task.FromResult(0);
      }
      
      public void OnStart()
      {
      }
   }
}