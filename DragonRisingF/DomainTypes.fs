#if INTERACTIVE
module DomainTypes
#else
module DragonRisingF.DomainTypes
#endif

open DraconicEngineF
open DisplayCore
open Entities
open FSharpx.Stm
open DragonRisingF.WorldState
open Akka.Actor
//open Akka.FSharp

let neutral = Alligence "neutral"

type Sense = 
   | Sight
   | Sound
   | Touch
   | Smell
   | Taste
   | Feeling
type Sensed = 
   { sense : Sense
     kind : string
     quality : string
     strength : int }





type SensoryEvent = { location : Loc; effects : Sensed list }
     
type AddConditionEvent = 
   { target : Entity
     newCondition : Condition
     duration : int option }

type ConditionExpiredEvent = 
   { entity : Entity
     expiringCondition : Condition }

type ConditionRemovedEvent = 
   { entity : Entity
     removedCondition : Condition }

type CreatureDamagedEvent = { creature : Entity; damage: Damage; damagingEntity: Entity option }
type CreatureKilledEvent = 
   { killedCreature : Entity
     cause : string option
     killingEntity : Entity option }

     
type AttackHitEvent = { attacker: Entity; target: Entity; weapon: Entity option; damage: Damage }
type AttackMissedEvent = { attacker: Entity; target: Entity; weapon: Entity option }

type CreatureHealedEvent = { creature : Entity; healer : Entity; amount : int }
     
type Fact =
   | SensoryFact of SensoryEvent
   | AttackHitResult of AttackHitEvent
   | AttackMissResult of AttackMissedEvent
   | CreatureDamage of CreatureDamagedEvent
   | CreatureKilled of CreatureKilledEvent
   | ConditionAddedFact of AddConditionEvent
   | ConditionExpired of ConditionExpiredEvent
   | ConditionRemoved of ConditionRemovedEvent
   | LaterEvent of int * Fact

type EffectId = int

type IntExpression =
   | ConstantInt of int
   | StatLookup of int
   | AddExpr of IntExpression * IntExpression
   | SubtractExpr of IntExpression * IntExpression
   | MultiplyExpr of IntExpression * IntExpression
   | DivideExpr of IntExpression * IntExpression
   | NegateExpr of IntExpression

type EntityFilter =
   | Any
   | OnlyCreaturesFilter
   | OnlyEnemiesFilter

type EffectBluePrint =
   | ApplyTemporaryCondition of string
   | DamageEffect of IntExpression
   | HealEffect of IntExpression
   | SensoryEffect of Sensed list

type QueryBlueprint =
   | AffectAllInRangeQuery of IntExpression * EntityFilter * QueryBlueprint list * EffectBluePrint list
   | SelectClosestCreatureQuery of IntExpression option * EntityFilter * QueryBlueprint list * EffectBluePrint list

type TargeterBlueprint =
   | AdjacentCreatureOrSelfTargeter of EntityFilter * TargeterBlueprint list * QueryBlueprint list * EffectBluePrint list
   | DirectionTargeter of DirectionLimit * TargeterBlueprint list * QueryBlueprint list * EffectBluePrint list
   | EntityInRangeTargeter of RangeLimits option * IntExpression * EntityFilter * TargeterBlueprint list * QueryBlueprint list * EffectBluePrint list
   | LocationInRangeTargeter of RangeLimits option * IntExpression * TargeterBlueprint list * QueryBlueprint list * EffectBluePrint list

type AreaEffect = (EffectId * (Area -> Fact list))
type EntityEffect = (EffectId * (Entity -> Fact list))
type LocationEffect = (EffectId * (Loc -> Fact list))

type Effect =
   | AreaEffect of AreaEffect
   | EntityEffect of EntityEffect
   | LocationEffect of LocationEffect
and NextEffect =
   | SimpleEffect of (Entity -> Effect)
   | IntToEffect of (int -> Entity -> Effect)
   | StringToEffect of (string -> Entity -> Effect)
and PowerFailure = string * Entity * NextEffect -> Effect


type PlanChoice = 
   | LocationChoice of Loc
   | EntityChoice of Entity
   | DirectionChoice of Direction
type FromLocation = Loc * PlanChoice list
type FromDirection = Direction * Loc * PlanChoice list

type FromLocationQuery = 
   | LocToLoc of (FromLocation -> Loc) * FromLocationQuery list * LocationEffect list
   | LocToEntity of (FromLocation -> EntityId) * FromLocationQuery list * EntityEffect list
   | LocToDirection of (FromLocation -> Direction) * FromDirecitonQuery
   | LocToArea of (FromLocation -> Area) * AreaEffect list
and FromDirecitonQuery = 
   | DirToLoc of (FromDirection -> Loc) * FromLocationQuery list * LocationEffect list
   | DirToEntity of (FromDirection -> Entity) * FromLocationQuery list * EntityEffect list
   | DirToArea of (FromDirection -> Area) * AreaEffect list

type FromLocationTargeter = 
   | TargetLocation of (FromLocation -> Async<Loc>) * FromLocationTargeter list * FromLocationQuery list * LocationEffect list
   | TargetDirection of (FromLocation -> Async<Direction>) * FromDirectionTargeter list * FromDirecitonQuery list
   | TargetEntity of (FromLocation -> Async<Entity>) * FromLocationTargeter list * FromLocationQuery list * EntityEffect list
and FromDirectionTargeter = 
   | TargetLocation of (FromDirection -> Async<Loc>) * FromLocationTargeter list * FromLocationQuery list * LocationEffect list
   | TargetEntity of (FromDirection -> Async<Entity>) * FromLocationTargeter list * FromLocationQuery list * EntityEffect list

type Plan = 
   { id: PlanId
     targeters : FromLocationTargeter list
     queries : FromLocationQuery list
     effects : EntityEffect list }

type FinalizedPlan = | FinalizedPlan of Plan * Fact list

type TimedEventsStore = TimedEventsStore of TVar<(Map<int, Fact list>)>

type MoveDetails = { initiator: Entity; direction : Direction }
type DropItemDetails = { initiator: Entity; itemToDrop : Entity }
type AttackEntityDetails = { initiator: Entity; target : Entity; weapon : Entity option }
type CastSpellDetails = { initiator: Entity; spell : Spell; finalizedPlan : FinalizedPlan }
type ManipulateEntityDetails = { initiator: Entity; target : Entity; item : Entity option }
type PickUpItemDetails = { initiator: Entity; itemToPick : Entity }
type UseItemDetails = { initiator: Entity; item : Entity; finalizedPlan : FinalizedPlan }

type ActionTaken = 
   | IdleAction
   | MoveAction of MoveDetails
   | DropItemAction of DropItemDetails
   | AttackEntityAction of AttackEntityDetails
   | CastSpellAction of CastSpellDetails
   | ManipulateEntityAction of ManipulateEntityDetails
   | PickUpItemAction of PickUpItemDetails
   | UseItemAction of UseItemDetails
   
type BehaviorMessage = 
   | PlanTurnMessage of World * AgentResponse<ActionTaken>
   | StopBehavior

type Behavior = IActorRef //Agent<BehaviorMessage>

type BehaviorLogic = Entity -> World -> ActionTaken

type BehaviorMaker = Entity -> Behavior

type BehaviorComponent = | BehaviorComponent of (string * Behavior) list
   
type CreatureLibrary = | CreatureLibrary of Map<string, Loc -> Entity> TVar
type ItemLibrary = | ItemLibrary of Map<string, unit -> Entity> TVar
type BehaviorLibrary = | BehaviorLibrary of Map<string, BehaviorMaker> TVar
type SpellLibrary = | SpellLibrary of Map<string, Spell> TVar
type TileLibrary = | TileLibrary of Map<TileId, TileType> TVar

type LevelUpBenefit =
| Constitution
| Strength
| Agility