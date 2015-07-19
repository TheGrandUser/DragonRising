module DragonRisingF.DomainTypes
open DraconicEngineF.CoreObjects
open DraconicEngineF.Entities
open FSharpx.Stm

type Alligence = Alligence of string
let neutral = Alligence "Neutral"

type IFF =
| Neutral
| Enemy
| Ally

type PlanChoice =
| LocationChoice of Loc
| EntityChoice of Entity
| DirectionChoice of Direction

type FromLocation = Loc * PlanChoice list
type FromDirection = Direction * Loc * PlanChoice list

type EntityOrLocationEffect = | EntityEffect of string | LocationEffect of string
type AreaEffect = AreaEffect of string

type FromLocationQuery =
| LocToLoc of (FromLocation -> Loc) * FromLocationQuery list * EntityOrLocationEffect list
| LocToEntity of (FromLocation -> EntityId) * FromLocationQuery list * EntityOrLocationEffect list
| LocToDirection of (FromLocation -> Direction) * FromDirecitonQuery
| LocToArea of (FromLocation -> Area) * AreaEffect list
and FromDirecitonQuery = 
| DirToLoc of (FromDirection -> Loc) * FromLocationQuery list * EntityOrLocationEffect list
| DirToEntity of (FromDirection -> Entity) * FromLocationQuery list * EntityOrLocationEffect list
| DirToArea of (FromDirection -> Area) * AreaEffect list

type FromLocationTargeter = 
| TargetLocation of (FromLocation -> Async<Loc>) * FromLocationTargeter list * FromLocationQuery list * EntityOrLocationEffect list
| TargetDirection of (FromLocation -> Async<Direction>) * FromDirecitonQuery
| TargetEntity of (FromLocation -> Async<Entity>) * FromLocationTargeter list * FromLocationQuery list * EntityOrLocationEffect list
and FromDirectionTargeter =
| TargetLocation of (FromDirection -> Async<Loc>) * FromLocationTargeter list * FromLocationQuery list * EntityOrLocationEffect list
| TargetEntity of (FromDirection -> Async<Entity>) * FromLocationTargeter list * FromLocationQuery list * EntityOrLocationEffect list

type Plan = { targeters: FromLocationTargeter list; queries: FromLocationQuery list; effects: EntityOrLocationEffect list }

type FinalizedPlan = FinalizedPlan of Plan * Stuff



type Behavior = PlayerControled | JustPassBehavior | BasicMonsterBehavior | ConfusedBehavior
type Condition = | Confused

type BehaviorComponent = { behaviors: Behavior list }
type CreatureComponent = { visionRadius: int }
type ConditionComponent = { conditions: Condition list }
type DrawnComponent = { seen: Character; explored: Character option }

type EquipmentSlot = EquipmentSlot of string
type EquipmentComponent = { weapon1: EntityId; weapon2: EntityId; equipped: Map<EquipmentSlot, EntityId> }

type Spell = { name: string; plan: Plan }
type UseInfo =
   | NormalUse of Plan
   | SpellUse of Spell
type Charge = { maxCharges: int; charges: int }
type Usable = { info: UseInfo; charge: Charge option }

type EquipableInfo = EquipableUse of EquipmentSlot
type Damage = { amount: int; kind: string }
type WeaponInfo = { isTwoHanded: bool; damage: Damage; range: int option; }
type ItemUseResult = | Used | Destroyed | NotUsed

type ItemComponent = 
| Flavor
| Usable of UseInfo * Charge option
| Equipable of EquipableInfo
| Weapon of EquipableInfo * WeaponInfo

type Manipulation = string
type ManipulatableComponent = 
| SelfOnly of Manipulation
| RequiresItem of Manipulation

type InventoryComponent = 
| EmptyInventory of int
| Inventory of int * Entity list
| FullInventory of Entity list

type MoveDetails = { direction: Direction }
type DropItemDetails = { itemToDrop: Entity }
type AttackEntityDetails = { target: Entity; weapon: Entity option }
type CastSpellDetails = { spell: Spell; finalizedPlan: FinalizedPlan }
type ManipulateEntityDetails = { target: Entity; item: Entity option }
type PickUpItemDetails = { itemToPick: Entity }
type UseItemDetails = { item: Entity; finalizedPlan: FinalizedPlan }

type ActionTaken =
| IdleAction
| MoveAction of MoveDetails
| DropItemAction of DropItemDetails
| AttackEntityAction of AttackEntityDetails
| CastSpellAction of CastSpellDetails
| ManipulateEntityAction of ManipulateEntityDetails
| PickUpItemAction of PickUpItemDetails
| UseItemAction of UseItemDetails

type Sense = | Sight | Sound | Touch | Smell | Taste | Feeling
type Sensed = { sense: Sense; kind: string; quality: string; strength: int}
type SensoryEvent = { location: Loc; effects: Sensed list }

type AttackEvent =
| AttackHit of AttackEntityDetails
| AttackMissed of AttackEntityDetails

type InflictDamageEvent = { target: Entity; damage: Damage }

type AddConditionEvent = { target: Entity; newCondition: Condition; duration: int }
type ConditionExpiredEvent = { entity: Entity; expireingCondition: Condition }
type ConditionRemovedEvent = { entity: Entity; removedCondition: Condition }

type CreatureKilledEvent = { creature: Entity; cause: string option; killingEntity: Entity option }
type CreatureHealedEvent = { creature: Entity; healer: Entity; amount: int}

type TileId = TileId of int
type TileType = { id: TileId; name: string; description: string option; inView: Character; explored: Character; blocksMovement: bool; blocksSight: bool }
type TileVisibility = | NotSeen | Explored | Seen
type Tile = { tileId: TileId; visibility: TileVisibility }

let makeTile id = { tileId = id; visibility = NotSeen }
let voidId = TileId 0
let beyondTheEdge = makeTile voidId

type Blockage = | NoBlock | BlockingTile | BlockingEntity | OffMap

type GameMap = { width: int; height: int; tiles: Tile array }
type EntityStore = EntityStore of TVar<Entity list>
type Scene = { focusEntity: Entity; stairs: Entity; map: GameMap; level: int; entityStore: EntityStore }