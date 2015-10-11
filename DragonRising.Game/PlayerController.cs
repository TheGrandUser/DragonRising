using DraconicEngine;
using DraconicEngine.EntitySystem;
using DraconicEngine.GameViews;
using DraconicEngine.Input;
using DraconicEngine.RulesSystem;
using DraconicEngine.Terminals;
using DraconicEngine.Terminals.Input;
using DraconicEngine.Widgets;
using DragonRising.Commands;
using DragonRising.Commands.Requirements;
using DragonRising.GameWorld;
using DragonRising.GameWorld.Behaviors;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Nodes;
using DragonRising.GameWorld.Powers;
using DragonRising.Plans;
using DragonRising.Plans.Targeters;
using DragonRising.Storage;
using DragonRising.Views;
using DragonRising.Widgets;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static DraconicEngine.Input.CommandGestureFactory;
using static LanguageExt.Prelude;

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
                     .IfSome(bc => bc.RemoveBehavior(this.playerControlledBehavior));

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

      MyPlayingScreen screen;
      SceneView sceneView;
      IMessageService messageService;

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

      static readonly CommandGesture2D moveCommandGesture = CreateEightWay((l, v) => new MovementCommand(v));
      static readonly CommandGesture waitCommandGesture = Create(RogueCommands.Wait, RogueKey.NumPad5);

      static readonly CommandGesture pickUpCommandGesture = Create(new PickUpItemCommand(), RogueKey.G);
      static readonly CommandGesture dropCommandGesture = Create(new DropItemCommand(), RogueKey.D);
      static readonly CommandGesture useItemCommandGesture = Create(new UseItemCommand(), RogueKey.I);

      //static readonly CommandGesture lookCommandGesture = Create(new LookCommand(), RogueKey.L);
      static readonly CommandGesture2D mouseLookCommandGesture = CreateMousePointer((loc, delta) => new LookAtCommand(loc));

      static readonly CommandGesture goDownGesture = Create(new GoDownCommand(), RogueKey.OemComma, RogueModifierKeys.Shift);

      static readonly CommandGesture quitCommandGesture = Create(RogueCommands.Quit, RogueKey.Escape);

      public PlayerController(MyPlayingScreen screen, IMessageService messageService, SceneView sceneView)
      {
         this.screen = screen;
         this.messageService = messageService;
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

               if (action == ActionTaken.Abort)
               {
                  return PlayerTurnResult.None;
               }

               this.playerControlledBehavior.SetNextAction(action);
               return PlayerTurnResult.TurnAdvancing;
            }
            else if (inputResult.Command is GoDownCommand)
            {
               // new level
               if (World.Current.Player.GetLocation() ==
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
               this.playerControlledBehavior.SetNextAction(ActionTaken.Idle);
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
               this.playerControlledBehavior.SetNextAction(ActionTaken.Idle);
               return PlayerTurnResult.TurnAdvancing;
            }
         }
         catch (OperationCanceledException)
         {
            this.playerControlledBehavior.SetNextAction(ActionTaken.Idle);
            return PlayerTurnResult.TurnAdvancing;
         }
      }

      async Task<RequirementFulfillment> GetFulfillmentAsync(PlanRequirement requirements)
      {
         if (requirements is NoRequirement)
         {
            return NoFulfillment.None;
         }
         #region Item
         if (requirements is ItemRequirement)
         {
            var itemRequirement = (ItemRequirement)requirements;

            var item = await SelectInventoryItem(PlayerCreature, itemRequirement.Message, RogueGame.Current.RootTerminal);

            if (item == null)
            {
               return NoFulfillment.None;
            }

            var itemComponent = item.GetComponent<ItemComponent>();

            if (itemComponent.Usable != null)
            {
               if (itemRequirement.NeedsItemsFulfillment)
               {
                  var finalizedPlan = await GetPowerTargets(itemComponent.Usable.Plan);
                  
                  return finalizedPlan.Match(
                     Some: plan => ItemFulfillment.Create(item, itemComponent.Usable.GetFact(PlayerCreature, plan)),
                     None: () => NoFulfillment.None);
               }
               else
               {
                  return ItemFulfillment.Create(item, None);
               }
            }
            else
            {
               return ItemFulfillment.Create(item, None);
            }
            //Func<Usable, Task<RequirementFulfillment>> onSome = async usable =>
            //{
            //   if (itemRequirement.NeedsItemsFulfillment)
            //   {
            //      var finalizedPlan = await GetPowerTargets(usable.Power);

            //      return finalizedPlan.Match(
            //         Some: plan => ItemFulfillment.Create(item, plan),
            //         None: () => NoFulfillment.None);
            //   }
            //   else
            //   {
            //      return ItemFulfillment.Create(item, None);
            //   }
            //};
            //var onNone = fun(() => Task.FromResult(ItemFulfillment.Create(item, None)));



            //return await (itemComponent.Usable != null ?
            //   onSome(itemComponent.Usable) : onNone());
         }
         #endregion
         #region LocationRequirement
         else if (requirements is LocationRequirement)
         {
            var locationRequirement = (LocationRequirement)requirements;

            //locationRequirement.Message

            var location = await SelectTargetLocation(
               PlayerCreature.GetLocation(),
               locationRequirement.Message,
               locationRequirement.SelectionRange,
               sceneView,
               None);

            return location.HasValue ?
               (RequirementFulfillment)new LocationFulfillment(location.Value) :
               NoFulfillment.None;
         }
         #endregion
         #region Entity Requirement
         else if (requirements is EntityRequirement)
         {
            var entityRequirement = (EntityRequirement)requirements;

            var entity = await SelectTargetEntity(PlayerCreature.Location,
               entityRequirement.Range, c => (!entityRequirement.ExcludeSelf || c != PlayerCreature) &&
               entityRequirement.DoesEntityMatch(c), sceneView, None);

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
            var directionReq = (DirectionRequirement)requirements;
            var dir = await SelectDirection(PlayerCreature.Location, directionReq.Message, directionReq.Limits);

            if (dir.HasValue)
            {
               return new DirectionFulfillment(dir.Value.ToDirection());
            }
            else
            {
               return NoFulfillment.None;
            }
         }
         #endregion

         return NoFulfillment.None;
      }

      async Task<bool> Confirm(string message)
      {
         var confirmDialog = new ConfirmDialog(message, RogueGame.Current.RootTerminal);

         var result = await RogueGame.Current.RunGameState(confirmDialog);

         return result;
      }

      public void SetLookAt(Loc? lookCursor)
      {
         messageService.ClearInfoMessages();

         if (lookCursor.HasValue)
         {
            var scenePoint = sceneView.ViewOffset + lookCursor.GetValueOrDefault();

            if (World.Current.Scene.IsVisible(scenePoint))
            {
               screen.SetHighlight(lookCursor.Value);

               foreach (var entity in World.Current.Scene.EntityStore.Entities.Where(en => en.GetLocation() == scenePoint))
               {
                  messageService.AddInfoMessage(new RogueMessage(entity.Name, RogueColors.White));
               }
            }
         }
         else
         {
            screen.ClearHighlight();
         }
      }


      public static async Task<Entity> SelectInventoryItem(Entity player, string message, ITerminal terminal)
      {
         var inventory = player.GetComponent<InventoryComponent>();
         var inventoryScreen = new InventoryScreen(inventory, message, terminal);
         var index = await RogueGame.Current.RunGameState(inventoryScreen);
         
         if (index != null)
         {
            return inventory.Items[index.Value];
         }
         return null;
      }

      public static Task<Loc?> SelectTargetLocation(
         Loc startLocation,
         string message,
         SelectionRange range,
         SceneView sceneView,
         Option<Area> areaOfEffect)
      {
         var targetTool = new LocationTargetingTool(startLocation, sceneView, message, range);

         return RogueGame.Current.RunGameState(targetTool);
      }

      public async static Task<Option<Entity>> SelectTargetEntity(Loc origin, SelectionRange range,
         Predicate<Entity> filter, SceneView sceneView, Option<Area> areaOfEffect)
      {
         var rangeSquared = range.Range * range.Range;


         var entitiesInRange = World.Current.Scene.EntityStore.AllCreatures()
            .Where(c => c.HasComponent<DrawnComponent>() && filter(c) &&
               World.Current.Scene.IsVisible(c.Location) &&
               (range.Range == null || Loc.IsDistanceWithin(c.Location, origin, range.Range.Value)))
            .Select(e => new SeeableNode() { Entity = e, Drawn = e.GetComponent<DrawnComponent>() })
            .ToImmutableList();

         if (!entitiesInRange.Any())
         {
            return None;
         }

         var selectEntityTool = new SelectEntityTool(entitiesInRange, sceneView);

         var result =  await RogueGame.Current.RunGameState(selectEntityTool);

         return result;
      }

      public static async Task<Vector?> SelectDirection(Loc origin, string message, DirectionLimit limit)
      {
         while (true)
         {
            if (limit == DirectionLimit.FullVector)
            {
               var keyPress = await InputSystem.Current.GetKeyPressAsync();

               if (keyPress.Key.IsEightWayMovementKey())
               {
                  return Vector.FromDirection(keyPress.Key.ToDirection());
               };
            }
            else
            {
               var keyPress = await InputSystem.Current.GetKeyPressAsync();

               if (limit == DirectionLimit.Cardinal ?
                  keyPress.Key.IsFourWayMovementKey() :
                  keyPress.Key.IsEightWayMovementKey())
               {
                  return Vector.FromDirection(keyPress.Key.ToDirection());
               };
            }
         }
      }

      private async Task<Option<FinalizedPlan>> GetPowerTargets(EffectPlan power)
      {
         var origin = PlayerCreature.Location;

         var results = await Targeter.HandleChildTargetersAsync(
            power.Targeters,
            t => t.GetPlayerTargetingAsync(sceneView, origin, ImmutableStack<Either<Loc, Vector>>.Empty));

         return results.Match(
            Some: r => Some(new FinalizedPlan(r, power.Queries, power.Effects)),
            None: () => None);
      }
   }
}
