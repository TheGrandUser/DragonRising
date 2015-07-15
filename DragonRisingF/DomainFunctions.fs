module DragonRisingF.DomainFunctions
open DraconicEngineF.CoreObjects
open DraconicEngineF.Entities
open DragonRisingF.DomainTypes


let creatureRepo = makeEntityPropMap<CreatureComponent>()

let setCreatureComp = setComponent creatureRepo
let getCreatureComp = getComponent creatureRepo
let removeCreatureComp = removeComponent creatureRepo

let inventoryRepo = makeEntityPropMap<InventoryComponent>()
