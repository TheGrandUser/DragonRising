using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameStates;
using DraconicEngine.Input;
using DraconicEngine.Terminals;
using DraconicEngine.Widgets;
using LanguageExt;
using LanguageExt.Prelude;

namespace DragonRising.GameStates
{
   class LocationTargetingTool : IGameState<Loc?>
   {
      enum TargetAction
      {
         Move,
         Select,
         MouseMove,
         MouseSelect,
         Cancel,
      }

      SceneView sceneView;

      Loc? result;
      public Loc? Result { get { return result; } }

      Loc startLocation;
      Loc location;

      public GameStateType Type { get { return GameStateType.Tool; } }

      ITerminal sceneTerminal;
      private bool isLimitedToFoV;

      public LocationTargetingTool(Loc startLocation, SceneView sceneView, ITerminal sceneTerminal, string message, bool isLimitedToFoV, int? maxRange)
      {
         this.startLocation = this.location = startLocation;
         this.sceneView = sceneView;
         this.isLimitedToFoV = isLimitedToFoV;

         this.sceneTerminal = sceneTerminal;
      }

      public async Task<TickResult> Tick()
      {
         var keyEvent = await InputSystem.Current.GetKeyPressAsync();
         if (keyEvent.Key == RogueKey.Enter || keyEvent.Key == RogueKey.Space)
         {
            if (!isLimitedToFoV || Scene.CurrentScene.IsVisible(location))
            {
               result = location;
               return TickResult.Finished;
            }
         }
         else if (keyEvent.Key == RogueKey.Escape)
         {
            result = null;
            return TickResult.Finished;
         }
         else if (keyEvent.Key.IsStandardMovementKey())
         {
            Vector offset = keyEvent.Key.ToMovementVec();

            location += offset;
         }
         return TickResult.Continue;
      }

      public void Draw()
      {
         var path = this.startLocation.GetLineTo(this.location);
         foreach (var scenePoint in path.TakeWhile(p => p != this.location))
         {
            Loc displayPoint = scenePoint - this.sceneView.ViewOffset;
            if (sceneTerminal.Size.Contains(displayPoint))
            {
               this.sceneTerminal.SetHighlight(displayPoint, RogueColors.Gray, RogueColors.DarkCyan);
            }
         }

         Loc finalPoint = this.location - this.sceneView.ViewOffset;
         if (sceneTerminal.Size.Contains(finalPoint))
         {
            this.sceneTerminal[finalPoint][RogueColors.White, RogueColors.DarkCyan].Write(Glyph.Plus);
         }
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
