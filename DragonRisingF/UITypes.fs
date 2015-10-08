module DragonRisingF.UITypes
open DraconicEngineF
open DraconicEngineF.Terminal
open DraconicEngineF.Entities
open DragonRisingF.DomainTypes


type NeedsItemFulfillment = NeedsItemFulfillment of bool

type PlanRequirement =
| AllRequirements of PlanRequirement list
| AndMaybeRequirement of PlanRequirement * PlanRequirement
| ConfirmRequirement
| DirectionRequirement of DirectionLimit option
| EntityRequirement of EntityFilter
| InventoryItemRequirement of NeedsItemFulfillment
| LocationRequirement of SelectionRange
| OrRequirement of PlanRequirement * PlanRequirement

type Requirement = Requirement of PlanRequirement * string

type Fulfillment =
| AllFulfillment of Fulfillment list
| AndMaybeFulfillment of Fulfillment * Fulfillment option
| ConfirmFulfillment of bool
| DirectionFulfillment of Choice<Direction, Vector>
| EntityFulfillment of Entity
| InventoryItemFulfillment of Entity * FinalizedPlan option
| LocationRequirement of Loc
| OrRequirement of Choice<Fulfillment, Fulfillment>

type PlayerTurnResult =
| TurnAdvancing
| FinalTurn


type RogueCommand =
| WaitCommand
| QuitCommand
| Abort
| MovementCommand of Direction
| InteractWithCreatureCommand of Entity
| AttackCommand of Entity
| DropItemCommand
| GoDownCommand
| LookCommand
| ManipulateCommand of Entity
| PickUpItemCommand
| UseItemCommand of (Entity -> bool)

//type Widget = { panel: Terminal; draw: unit -> unit; tick: (unit -> Async<TickResult>) option }

type MenuItem<'T> = { name: string; canExecute: unit -> bool; value: 'T }
type MenuNavigation = | Up | Down | Select | Cancel
type MenuResult<'T> = 
   | NoResult
   | SomeResult of 'T
   | CancelledResult

let menuItem name value canExecute = { name = name; canExecute = canExecute; value = value}
let simpleMenuItem name value = { name = name; canExecute = (fun () -> true); value = value}

type MainMenuCommand = | Exit | ContinueGame | NewGame | LoadGame