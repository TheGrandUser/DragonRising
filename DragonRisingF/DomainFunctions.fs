#if INTERACTIVE
module DomainFunctions
#else
module DragonRisingF.DomainFunctions
#endif

open DraconicEngineF
open Game
open Entities
open DomainTypes
open FSharpx.Stm
open System
open WorldState
open Akka.FSharp
open Akka.Actor

//let creatureRepo = makeEntityPropMap<CreatureComponent>()

//let setCreatureComp = setComponent creatureRepo
//let getCreatureComp = getComponent creatureRepo
//let removeCreatureComp = removeComponent creatureRepo

//let inventoryRepo = makeEntityPropMap<InventoryComponent>()

let sendToBack scene entity = ()

let removeFrom list item =
   match list |> List.tryFindIndex (fun i -> i = item) with
   | Some index -> 
     (match List.splitAt index list with
      | (front, h :: []) -> front
      | (front, h :: t) -> List.append front t
      | (front, []) -> front) |> Some
   | None -> None

let makeTile id = { tileId = id; visibility = NotSeen }
let voidId = TileId 0
let beyondTheEdge = makeTile voidId

let isCreature e =
   match getComponent<CreatureComponent> e with
   | Some c -> true
   | None -> false
let isLivingCreature e =
   match getComponent<CreatureComponent> e with
   | Some c -> c.isAlive
   | None -> false

let doesEntityBlock { blocks = b} =  b

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

let areCreaturesEnemies e1 e2 =
   match (getComponent<CreatureComponent> e1, getComponent<CreatureComponent> e1) with
   | (Some creature1, Some creature2) -> areEnemies creature1.alligence creature2.alligence
   | _ -> false


let getTile (scene: Scene) (location: Loc) =
   if location.X < 0 || location.X > scene.map.width || location.Y < 0 || location.Y > scene.map.width 
      then Some scene.map.tiles.[location.X + location.Y * scene.map.width]
      else None

let getEntities (EntityStore e) = stm { return! e |> readTVar } |> atomically

let getEntitiesAtLocation (scene: Scene) location =
   let entities = getEntities scene.entityStore
   entities |> List.filter (fun e -> getLocation e = location)

let isTileBlocked scene location =
   let t = getTile scene location
   match t with
   | None -> OffMap
   | Some tile ->
      let tileType = Map.find tile.tileId scene.map.tileTypes
      if tileType.blocksMovement then BlockingTile tileType
      else
         match (getEntities scene.entityStore) |> List.tryFind (fun e -> getLocation e = location && getBlocks e) with
            | Some entity -> BlockingEntity entity
            | None -> NoBlock

let isTileVisible (scene: Scene) (location: Loc) =
   match getTile scene location with
   | None -> false
   | Some tile -> tile.visibility = Seen



let moveOr something scene self direction =
   let moveVec = Vector(direction)
   let newLoc = moveVec + getLocation self
   match isTileBlocked scene newLoc with
   | NoBlock -> MoveAction <| { initiator = self; direction = moveVec.ToDirection.Value }
   | blocked -> something blocked

let moveOrIdle = moveOr (fun _ -> IdleAction)

let moveOrAttack scene self = 
   let isAnEnmy = areCreaturesEnemies self
   moveOr (fun blockage -> 
      match blockage with
      | BlockingEntity e when isAnEnmy e -> AttackEntityAction { initiator = self; target = e; weapon = None } 
      | _ -> IdleAction) scene self

let tryManyMoves decider self targetLocation =
   let rec getAction = function
      | [] -> IdleAction
      | moveVec :: rest -> 
         let dir = (toDirection moveVec).Value
         let action = decider self dir
         match action with
         | IdleAction -> getAction rest
         | a -> a
   targetLocation - getLocation self |> pathFindAttempts |> getAction

let makeBehavior (behavior: BehaviorLogic) (self: Entity) =
   let (EntityId id) = self.id
   let name = sprintf "Behavior-%i" id
   spawn actorSystem name (fun inbox ->
      let selfedBehavior = behavior self
      let rec loop () = actor {
         let! msg = inbox.Receive ()

         match msg with 
         | PlanTurnMessage (world, response) ->
            let action = selfedBehavior world
            response.Reply action
            return! loop ()
         | StopBehavior -> return ()
         }
      loop ())
let stopBehavior (b: Behavior) = b <! StopBehavior

let clearAnyBehaviors e =
   match getComponent<BehaviorComponent> e with
   | Some (BehaviorComponent behaviors) ->
      behaviors |> List.iter (fun (_, b) -> stopBehavior b)
   | None -> ()

let basicMonsterBehavior self world =
   let location = getLocation self
   if isTileVisible world.scene location then
      let player = world.scene.focusEntity
      if areAdjacent location <| getLocation player 
         then AttackEntityAction { initiator = self; target = player; weapon = None }
         else tryManyMoves (moveOrAttack world.scene) self <| getLocation player
   else IdleAction

let makeBasicMonsterBehavior = makeBehavior basicMonsterBehavior

let confusedBehavior (r: System.Random) self world =
   let result = r.Next(100)
   if result < 70 then
      let direction = getRandomDirection r
      moveOrIdle world.scene self direction
   else IdleAction

let makeConfusedBehavior r = confusedBehavior r |> makeBehavior

let justPassBehavior self world = IdleAction
let makeJustPassBehavior = justPassBehavior |> makeBehavior



let addEntity (EntityStore entityStore) e =
   stm {
      let! entities = readTVar entityStore
      return if List.exists (fun e' -> e = e') entities then false
             else stm { do! e :: entities |> writeTVar entityStore } |> atomically
                  true
   } |> atomically


let removeEntity (EntityStore entityStore) e =
   stm {
      let! entities = readTVar entityStore
      return! match removeFrom entities e with
              | Some newList -> stm {
               do! writeTVar entityStore newList
               return true
               }
              | None -> stm { return false }
   } |> atomically


let isBlocked scene = false
let isBlockedForEntity scene e = false

let onlyCreaturesFilter (_: Entity) target = isCreature target
let onlyEnemiesFilter user target = areCreaturesEnemies user target

let normalDamage a = { amount = a; kind = "Normal" }

let asTrue _ = true
let asFalse _ = true

let removeFromInventory e item =
   match getComponent<InventoryComponent> e with
   | Some (EmptyInventory capacity) -> false
   | Some (Inventory (capacity, items))
   | Some (FullInventory (capacity, items)) -> 
      match removeFrom items item with
      | Some [] -> setComponent e <| EmptyInventory (capacity) |> asTrue
      | Some newItems -> setComponent e <| Inventory (capacity, newItems) |> asTrue
      | None -> false
   | None -> false

let tryAddToInventory e item =
   match getComponent<InventoryComponent> e with
   | Some (EmptyInventory 1) -> setComponent e FullInventory (1, [item]) |> asTrue
   | Some (EmptyInventory capacity) -> setComponent e Inventory (capacity, [item]) |> asTrue
   | Some (Inventory (capacity, items)) when items.Length + 1 = capacity -> setComponent e <| FullInventory (capacity, item :: items) |> asTrue
   | Some (Inventory (capacity, items)) -> setComponent e <| Inventory (capacity, item :: items) |> asTrue
   | Some (FullInventory (capacity, items)) -> false
   | None -> false

let processFinalizedPlan (plan: FinalizedPlan): Fact list =
   []

let addEventFromNow (TimedEventsStore upcomingEvents) duration facts =
   ()

let getBehavior (BehaviorLibrary behaviors) name = 
   let b' = readTVar behaviors |> atomically
   b'.TryFind name

let inventoryCount = function
   | EmptyInventory c -> 0
   | Inventory (c, items) -> List.length items
   | FullInventory (c, items) -> c
   