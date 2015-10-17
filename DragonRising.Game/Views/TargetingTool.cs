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
   class LocationTargetingTool : IGameView<Loc?>
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

      Loc startLocation;
      Loc location;

      public GameViewType Type { get { return GameViewType.PartialScreen; } }

      ITerminal sceneTerminal;
      SelectionRange selectionRange;
      private Area area;

      public LocationTargetingTool(Loc startLocation, SceneView sceneView, string message, SelectionRange selectionRange, Option<Area> area)
      {
         this.startLocation = this.location = startLocation;
         this.sceneView = sceneView;
         this.selectionRange = selectionRange;

         this.sceneTerminal = sceneView.Panel;
         this.area = area.Match(s => s, () => null);
      }

      public async Task<Loc?> DoLogic()
      {
         while (true)
         {
            var keyEvent = await InputSystem.Current.GetKeyPressAsync();
            if (keyEvent.Key == RogueKey.Enter || keyEvent.Key == RogueKey.Space)
            {
               if (!selectionRange.Range.HasValue || Loc.IsDistanceWithin(startLocation, location, selectionRange.Range.Value))
               {
                  if (IsWithinRange(location))
                  {
                     return location;
                  }
               }
            }
            else if (keyEvent.Key == RogueKey.Escape)
            {
               return null;
            }
            else if (keyEvent.Key.IsEightWayMovementKey())
            {
               Vector offset = keyEvent.Key.ToMovementVec();

               var newLoc = location + offset;
               if (IsWithinRange(newLoc))
               {
                  location = newLoc;
               }
            }
         }
      }

      bool IsWithinRange(Loc loc)
      {
         return (!this.selectionRange.Range.HasValue || (loc - startLocation).LengthSquared <= (this.selectionRange.Range.Value * this.selectionRange.Range.Value)) &&
            (!selectionRange.Limits.HasFlag(RangeLimits.LineOfEffect) || World.Current.Scene.IsUnblockedBetween(startLocation, location)) &&
            (!selectionRange.Limits.HasFlag(RangeLimits.LineOfSight) || World.Current.Scene.IsVisible(location));
      }

      public void Draw()
      {
         if (area != null)
         {
            foreach (var scenePoint in area.GetPointsInArea())
            {
               Loc displayPoint = scenePoint - this.sceneView.ViewOffset + this.location;
               if (sceneTerminal.Size.Contains(displayPoint))
               {
                  this.sceneTerminal.SetHighlight(displayPoint, RogueColors.DarkGray, RogueColors.LightCyan);
               }
            }
         }

         var path = this.startLocation.GetLineTo(this.location);
         foreach (var scenePoint in path.TakeWhile(p => p != this.location))
         {
            Loc displayPoint = scenePoint - this.sceneView.ViewOffset;
            if (sceneTerminal.Size.Contains(displayPoint))
            {
               this.sceneTerminal.SetHighlight(displayPoint, RogueColors.LightGray, RogueColors.DarkCyan);
            }
         }



         Loc finalPoint = this.location - this.sceneView.ViewOffset;
         if (sceneTerminal.Size.Contains(finalPoint))
         {
            this.sceneTerminal[finalPoint][RogueColors.White, RogueColors.DarkCyan].Write(Glyph.Plus);
         }
      }
   }
}
