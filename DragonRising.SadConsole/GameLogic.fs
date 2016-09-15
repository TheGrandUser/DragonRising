module GameLogic

open GameTypes
open ActionTypes
open FSharpx
open SadConsole


let mainRandom = new System.Random()

let getLocation id locations =
   Map.find id locations.locById

let doesTileBlockMovement tt =
   match tt with
   | Wall -> true
   | Floor -> false
let isInWorld world (location: Loc) =
   let (x,y) = location.X, location.Y
   x >= 0 && x < world.map.width && y >= 0 && y < world.map.height
let isMovementBlocked world (location: Loc) = 
   if isInWorld world location then
      let (x,y) = location.X, location.Y
      doesTileBlockMovement world.map.tiles.[x,y].t
   else true
let otherCreatureAtPoint meId world loc =
   let found = Map.tryFind loc world.locations.idByLoc
   found |> Option.filter 
      (fun eId -> 
         eId <> meId && world.creatures.ContainsKey eId)

let canMoveInto id world location =
   if isInWorld world location then
      let (x,y) = location.X, location.Y
      let tileBlocks = doesTileBlockMovement world.map.tiles.[x,y].t
      if tileBlocks then false
      else
         let entityBlocks = 
            match Map.tryFind location world.locations.idByLoc with
            | None -> false
            | Some eId ->
               eId <> id && world.creatures.ContainsKey eId
         not entityBlocks
   else false

let updateLocation locations id l =
   let oldLoc = locations.locById.[id]
   let newByLoc = 
      locations.idByLoc
      |> Map.remove oldLoc
      |> Map.add l id
   let newById = 
      locations.locById
      |> Map.remove id
      |> Map.add id l
   { locById = newById; idByLoc = newByLoc }


let getLocs size (loc: Loc) dir =
   let unitX = Vector(1,0)
   let unitY = Vector(0,1)
   let unitXY = Vector(1,1)
   match size with
   | Small
   | Medium -> [loc]
   | Large -> 
      
      [loc; loc + unitX; loc + unitY; loc+unitXY]
   | Huge ->
      seq {
         for y = 0 to 2 do
            for x = 0 to 2 do
               yield loc + (Vector (x,y))
      } |> Seq.toList
            

let tryMove world id loc (dir: Vector) =
   let size = 
      Map.tryFind id world.creatures 
      |> Option.map (fun c -> c.race.size)
      |> Option.getOrElse Medium
   let newLoc = loc + dir

   let locsToCheck = getLocs size newLoc dir 

   let impediment = locsToCheck |> List.exists (isMovementBlocked world)
   if impediment then Blocked
   else
      let others = locsToCheck |> List.choose (otherCreatureAtPoint id world)
      match others with
      | eId :: rest -> AttackEntity (ConfirmedAttack eId)
      | [] -> MoveTo (ConfirmedMove newLoc)

let moveEntityTo world id (ConfirmedMove newLoc) =
   { world with locations = updateLocation world.locations id newLoc }

let attackEntity world meId (ConfirmedAttack targetId) =
   let creature = world.creatures.[targetId]
   let msg = sprintf "You try to eat the %s, but the code to open your jaws is not written" creature.race.name
   world, [LogMessage (ColoredString(msg))]
let moveEntity world id dir =
   let l = Map.tryFind id world.locations.locById
   match l with
   | Some location -> 
      let size = 
         Map.tryFind id world.creatures 
         |> Option.map (fun c -> c.race.size)
         |> Option.getOrElse Medium
      let newLoc = location + dir
      
      if getLocs size newLoc dir |> List.forall (canMoveInto id world) then
         { world with locations = updateLocation world.locations id newLoc }
      else world
   | None -> world



let dragonRace = { name = "Dragon"; size = Huge }
let elfRace = { name = "Elf"; size = Medium }

let makeElf id x y =
   id, Loc(x,y), {id = id; name = "Carnie"; race = elfRace}

let createMap width height =
   let chooseTile x y = if x = 0 || x = width - 1 || y = 0 || y = height - 1 then Wall else Floor
   let tiles = 
      Array2D.init width height 
         (fun x y -> { t = chooseTile x y; seen = true })
   let map = { tiles = tiles; width = width; height = height }
   map, Loc(10,10), [makeElf (EntityId 2) 15 20]

let createLocations idLocs =
   let locById = Map.ofList idLocs
   let idByLoc = idLocs |> List.map(fun (i,l) -> l, i) |> Map.ofList
   { locById = locById; idByLoc = idByLoc }

let createNewGameWorld playerName width height =
   
   let playerId = EntityId 1
   let playerCreature = {name = playerName; race = dragonRace; id = playerId }

   let map, start, creatures = createMap width height

   let idLoc = creatures |> List.map(fun (i,l, _) -> (i,l))
   let idCreature = creatures |> List.map(fun (i,_,c) -> (i,c))

   let creatures = (playerId, playerCreature) :: idCreature |> Map.ofList

   let state = {
      playerId = playerId
      creatures = creatures
      locations = (playerId, start) :: idLoc |> createLocations
      map = map
      }
   state