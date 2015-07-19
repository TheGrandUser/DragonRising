module DragonRisingF.DomainFunctions
open DraconicEngineF.CoreObjects
open DraconicEngineF.Entities
open DragonRisingF.DomainTypes
open FSharpx.Stm

//let creatureRepo = makeEntityPropMap<CreatureComponent>()

//let setCreatureComp = setComponent creatureRepo
//let getCreatureComp = getComponent creatureRepo
//let removeCreatureComp = removeComponent creatureRepo

//let inventoryRepo = makeEntityPropMap<InventoryComponent>()

let alligenceCTS = new System.Threading.CancellationTokenSource();

let alligenceManager = makeRepository<string, Alligence> [("Neutral", neutral)] alligenceCTS.Token
let alligenceRelationships = makeRepository<Alligence * Alligence, IFF> [] alligenceCTS.Token

let getRelationship k = 
   let getter = makeGetter alligenceRelationships
   match getter k with
   | Some r -> r
   | None -> Neutral

let areEnemies alligence1 alligence2 =
   if alligence1 = neutral || alligence2 = neutral then false
   else getRelationship (alligence1, alligence2) = Enemy


let addEntity (EntityStore entityStore) e =
   stm {
      let! entities = readTVar entityStore
      return if List.exists (fun e' -> e = e') entities then false
             else stm { do! e :: entities |> writeTVar entityStore } |> atomically
                  true
   } |> atomically

let isBlocked scene = false
let isBlockedForEntity scene e = false