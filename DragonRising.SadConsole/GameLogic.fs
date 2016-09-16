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
   | _ -> false
let isInWorld map (location: Loc) =
   let (x,y) = location.X, location.Y
   x >= 0 && x < map.width && y >= 0 && y < map.height
let isMovementBlocked world location = 
   if isInWorld world.map location.xy then
      let (x,y) = location.xy.X, location.xy.Y
      doesTileBlockMovement world.map.tiles.[location.layer].[x,y].t
   else true
let otherCreatureAtPoint meId world loc =
   let found = Map.tryFind loc world.locations.idByLoc
   found |> Option.filter 
      (fun eId -> 
         eId <> meId && world.creatures.ContainsKey eId)

let canMoveInto id world location =
   if isInWorld world.map location.xy then
      let (x,y) = location.xy.X, location.xy.Y
      let tileBlocks = doesTileBlockMovement world.map.tiles.[location.layer].[x,y].t
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


let getLocs size loc dir =
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
   let newLoc = { loc with xy = loc.xy + dir }

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

let findRandomStartingSpot map size =
   let checkSpot loc =
      let tiles = map.tiles.[loc.layer]
      let toCheck = getLocs size loc Vector.Zero
      toCheck |> List.forall (fun c ->
         if not (isInWorld map c.xy) then false
         else
            let tile = tiles.[c.xy.X, c.xy.Y]
            tile.t <> Wall && tile.t <> OpenSpace)
   let rec iter () =
      let layer = mainRandom.Next(map.tiles.Length-1)
      let x = mainRandom.Next(map.width)
      let y = mainRandom.Next(map.height)
      let loc = { xy = Loc(x,y); layer = layer }
      if checkSpot loc then loc
      else iter ()
   iter ()
   
let makeElf id loc =
   id, loc, {id = id; name = "Carnie"; race = elfRace}

let createMap width height =
   let world = WorldGeneration.createRegions 10 10
   let region = WorldGeneration.createLocalTerrain world 6 4 width height
   region, findRandomStartingSpot region Huge , List.init 20 (fun i -> makeElf (EntityId (i+2)) (findRandomStartingSpot region Medium))

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