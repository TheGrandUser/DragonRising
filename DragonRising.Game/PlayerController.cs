using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Items;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.Input;
using DraconicEngine.Widgets;
using DragonRising.GameStates;
using LanguageExt;
using LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Input.CommandGestureFactory;
using DraconicEngine.GameStates;
using DraconicEngine.Terminals.Input.Commands;
using DraconicEngine.Terminals.Input;
using DragonRising.Commands;

namespace DragonRising
{
   public class PlayerController : IPlayerController
   {
      DecisionComponent playerDecisionComponent;

      Entity playerCreature;
      public Entity PlayerCreature
      {
         get { return this.playerCreature; }
         set
         {
            if (this.playerCreature != value)
            {
               if (this.playerCreature != null)
               {
                  this.playerCreature.As<BehaviorComponent>(b => b.IsDirectlyControlled = false);
                  this.playerDecisionComponent = null;
               }
               this.playerCreature = value;

               if (this.playerCreature != null)
               {
                  this.playerCreature.As<BehaviorComponent>(b => b.IsDirectlyControlled = true);
                  this.playerDecisionComponent = this.playerCreature.GetComponent<DecisionComponent>();
               }
            }
         }
      }

      FocusEntitySceneView sceneView;


      static IEnumerable<CommandGesture> CommandGestures
      {
         get
         {
            yield return moveCommandGesture;
            yield return mouseLookCommandGesture;
            yield return useItemCommandGesture;
            yield return pickUpCommandGesture;
            yield return dropCommandGesture;
            yield return waitCommandGesture;
            yield return quitCommandGesture;
         }
      }

      static readonly CommandGesture moveCommandGesture = CreateAction(kg => new MovementCommand(kg.Key.ToDirection()), GestureSet.Create8WayMove());
      static readonly CommandGesture waitCommandGesture = Create(RogueCommands.Wait, GestureSet.Create(RogueKey.NumPad5));

      static readonly CommandGesture pickUpCommandGesture = Create(new PickUpItemCommand(), RogueKey.G);
      static readonly CommandGesture dropCommandGesture = Create(new DropItemCommand(), RogueKey.D);
      static readonly CommandGesture useItemCommandGesture = Create(new UseItemCommand(), RogueKey.I);

      static readonly CommandGesture lookCommandGesture = Create(new LookCommand(), RogueKey.L);
      static readonly CommandGesture2D mouseLookCommandGesture = CreateMousePointer((loc, delta) => new LookAtCommand(loc));

      static readonly CommandGesture quitCommandGesture = Create(RogueCommands.Quit, GestureSet.Create(RogueKey.Escape));

      public PlayerController(FocusEntitySceneView sceneView)
      {
         this.sceneView = sceneView;
      }

      public async Task<PlayerTurnResult> CheckInputAsync()
      {

         if (playerCreature.GetComponentOrDefault<BehaviorComponent>()?.IsDirectlyControlled ?? true)
         {
            var inputResult = await InputSystem.Current.GetCommandAsync(CommandGestures);

            if (inputResult.Command is LookAtCommand)
            {
               SetLookAt(inputResult.As2D().Point);

               return PlayerTurnResult.None;
            }
            else if (inputResult.Command is ActionCommand)
            {
               var initialCommand = (ActionCommand)inputResult.Command;

               RequirementFulfillment preFulfillment = null;
               if (initialCommand.Requirement is DirectionRequirement && inputResult is InputResult2D)
               {
                  preFulfillment = new DirectionFulfillment(inputResult.As2D().Delta.ToDirection());
               }

               var action = await ActionCommand.GetFinalActionAsync(initialCommand, this.playerCreature, GetFulfillmentAsync, preFulfillment);

               if (action == RogueAction.Abort)
               {
                  return PlayerTurnResult.None;
               }

               this.playerDecisionComponent.ActionToDo = action;
               return PlayerTurnResult.TurnAdvancing;
            }
            else if (inputResult.Command is LookCommand)
            {
               var look = (LookCommand)inputResult.Command;
               await look.Do();
               return PlayerTurnResult.None;
            }
            else if (inputResult.Command == RogueCommands.Wait)
            {
               this.playerDecisionComponent.ActionToDo = RogueAction.Idle;
               return PlayerTurnResult.TurnAdvancing;
            }
            else if (inputResult.Command == RogueCommands.NoOp)
            {
               return PlayerTurnResult.None;
            }
            else if (inputResult.Command == RogueCommands.Quit)
            {
               var confirmed = await Confirm("Do you really want to quit?");

               return confirmed ? PlayerTurnResult.Quit : PlayerTurnResult.None;
            }
            else
            {
               throw new NotImplementedException();
            }
         }
         else
         {
            await Task.Delay(TimeSpan.FromSeconds(0.3));
            return PlayerTurnResult.TurnAdvancing;
         }
      }

      async Task<RequirementFulfillment> GetFulfillmentAsync(ActionRequirement requirements)
      {
         if (requirements is NoRequirement)
         {
            return NoFulfillment.None;
         }
         #region Item
         if (requirements is ItemRequirement)
         {
            var itemRequirement = (ItemRequirement)requirements;

            var item = await SelectInventoryItem(itemRequirement);

            if (item == null)
            {
               return NoFulfillment.None;
            }

            if (itemRequirement.NeedsItemsFulfillment)
            {
               RequirementFulfillment itemFulfillment =
                  item.GetComponent<ItemComponent>().Template.Usage.Requirements is NoRequirement ? NoFulfillment.None :
                  await GetFulfillmentAsync(item.GetComponent<ItemComponent>().Template.Usage.Requirements);

               return new ItemFulfillment(item, itemFulfillment);
            }
            return new ItemFulfillment(item, NoFulfillment.None);
         }
         #endregion
         #region LocationRequirement
         else if (requirements is LocationRequirement)
         {
            var locationRequirement = (LocationRequirement)requirements;

            //locationRequirement.Message

            var location = await SelectTargetLocation(locationRequirement);

            if (location.HasValue)
            {
               return new LocationFulfillment(location.Value);
            }
            else
            {
               return NoFulfillment.None;
            }
         }
         #endregion
         #region Entity Requirement
         else if (requirements is EntityRequirement)
         {
            var entity = await SelectTargetEntity((EntityRequirement)requirements);

            return entity.Match<RequirementFulfillment>(
               Some: e => new EntityFulfillment(e),
               None: () => NoFulfillment.None);
         }
         #endregion
         #region And
         else if (requirements is AndRequirement)
         {
            var andRequirement = (AndRequirement)requirements;

            var fulfillment1 = await GetFulfillmentAsync(andRequirement.First);
            if (fulfillment1 is NoFulfillment)
            {
               return fulfillment1;
            }
            var fulfillment2 = await GetFulfillmentAsync(andRequirement.First);
            if (fulfillment2 is NoFulfillment)
            {
               return fulfillment2;
            }
            return new AndFulfillment(fulfillment1, fulfillment2);
         }
         #endregion
         #region And Maybe
         else if (requirements is AndMaybeRequirement)
         {
            var andRequirement = (AndMaybeRequirement)requirements;

            var fulfillment1 = await GetFulfillmentAsync(andRequirement.First);
            if (fulfillment1 is NoFulfillment)
            {
               return fulfillment1;
            }

            if (!andRequirement.IsSecondRequired(fulfillment1))
            {
               return new AndMaybeFulfillment(fulfillment1, None);
            }

            var fulfillment2 = await GetFulfillmentAsync(andRequirement.First);
            if (fulfillment2 is NoFulfillment)
            {
               return fulfillment2;
            }
            return new AndMaybeFulfillment(fulfillment1, Some(fulfillment2));
         }
         #endregion
         #region Or
         else if (requirements is OrRequirement)
         {
            var orRequirement = (OrRequirement)requirements;

            var fulfillment1 = await GetFulfillmentAsync(orRequirement.Prefered);
            if (fulfillment1 is NoFulfillment)
            {
               return await GetFulfillmentAsync(orRequirement.Alternate);
            }
            return fulfillment1;
         }
         #endregion
         #region Confrim
         else if (requirements is ConfirmRequirement)
         {
            var confirm = (ConfirmRequirement)requirements;

            return new ConfirmFulfillment(await Confirm(confirm.Message));
         }
         #endregion
         #region Direction
         else if (requirements is DirectionRequirement)
         {
            // Generally shoulod be prefulfilled, but...
            var dir = await SelectDirection((DirectionRequirement)requirements);

            return new DirectionFulfillment(dir);
         }
         #endregion

         return NoFulfillment.None;
      }

      async Task<bool> Confirm(string message)
      {
         var confirmDialog = new ConfirmDialog(message, RogueGame.Current.RootTerminal);

         await RogueGame.Current.RunGameState(confirmDialog);

         return confirmDialog.Result;
      }

      public void SetLookAt(Loc? lookCursor)
      {
         MyPlayingState.Current.ClearInfoMessages();

         if (lookCursor.HasValue)
         {
            var scenePoint = this.sceneView.ViewOffset + lookCursor.GetValueOrDefault();

            if (MyPlayingState.Current.Scene.IsVisible(scenePoint))
            {
               MyPlayingState.Current.SetHighlight(lookCursor.Value);

               foreach (var entity in MyPlayingState.Current.Scene.EntityStore.AllEntities.Where(en => en.Location == scenePoint))
               {
                  MyPlayingState.Current.AddInfoMessage(new RogueMessage(entity.Name, RogueColors.White));
               }
            }
         }
         else
         {
            MyPlayingState.Current.ClearHighlight();
         }
      }

      public async Task<Entity> SelectInventoryItem(ItemRequirement requirement)
      {
         var inventory = this.PlayerCreature.GetComponent<InventoryComponent>();
         var inventoryScreen = new InventoryScreen(inventory, requirement.Message);
         await RogueGame.Current.RunGameState(inventoryScreen);

         var index = inventoryScreen.Result;
         if (index != null)
         {
            return inventory.Items[index.Value];
         }
         return null;
      }

      public async Task<Loc?> SelectTargetLocation(LocationRequirement requirement)
      {
         //string message, bool isLimitedToFoV = true, int? maxRange = null

         MyPlayingState playingState = MyPlayingState.Current;
         var targetTool = new LocationTargetingTool(this.PlayerCreature.Location, this.sceneView, playingState.ScenePanel, requirement.Message, requirement.IsLimitedToFoV, requirement.MaxRange);

         await RogueGame.Current.RunGameState(targetTool);

         return targetTool.Result;
      }

      public async Task<Option<Entity>> SelectTargetEntity(EntityRequirement requirement)
      {
         MyPlayingState playingState = MyPlayingState.Current;
         var range = requirement.MaxRange;
         var rangeSquared = range * range;

         var entitiesInRange = Scene.CurrentScene.EntityStore.AllCreaturesExceptSpecial.Where(c => 
            requirement.DoesEntityMatch(c) &&
            Scene.CurrentScene.IsVisible(c.Location) &&
            (range == 0 || (c.Location - this.PlayerCreature.Location).LengthSquared <= rangeSquared)).ToArray();

         if (!entitiesInRange.Any())
         {
            return None;
         }

         var selectEntityTool = new SelectEntityTool(ImmutableList.CreateRange<Entity>(entitiesInRange), this.sceneView, playingState.ScenePanel);

         await RogueGame.Current.RunGameState(selectEntityTool);

         return selectEntityTool.Result;
      }

      public Task<Direction> SelectDirection(DirectionRequirement requirement)
      {
         throw new NotImplementedException();
      }
   }
}
