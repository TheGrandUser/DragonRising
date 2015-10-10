using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameViews;
using DraconicEngine.Input;
using DraconicEngine.Terminals;
using DraconicEngine.Widgets;
using LanguageExt;
using static LanguageExt.Prelude;
using DragonRising.Widgets;
using DragonRising.GameWorld;
using DraconicEngine.RulesSystem;

namespace DragonRising.Views
{
   class LocationTargetingTool : IGameView
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

      public GameViewType Type { get { return GameViewType.PartialScreen; } }

      ITerminal sceneTerminal;
      SelectionRange selectionRange;

      public LocationTargetingTool(Loc startLocation, SceneView sceneView, string message, SelectionRange selectionRange)
      {
         this.startLocation = this.location = startLocation;
         this.sceneView = sceneView;
         this.selectionRange = selectionRange;

         this.sceneTerminal = sceneView.Panel;
      }

      public async Task<TickResult> DoLogic()
      {
         while (true)
         {
            var keyEvent = await InputSystem.Current.GetKeyPressAsync();
            if (keyEvent.Key == RogueKey.Enter || keyEvent.Key == RogueKey.Space)
            {
               if (!selectionRange.Range.HasValue || Loc.IsDistanceWithin(startLocation, location, selectionRange.Range.Value))
               {
                  if (
                     (!selectionRange.Limits.HasFlag(RangeLimits.LineOfEffect) || World.Current.Scene.IsUnblockedBetween(startLocation, location)) &&
                     (!selectionRange.Limits.HasFlag(RangeLimits.LineOfSight) || World.Current.Scene.IsVisible(location)))
                  {
                     result = location;
                     return TickResult.Finished;
                  }
               }
            }
            else if (keyEvent.Key == RogueKey.Escape)
            {
               result = null;
               return TickResult.Finished;
            }
            else if (keyEvent.Key.IsEightWayMovementKey())
            {
               Vector offset = keyEvent.Key.ToMovementVec();

               location += offset;
            }
         }
      }

      public Task Draw()
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

         return Task.FromResult(0);
      }
   }
}
