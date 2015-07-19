module DragonRising.Generators

open DraconicEngineF.CoreObjects
open DraconicEngineF.Entities
open DraconicEngineF.ItemSelection
open DragonRisingF.DomainTypes
//open DragonRisingF.DomainFunctions

let createRoom makeClearTile { width = w; tiles = t } (room : TerminalRect) = 
   for y = room.Top + 1 to room.Bottom - 1 do
      let stride = y * w
      for x = room.Left + 1 to room.Right - 1 do
         t.[x + stride] <- makeClearTile()

let createHTunnel makeClearTile { width = w; tiles = t } x1 x2 y = 
   let stride = y * w
   for x = x1 to x2 do
      t.[x + stride] <- makeClearTile()

let createVTunnel makeClearTile { width = w; tiles = t } y1 y2 x = 
   for y = y1 to y2 do
      t.[x + y * w] <- makeClearTile()

let getRandomLocationInRoom (r: System.Random) (room: TerminalRect) =
   let x = r.Next(room.Left+1, room.Right)
   let y = r.Next(room.Top+1, room.Bottom)
   Loc (x, y)

let doEntityGeneration r level entityGen countPerLevel room =
   let count = fromDungeonLevel 0 level countPerLevel
   let rec gen i (acc: Entity list) =
      let location = getRandomLocationInRoom r room
      if (acc |> List.exists (fun e -> e.location = location)) then acc
      else
         let entity = entityGen level
         if i < count then
            gen (i+1) ({entity with location = location } :: acc)
         else acc
   gen 0 []
   
module DungeonGenerator = 
   let makeMap r popGen itemGen makeFillTile makeClearTile level width height = 
      let roomMaxSize = 10
      let roomMinSize = 6
      let maxRooms = 30
      let map = {width =width; height=height; tiles = Array.init (width*height) makeFillTile }
      
      let itemsPerRoomByLevel = 
         [ (1, 1)
           (2, 4) ]
      
      let monstersPerRoomByLevel = 
         [ (2, 1)
           (3, 4)
           (5, 6) ]
      
      let dungeonRoom = createRoom makeClearTile map
      let dungeonHTunnel = createHTunnel makeClearTile map
      let dungeonVTunnel = createVTunnel makeClearTile map
      let placeMonsters = doEntityGeneration r level popGen monstersPerRoomByLevel
      let placeItems = doEntityGeneration r level itemGen itemsPerRoomByLevel
      
      let makeRoom (rooms, monsters, items) newRoom = 
         dungeonRoom newRoom
         match rooms with
         | (prevRoom : TerminalRect) :: _ -> 
            let prevCenter = prevRoom.Center
            let currentCenter = newRoom.Center
            if r.Next(2) = 0 then 
               dungeonHTunnel prevCenter.X currentCenter.X prevCenter.Y
               dungeonHTunnel prevCenter.Y currentCenter.Y prevCenter.X
            else 
               dungeonHTunnel prevCenter.Y currentCenter.Y prevCenter.X
               dungeonHTunnel prevCenter.X currentCenter.X prevCenter.Y
         | [] -> ()
         let newMonsters = placeMonsters newRoom
         let newItems = placeItems newRoom
         newRoom :: rooms, List.append monsters newMonsters, List.append items newItems
      
      //for i = 0 to maxRooms do
      let rec makeRooms i (rooms, monsters, items) = 
         let w = r.Next(roomMinSize, roomMaxSize + 1)
         let h = r.Next(roomMinSize, roomMaxSize + 1)
         let x = r.Next(0, width - w - 1)
         let y = r.Next(0, height - h - 1)
         let newRoom = TerminalRect(x, y, w, h)
         
         let newRooms = 
            (if List.exists (fun room -> Intersects room newRoom) rooms = false then makeRoom (rooms, monsters, items) newRoom
             else (rooms, monsters, items))
         if i < maxRooms then makeRooms (i + 1) newRooms
         else newRooms
      
      let (allRooms, allMonsters, allItems) = makeRooms 0 ([], [], [])
      
      let seenC = { glyph = '<'; foreColor = white; backColor = None }
      let exploredC = { glyph = '<'; foreColor = white; backColor = None }
      let stairs = makeEntity "Stais" [ { seen = seenC; explored = Some exploredC } ] (List.head allRooms).Center
      
      let firstRoom = allRooms |> List.rev |> List.head

      (firstRoom.Center, stairs, allMonsters, allItems)
