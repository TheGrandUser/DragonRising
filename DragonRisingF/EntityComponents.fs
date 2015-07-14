module DragonRisingF.EntityComponents
open DraconicEngineF.CoreObjects
open DraconicEngineF.Entities
open DragonRisingF.Plans

type Behavior = PlayerControled | Normal | Agressive | Confused
type Condition = Confused

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
