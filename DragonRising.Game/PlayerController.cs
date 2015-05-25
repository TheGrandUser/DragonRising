using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DragonRising.GameWorld.Items;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.GameWorld.Actions.Requirements;
using DraconicEngine.Input;
using DraconicEngine.Widgets;
using DragonRising.GameStates;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DraconicEngine.Input.CommandGestureFactory;
using DraconicEngine.GameViews;
using DraconicEngine.Terminals.Input;
using DragonRising.Commands;
using DraconicEngine.GameWorld.Behaviors;
using System.Threading;
using System.Diagnostics;
using DragonRising.GameWorld.Nodes;
using DragonRising.Storage;
using DragonRising.GameWorld.Components;
using DragonRising.Widgets;
using DragonRising.GameWorld.Actions.Requirements;
using DragonRising.GameWorld;

namespace DragonRising
{
   public class PlayerController : IPlayerController
   {
      ExternallyControlledBehavior playerControlledBehavior;
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
                  this.playerCreature.TryGetComponent<BehaviorComponent>()
                     .ForEach(bc => bc.RemoveBehavior(this.playerControlledBehavior));

                  this.playerControlledBehavior = null;
               }
               this.playerCreature = value;

               if (this.playerCreature != null)
               {
                  var behaviorComponent = this.playerCreature.GetComponent<BehaviorComponent>();
                  var controlledBehavior = behaviorComponent.Behaviors.OfType<ExternallyControlledBehavior>().FirstOrDefault();
                  if (controlledBehavior == null)
                  {
                     behaviorComponent.PushBehavior(controlledBehavior = new ExternallyControlledBehavior());
                  }

                  this.playerControlledBehavior = controlledBehavior;
               }
            }
         }
      }

      FocusEntitySceneView sceneView;


      static IEnumerable<CommandGesture> ActionCommandGestures
      {
         get
         {
            yield return moveCommandGesture;
            yield return useItemCommandGesture;
            yield return pickUpCommandGesture;
            yield return dropCommandGesture;
            yield return waitCommandGesture;
            yield return goDownGesture;
         }
      }

      static IEnumerable<CommandGesture> NonActionCommandGestures
      {
         get
         {
            yield return mouseLookCommandGesture;
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

      static readonly CommandGesture goDownGesture = Create(new GoDownCommand(), GestureSet.Create(RogueKey.OemComma, RogueModifierKeys.Shift));

      static readonly CommandGesture quitCommandGesture = Create(RogueCommands.Quit, GestureSet.Create(RogueKey.Escape));

      public PlayerController(FocusEntitySceneView sceneView)
      {
         this.sceneView = sceneView;
      }

      public async Task<PlayerTurnResult> GetInputAsync(TimeSpan timeout)
      {
         var behaviorComponent = playerCreature.GetComponentOrDefault<BehaviorComponent>();
         var isControlled = behaviorComponent?.CurrentBehavior == this.playerControlledBehavior;

         var commands = isControlled ?
            NonActionCommandGestures.Concat(ActionCommandGestures) :
            NonActionCommandGestures;

         CancellationToken cancelToken = CancellationToken.None;

         if (!isControlled)
         {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(timeout);
            cancelToken = cts.Token;
         }

         try
         {
            var inputResult = await InputSystem.Current.GetCommandAsync(commands, cancelToken);

            if (inputResult.Command is LookAtCommand)
            {
               SetLookAt(inputResult.As2D().Point);

               return PlayerTurnResult.None;
            }
            else if (inputResult.Command is ActionCommand)
            {
               var initialCommand = (ActionCommand)inputResult.Command;
               var requirement = initialCommand.GetRequirement(playerCreature);
               RequirementFulfillment preFulfillment = null;
               if (requirement is DirectionRequirement && inputResult is InputResult2D)
               {
                  preFulfillment = new DirectionFulfillment(inputResult.As2D().Delta.ToDirection());
               }

               var action = await ActionCommand.GetFinalActionAsync(initialCommand, this.playerCreature, GetFulfillmentAsync, preFulfillment);

               if (action == RogueAction.Abort)
               {
                  return PlayerTurnResult.None;
               }

               this.playerControlledBehavior.SetNextAction(action);
               return PlayerTurnResult.TurnAdvancing;
            }
            else if (inputResult.Command is GoDownCommand)
            {
               // new level
               if(World.Current.Player.GetLocation() ==
                  World.Current.Scene.Stairs.GetLocation())
               {
                  MessageService.Current.PostMessage("You take a moment to rest, and recover your strength.", RogueColors.LightViolet);
                  World.Current.Player.As<CombatantComponent>(cc => cc.Heal(cc.MaxHP / 2));
                  MessageService.Current.PostMessage("After a rare moment of peace, you descend deeper into the heart of the dungeon...", RogueColors.Red);
                  World.Current.NextLevel();
               }

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
               this.playerControlledBehavior.SetNextAction(RogueAction.Idle);
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
               this.playerControlledBehavior.SetNextAction(RogueAction.Idle);
               return PlayerTurnResult.Idle;
            }
         }
         catch (OperationCanceledException)
         {
            this.playerControlledBehavior.SetNextAction(RogueAction.Idle);
            return PlayerTurnResult.Idle;
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

            var itemComponent = item.GetComponent<ItemComponent>();

            Func<Usable, Task<ItemFulfillment>> onSome = async usable =>
            {
               if (itemRequirement.NeedsItemsFulfillment)
               {
                  RequirementFulfillment itemFulfillment =
                     usable.Usage.Requirements is NoRequirement ? NoFulfillment.None :
                     await GetFulfillmentAsync(usable.Usage.Requirements);

                  return new ItemFulfillment(item, itemFulfillment);
               }
               else
               {
                  return new ItemFulfillment(item, NoFulfillment.None);
               }
            };
            Func<Task<ItemFulfillment>> onNone = () => Task.FromResult(new ItemFulfillment(item, NoFulfillment.None));

            

            return await (itemComponent.Usable != null ?
               onSome(itemComponent.Usable) : onNone());
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
         else if (requirements is AllRequirement)
         {
            var allRequirement = (AllRequirement)requirements;
            List<RequirementFulfillment> fulfillments = new List<RequirementFulfillment>();
            foreach (var requirement in allRequirement.Requirements)
            {
               var fulfillment = await GetFulfillmentAsync(requirement);
               if (fulfillment is NoFulfillment)
               {
                  return fulfillment;
               }
               fulfillments.Add(fulfillment);
            }

            return new AllFulfillment(fulfillments);
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
         MyPlayingScreen.Current.ClearInfoMessages();

         if (lookCursor.HasValue)
         {
            var scenePoint = this.sceneView.ViewOffset + lookCursor.GetValueOrDefault();
            
            if (World.Current.Scene.IsVisible(scenePoint))
            {
               MyPlayingScreen.Current.SetHighlight(lookCursor.Value);

               foreach (var entity in World.Current.Scene.EntityStore.Entities.Where(en => en.GetLocation() == scenePoint))
               {
                  MyPlayingScreen.Current.AddInfoMessage(new RogueMessage(entity.Name, RogueColors.White));
               }
            }
         }
         else
         {
            MyPlayingScreen.Current.ClearHighlight();
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

         MyPlayingScreen playingState = MyPlayingScreen.Current;
         var targetTool = new LocationTargetingTool(this.PlayerCreature.GetLocation(), this.sceneView, playingState.ScenePanel, requirement.Message, requirement.IsLimitedToFoV, requirement.MaxRange);

         await RogueGame.Current.RunGameState(targetTool);

         return targetTool.Result;
      }

      public async Task<Option<Entity>> SelectTargetEntity(EntityRequirement requirement)
      {
         MyPlayingScreen playingState = MyPlayingScreen.Current;
         var range = requirement.MaxRange;
         var excludeSelf = requirement.ExcludeSelf;
         var rangeSquared = range * range;


         var entitiesInRange = World.Current.Scene.EntityStore.AllCreatures()
            .Where(c => (!excludeSelf || c != this.playerCreature) &&
               c.HasComponent<DrawnComponent>() && c.HasComponent<LocationComponent>() &&
               requirement.DoesEntityMatch(c) &&
               World.Current.Scene.IsVisible(c.GetComponent<LocationComponent>().Location) &&
               (range == null || (c.GetComponent<LocationComponent>().Location - this.PlayerCreature.GetComponent<LocationComponent>().Location).LengthSquared <= rangeSquared))
            .Select(e => new SeeableNode() { Entity = e, Drawn = e.GetComponent<DrawnComponent>(), Loc = e.GetComponent<LocationComponent>() })
            .ToArray();

         if (!entitiesInRange.Any())
         {
            return None;
         }

         var selectEntityTool = new SelectEntityTool(ImmutableList.CreateRange(entitiesInRange), this.sceneView, playingState.ScenePanel);

         await RogueGame.Current.RunGameState(selectEntityTool);

         return selectEntityTool.Result;
      }

      public Task<Direction> SelectDirection(DirectionRequirement requirement)
      {
         throw new NotImplementedException();
      }

   }
}
