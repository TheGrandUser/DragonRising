#if INTERACTIVE
module WorldState
#else
module DragonRisingF.WorldState
#endif

open DraconicEngineF
open DisplayCore
open Entities
open FSharpx.Stm

type PlanId = | PlanId of int
type Alligence = | Alligence of string
type IFF = 
   | Neutral
   | Enemy
   | Ally

type Condition = 
   | Confused
   | Slowed

type EquipableSlot = 
   | EquipmentSlot of string
   | OneHanded
   | TwoHanded
  
type Spell = { name : string; plan : PlanId }

type UseInfo = 
   | NormalUse of PlanId
   | SpellUse of Spell

type Charge = { maxCharges : int; charges : int }

type Usable = { info : UseInfo; charge : Charge option }

type Damage = { amount : int; kind : string }

type WeaponInfo = { isTwoHanded : bool; damage : Damage; range : int option }

type ItemUseResult = 
   | Used
   | Destroyed
   | NotUsed

type Manipulation = string

type TileId = | TileId of int

type TileType = 
   { id : TileId
     name : string
     description : string option
     inView : Character
     explored : Character
     blocksMovement : bool
     blocksSight : bool }

type TileVisibility = 
   | NotSeen
   | Explored
   | Seen

type Tile = 
   { tileId : TileId
     visibility : TileVisibility }

type Blockage = 
   | NoBlock
   | BlockingTile of TileType
   | BlockingEntity of Entity
   | OffMap

type GameMap = { width : int; height : int; tiles : Tile array; tileTypes : Map<TileId, TileType> }
 
type CombatantComponent = 
   { power : int
     defense : int }

type ConditionComponent = 
   { conditions : Condition list }
   
type CreatureComponent = 
   { hp : int
     maxHP : int
     isAlive : bool
     visionRadius : int
     alligence : Alligence }
type ExperienceComponent = { xp: TVar<int> }
     
type DrawnComponent = 
   { seen : Character
     explored : Character option }

type EquipmentComponent = 
   { weapon1 : EntityId
     weapon2 : EntityId
     equipped : Map<EquipableSlot, EntityId> }
   
type InventoryComponent = 
   | EmptyInventory of int
   | Inventory of int * Entity list
   | FullInventory of int * Entity list
     
type ItemComponent = 
   | Flavor
   | Usable of UseInfo * Charge option
   | Equipable of EquipableSlot
   | Weapon of EquipableSlot * WeaponInfo

type ManipulatableComponent = 
   | SelfOnly of Manipulation
   | RequiresItem of Manipulation

type Scene = { focusEntity : Entity; stairs : Entity; map : GameMap; level : int; entityStore : EntityStore }
type World = { scene : Scene; entities : EntityStore } // alligences, factions, world map, big overarching stuff