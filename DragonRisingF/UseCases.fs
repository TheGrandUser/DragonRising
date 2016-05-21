module DragonRisingF.UseCases
open FSharpx.Collections
open DraconicEngineF
open DraconicEngineF.Terminal
open DraconicEngineF.Entities
open DraconicEngineF.InputTypes
open DragonRisingF.WorldState
open DragonRisingF.DomainTypes
open DragonRisingF.DomainFunctions
open DragonRisingF.UITypes
open FSharp.Control

let all _ = true
let asAsync<'T> t = async { return t }

let turnAction a = (Some a, Some TurnAdvancing)
let noTurn = (None, None)
let endGame = (None, Some FinalTurn)

let selectEntityInRange filter range player: Async<Entity option> = async { return None }
let selectItemFromInventory itemFilter player: Async<Entity option> =
   //let inventory 
   async { return None }

let getCommandAction player command = 
   match command with
   | WaitCommand -> turnAction IdleAction |> asAsync
   | QuitCommand -> endGame |> asAsync
   | Abort -> noTurn |> asAsync
   | MovementCommand dir -> turnAction (MoveAction { initiator = player; direction = dir }) |> asAsync
   | InteractWithCreatureCommand e -> noTurn |> asAsync
   | AttackCommand e -> turnAction (AttackEntityAction { initiator = player; target=e; weapon = None}) |> asAsync
   | DropItemCommand -> async {
      let! item = selectItemFromInventory all player
      return match item with
               | Some i -> turnAction (PickUpItemAction { initiator = player; itemToPick = i })
               | None -> noTurn
      }
   | GoDownCommand -> noTurn |> asAsync
   | LookCommand -> noTurn |> asAsync
   | ManipulateCommand e -> noTurn |> asAsync
   | PickUpItemCommand -> noTurn |> asAsync
   | UseItemCommand itemFilter -> noTurn |> asAsync

let getPanels terminal compact items =
   let spacingPerItem = if compact then 1 else 2
   let messageY = if compact then 0 else 1
   let firstItemY = if compact then 1 else 3
   let pageYFromBottom = if compact then 0 else 1
   let messageArea = messageY + if compact then 1 else 2
   let pageArea = pageYFromBottom + 1
   let buffer = if compact then 4 else 5
   let mainSize = (Terminal.getSize terminal)
   let availableItemHeight = mainSize.Y - messageArea
   let ipp = if availableItemHeight / spacingPerItem < List.length items then
               (availableItemHeight - pageArea) / spacingPerItem
               else availableItemHeight / spacingPerItem
   (ipp, terminal |> Terminal.fromPoint 3 messageY, terminal |> Terminal.fromPoint 3 firstItemY, terminal |> Terminal.fromPoint 3 (mainSize.Y - pageYFromBottom))

let cancellableMenuWidget getCommand terminal message compact items =
   do if (Terminal.getSize terminal).Y < (if compact then  7 else 11) then failwith "The given terminal is too small, must be at least 11 spaces high"
   let spacingPerItem = if compact then 1 else 2
   let (itemsPerPage, messagePanel, itemsPanel, pagePanel) = getPanels terminal compact items
   let pageCount = 
      let l = List.length items
      (l / itemsPerPage) + if l % itemsPerPage = 0 then 0 else 1
   
   let mutable currentMenuItem = items |> List.findIndex (fun {canExecute = can} -> can())
   let mutable currentMenuPage = currentMenuItem / itemsPerPage

   let gestures = [
      CommandGesture (makeSimpleGestures [RogueKey.Up; RogueKey.NumPad8], MenuNavigation.Up)
      CommandGesture (makeSimpleGestures [RogueKey.Down; RogueKey.NumPad2], MenuNavigation.Down)
      CommandGesture (makeSimpleGestures [RogueKey.Enter; RogueKey.Space], MenuNavigation.Select)
      CommandGesture (makeSimpleGestures [RogueKey.Escape], MenuNavigation.Cancel)
      ]

   let logic () = 
      let validItems = 
         items 
         |> List.mapi (fun i mi -> (i, mi))
         |> List.filter (fun (i, mi) -> mi.canExecute())
         |> List.map (fun (i, _) -> i)
      let currentValidIndex = validItems |> List.findIndex (fun i -> i = currentMenuItem)
      
      async {
         let! result =
            asyncSeq {
               while true do
                  let! inputResult = getCommand gestures
                  yield 
                     match inputResult with
                     | InputResultSingle (MenuNavigation.Up) ->
                        let nextIndex = max 0 (currentValidIndex - 1)
                        NoResult
                     | InputResultSingle (MenuNavigation.Down) ->
                        let nextIndex = min (validItems.Length - 1) (currentValidIndex + 1)
                        currentMenuItem <- validItems |> List.item (nextIndex)
                        NoResult
                     | InputResultSingle (MenuNavigation.Select) -> SomeResult (items |> List.item currentMenuItem).value
                     | InputResultSingle (MenuNavigation.Cancel) -> CancelledResult
                     | _ -> NoResult }
            |> AsyncSeq.skipWhile(fun ir -> match ir with | SomeResult r -> false | _ -> true)
            |> AsyncSeq.map (fun ir -> match ir with | SomeResult r -> Some r | _ -> None)
            |> AsyncSeq.tryFirst
         return Option.bind (fun o -> o) result
      }

   let draw () =
      messagePanel |> Terminal.write message
      items 
      |> List.iteri (fun i item -> 
         let color = 
            match (item.canExecute(), currentMenuItem = i) with
            | (true, true) -> RogueColors.white
            | (true, false) -> RogueColors.lightGray
            | _ -> RogueColors.gray
         itemsPanel |> Terminal.fromPoint 0 (i * spacingPerItem) |> Terminal.withForeColor color |> write item.name)
      if pageCount > 1 then
         pagePanel |> write (sprintf "Page %i/%i" currentMenuPage pageCount)
   (logic, draw)

let menuWidget getCommand terminal message compact items =
   do if (Terminal.getSize terminal).Y < (if compact then  7 else 11) then failwith "The given terminal is too small, must be at least 11 spaces high"
   let spacingPerItem = if compact then 1 else 2
   let (itemsPerPage, messagePanel, itemsPanel, pagePanel) = getPanels terminal compact items
   let pageCount = 
      let l = List.length items
      (l / itemsPerPage) + if l % itemsPerPage = 0 then 0 else 1
   
   let mutable currentMenuItem = items |> List.findIndex (fun {canExecute = can} -> can())
   let mutable currentMenuPage = currentMenuItem / itemsPerPage

   let gestures = [
      CommandGesture (makeSimpleGestures [RogueKey.Up; RogueKey.NumPad8], MenuNavigation.Up)
      CommandGesture (makeSimpleGestures [RogueKey.Down; RogueKey.NumPad2], MenuNavigation.Down)
      CommandGesture (makeSimpleGestures [RogueKey.Enter; RogueKey.Space], MenuNavigation.Select)
      ]

   let logic () = 
      let validItems = 
         items 
         |> List.mapi (fun i mi -> (i, mi))
         |> List.filter (fun (i, mi) -> mi.canExecute())
         |> List.map (fun (i, _) -> i)
      let currentValidIndex = validItems |> List.findIndex (fun i -> i = currentMenuItem)
      
      async {
         let! result = 
            asyncSeq {
               while true do
                  let! inputResult = getCommand gestures
                  yield 
                     match inputResult with
                     | InputResultSingle (MenuNavigation.Up) ->
                        let nextIndex = max 0 (currentValidIndex - 1)
                        None
                     | InputResultSingle (MenuNavigation.Down) ->
                        let nextIndex = min (validItems.Length - 1) (currentValidIndex + 1)
                        currentMenuItem <- validItems |> List.item (nextIndex)
                        None
                     | InputResultSingle (MenuNavigation.Select) -> Some (items |> List.item currentMenuItem).value
                     | _ -> None }
            |> AsyncSeq.skipWhile (fun r -> r = None)
            |> AsyncSeq.choose (fun r -> r)
            //|> AsyncSeq.choose (fun r -> r)
            |> AsyncSeq.tryFirst 
         return result.Value
         }

   let draw () =
      messagePanel |> Terminal.write message
      items 
      |> List.iteri (fun i item -> 
         let color = 
            match (item.canExecute(), currentMenuItem = i) with
            | (true, true) -> RogueColors.white
            | (true, false) -> RogueColors.lightGray
            | _ -> RogueColors.gray
         itemsPanel |> Terminal.fromPoint 0 (i * spacingPerItem) |> Terminal.withForeColor color |> write item.name)
      if pageCount > 1 then
         pagePanel |> write (sprintf "Page %i/%i" currentMenuPage pageCount)
   (logic, draw)

let gameEndScreen getKeyPress rootTerminal =
   let message = "Game Over (Press Enter to continue)"
   let messageSize = Vector(message.Length + 4, 5)
   let messagePosition = (lowerRight rootTerminal - messageSize) / 2
   let messageTerminal = rootTerminal |> Terminal.withRegion messagePosition.X messagePosition.Y messageSize.X messageSize.Y |> Terminal.withColor RogueColors.white RogueColors.black

   let draw () = 
      Terminal.clear messageTerminal
      Terminal.drawBox messageTerminal doubleLines
      messageTerminal |> Terminal.fromPoint 2 2 |> Terminal.write message
      async { return () }

   let doLogic () = async {
      while true do
         let! (keyEvent, mods) = getKeyPress()
         if keyEvent = RogueKey.Enter && mods = RogueModifierKeys.None then 
            return ()
      }
      
   { doLogic = doLogic; drawInfo = { kind = PartialScreen; draw = draw} }

let inventoryScreen getKeyPress rootTerminal (player: Entity) inventory message =
   let header = match message with
                | Some msg -> player.name + "' inventory: " + msg
                | None -> player.name + "' inventory"
   let itemsTerminal = 
      let size = (Terminal.getSize rootTerminal - Vector(3, 4))
      rootTerminal |> Terminal.withRegion 3 4 size.X size.Y

   let headerPos = Loc((rootTerminal.Size.X - header.Length) / 2, 1)
   let headerTerminal = rootTerminal |> Terminal.fromPoint headerPos.X headerPos.Y

   let draw () = 
      Terminal.clear rootTerminal
      Terminal.drawBox rootTerminal Terminal.doubleLines
      headerTerminal |> Terminal.write header 

      match inventory with
      | EmptyInventory _ -> itemsTerminal |> Terminal.write "Inventory is empty."
      | Inventory (c, items)
      | FullInventory (c, items) ->
         let skipLines = items.Length < itemsTerminal.Size.Y / 2
         items |> Seq.truncate 26 |> Seq.iteri (fun i item -> 
            let text = sprintf "(%c) %s" (char i + 'a') item.name
            ())
         ()
      async { return () }

   let (resultFuture, result) = future<int option>() 
   let doLogic () = 
      asyncSeq {
         while true do
            let! (keyEvent, mods) = getKeyPress()
            yield keyEvent }
      |> AsyncSeq.takeWhile (fun k -> k <> RogueKey.Escape)
      |> AsyncSeq.map (fun k -> int (k - RogueKey.A))
      |> AsyncSeq.filter(fun i -> i >= 0 && i < (inventoryCount inventory))
      |> AsyncSeq.tryFirst
   { doLogic = doLogic; drawInfo = { kind = WholeScreen; draw = draw } }

let levelUpScreen getCommand rootTerminal {power = power; defense = defense} {maxHP = maxHP} =
   
   let (menuLogic, menuDraw) = 
      let terminal = rootTerminal |> fromPoint 10 5
      let items = [
         simpleMenuItem (sprintf "Constitution (+20 hp from %i)" maxHP) Constitution
         simpleMenuItem (sprintf "Strength (+1 attack from %i)" power) Strength
         simpleMenuItem (sprintf "Agility (+1 defense from %i)" defense) Agility
         ]
      menuWidget getCommand terminal "'Level up! Choose a stat to raise:" false items

   let (resultFuture, result) = future<LevelUpBenefit>()
   let draw () =
      Terminal.clear rootTerminal
      menuDraw()
      async { return () }
   let doLogic () = async {
      let! result = menuLogic()
      return result }

   { doLogic = doLogic;  drawInfo = { kind = WholeScreen; draw = draw } }

let loadGameScreen getCommand rootTerminal saveNames =
   let message = "Please select a save game"
   let savesTerminal =
      let height = List.length saveNames + 3*2+3
      let width = ((message :: saveNames) |> List.maxBy (fun m -> String.length m) |> String.length) + 3*2
      let (screenWidth, _) = Terminal.getTerminalSize rootTerminal
      let x = (screenWidth - width) / 2
      rootTerminal |> Terminal.withRegion x 10 width height

   let (resultFuture, result) = future<string option>()
   
   let (menuLogic, menuDraw) = 
      let items = saveNames |> List.map (fun s -> simpleMenuItem s s)
      cancellableMenuWidget getCommand savesTerminal message false items

   let draw () =
      Terminal.clear rootTerminal
      menuDraw()
      async { return () }
   let doLogic () = async {
      let! result = menuLogic()
      return result }

   { doLogic = doLogic; drawInfo = { kind = WholeScreen; draw = draw } }, result


let loadDataScreen rootTerminal loaders =
   let messageTerminal =
      let (screenWidth, screenHeight) = Terminal.getTerminalSize rootTerminal
      let height = 7
      let width = loaders |> List.maxBy (fun (m, _) -> String.length m) |> fst |> String.length
      let x = (screenWidth - width) / 2
      let y = screenHeight - height - 5
      rootTerminal |> Terminal.withRegion x y width height |> Terminal.withForeColor RogueColors.lightGray
   let mutable message = "Loading..."
   let draw () =
      Terminal.clear rootTerminal
      Terminal.write message messageTerminal
      async { return () }
   let mutable queue = Queue.ofList loaders
   let doLogic () = async {
      for (m, loader) in loaders do
         message <- m
         do! loader()
      }

   { doLogic = doLogic; drawInfo = { kind = WholeScreen; draw = draw } }

type TargetAction = | Cycle | Accept | Cancel | Point
let selectEntityTool getCommand sceneTerminal viewOffset availableEntities =
   let commands = [
      CommandGesture1D (fourWayMove, toCycle, (fun _ -> Cycle))
      CommandGesture (makeSimpleGestures [RogueKey.Escape], Cancel)
      ]
   let doLogic () = async { return () }
   let draw () = async { return () }
   { doLogic = doLogic; drawInfo = { kind = PartialScreen; draw = draw; } }

let targetingTool getCommand rootTerminal =
   let doLogic () = async { return () }
   let draw () = async { return () }
   { doLogic = doLogic; drawInfo = { kind = PartialScreen; draw = draw; } }

let newGameScreen getCommand rootTerminal =
   let doLogic () = async { return () }
   let draw () = async { return () }
   { doLogic = doLogic; drawInfo = { kind = WholeScreen; draw = draw; } }

let playingScreen getCommand rootTerminal saveManager world =
   let doLogic () = async { return () }
   let draw () = async { return () }
   { doLogic = doLogic; drawInfo = { kind = WholeScreen; draw = draw; } }

let mainMenuScreen getCommand rootTerminal saveManager =
   let myPlayingScreen = playingScreen getCommand rootTerminal saveManager
   let (menuLogic, menuDraw) = 
      let items = [
         menuItem "Continue" ContinueGame (fun () -> DataAccess.lastSavedGame saveManager |> Option.isSome)
         simpleMenuItem "New Game" NewGame
         menuItem "Continue" LoadGame (fun () -> DataAccess.getSavedGames saveManager |> Seq.isEmpty |> not)
         simpleMenuItem "Exit" Exit
         ]
      let height = items.Length + 3*2+1
      let width = items |> List.map (fun i -> i.name.Length) |> List.max
      let (x, y) = 
         let (w, h) = rootTerminal |> Terminal.getTerminalSize
         (w - width)/2, h - height-5
      let terminal = rootTerminal |> Terminal.withRegion x y width height
      menuWidget getCommand terminal "" false items

   let doLogic () = async { return () }
   let draw () = async { return () }
   { doLogic = doLogic; drawInfo = { kind = WholeScreen; draw = draw; } }