using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameStates;
using DraconicEngine.Input;
using DraconicEngine.Widgets;
using DraconicEngine.Terminals;
using DraconicEngine.Input.CommandGestureFactory;
using DraconicEngine.Terminals.Input;
using LanguageExt;
using LanguageExt.Prelude;
using System.Threading;
using DragonRising.GameWorld.Nodes;
using DragonRising.Widgets;

namespace DragonRising.GameStates
{
   class SelectEntityTool : IGameState<Entity>
   {
      IImmutableList<SeeableNode> availableEntities;
      SceneView sceneView;
      ITerminal sceneTerminal;

      Entity result;
      public Entity Result { get { return result; } }

      public GameStateType Type { get { return GameStateType.Tool; } }

      public SelectEntityTool(IImmutableList<SeeableNode> availableEntities, SceneView sceneView, ITerminal scenePanel)
      {
         

         this.availableEntities = availableEntities;
         this.sceneView = sceneView;
         this.sceneTerminal = scenePanel;
      }

      enum TargetAction
      {
         Cylce,
         Accept,
         Cancel,
         Point,
      }

      // key scroll through list
      // key cancel
      // mouse point
      // accept

      CommandGesture1D cylce = Create1D(
         TargetAction.Cylce,
         g => g.Modifiers == RogueModifierKeys.None ? g.Key.ToCycle() : 0,
         GestureSet.Create4WayMove());

      CommandGesture cancel = CreateGesture(TargetAction.Cancel, GestureSet.Create(RogueKey.Escape));

      CommandGesture2D point = CreateMousePointer(TargetAction.Point);

      CommandGesture accept = CreateGesture(TargetAction.Accept,
         GestureSet.Create(RogueMouseAction.LeftClick, RogueKey.Enter, RogueKey.Space));


      IEnumerable<CommandGesture> Gestures
      {
         get
         {
            yield return cylce;
            yield return cancel;
            yield return point;
            yield return accept;
         }
      }

      int lastSelectedIndex = 0;
      int? currentSelectedIndex = 0;

      public async Task<TickResult> Tick()
      {
         var inputResult = await InputSystem.Current.GetCommandAsync(this.Gestures, CancellationToken.None);
         var command = inputResult.Command as ValueCommand<TargetAction>;

         switch (command.Value)
         {
            case TargetAction.Cylce:
               var inputResult1D = inputResult.As1D();
               if (inputResult1D.Delta > 0)
               {
                  if (currentSelectedIndex != null)
                  {
                     lastSelectedIndex++;
                  }
               }
               else if (inputResult1D.Delta < 0)
               {
                  if (currentSelectedIndex != null)
                  {
                     lastSelectedIndex--;
                  }
               }

               lastSelectedIndex = (lastSelectedIndex + this.availableEntities.Count) % this.availableEntities.Count;

               currentSelectedIndex = lastSelectedIndex;
               break;
            case TargetAction.Accept:
               if (this.currentSelectedIndex.HasValue && this.currentSelectedIndex.Value < this.availableEntities.Count)
               {
                  this.result = this.availableEntities[this.currentSelectedIndex.Value].Entity;
                  return TickResult.Finished;
               }
               break;
            case TargetAction.Cancel:
               this.result = null;
               return TickResult.Finished;
            case TargetAction.Point:
               var inputResult2D = inputResult.As2D();
               var rootPoint = inputResult2D.Point.Value;
               var localPoint = MyPlayingState.Current.ScenePanel.RootVecToLocalVec(rootPoint);
               if (localPoint.HasValue)
               {
                  var scenePoint = this.sceneView.ViewOffset + localPoint;

                  var entity = this.availableEntities.FirstOrDefault(e => e.Loc.Location == scenePoint);

                  if (entity != null)
                  {
                     this.currentSelectedIndex = this.lastSelectedIndex = this.availableEntities.IndexOf(entity);
                  }
                  else
                  {
                     this.currentSelectedIndex = null;
                  }
               }
               break;
            default:
               break;
         }

         return TickResult.Continue;
      }

      public Task Draw()
      {
         int index = 0;
         foreach(var entity in this.availableEntities)
         {
            Loc displayLocation = entity.Loc.Location - this.sceneView.ViewOffset;

            var foreColor = index == this.currentSelectedIndex ? entity.Drawn.SeenCharacter.ForeColor : RogueColors.Gray;

            var character = new Character(entity.Drawn.SeenCharacter.Glyph, foreColor, RogueColors.White);

            if (this.sceneTerminal.Size.Contains(displayLocation))
            {
               this.sceneTerminal.SetHighlight(displayLocation, foreColor, RogueColors.White);
            }
            index++;
         }
         
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
