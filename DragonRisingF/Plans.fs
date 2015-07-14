module DragonRisingF.Plans
open DraconicEngineF.CoreObjects
open DraconicEngineF.Entities

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

type Targeter = 
| TargetLocation of int
| TargetDirection
| TargetEntity of int

type Plan = { targeters: Targeter list; queries: FromLocationQuery list; effects: EntityOrLocationEffect list }
