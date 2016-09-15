module Consoles

open System
open FSharpx
open Microsoft.FSharp.Linq
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

open SadConsole
open SadConsole.Consoles
open SadConsole.Controls
open SadConsole.Game
open SadConsole.Input

open GameTypes
open GameLogic
open ActionTypes

let checkPressed (info: KeyboardInfo) = AsciiKey.Get >> info.KeysPressed.Contains
let checkDown (info: KeyboardInfo) = AsciiKey.Get >> info.KeysDown.Contains



let checkDirection diag info =
   let checkPressed = checkPressed info
   if checkPressed Keys.Down || checkPressed Keys.NumPad2 then new Vector(0, 1) |> Some
   elif checkPressed Keys.Up || checkPressed Keys.NumPad8 then new Vector (0, -1) |> Some
   elif checkPressed Keys.Left || checkPressed Keys.NumPad4 then new Vector (-1, 0) |> Some
   elif checkPressed Keys.Right || checkPressed Keys.NumPad6 then new Vector (1, 0) |> Some
   elif diag then
      if checkPressed Keys.NumPad1 || checkPressed Keys.End then new Vector (-1, 1) |> Some
      elif checkPressed Keys.NumPad7 || checkPressed Keys.Home then new Vector (-1, -1) |> Some
      elif checkPressed Keys.NumPad3 || checkPressed Keys.PageDown then new Vector (1, 1) |> Some
      elif checkPressed Keys.NumPad9 || checkPressed Keys.PageUp then new Vector (1, -1) |> Some
      else None
   else None


let button width label click =
   let button = new Button(width, 1)
   button.Text <- label
   button.ButtonClicked.Add click
   button

let arrangeButtons sWidth sHeight (offset: Point) (buttons) =
   let height = List.length buttons + 3 * 2 + 1
   let width = 4 + (buttons |> List.map (fun (t: string, _, _) -> t.Length) |> List.max)
   let x = (sWidth - width) / 2 + offset.X
   let y = sHeight - height + offset.Y
   buttons |> List.mapi (fun i (t, h, e) -> 
      let b = button width t h
      b.IsEnabled <- e
      b)


let raceToGlyph (r: Race) =
   if r = dragonRace then int 'D', Color.ForestGreen
   elif r = elfRace then int 'e', Color.LavenderBlush
   else int '?', Color.Magenta

let createTextSurfaceBySize x y glyph color =
   let animation = new AnimatedTextSurface("default", x, y, Engine.DefaultFont)
   let frame = animation.CreateFrame()
   for cell in frame.Cells do
      cell.GlyphIndex <- glyph
      cell.Foreground <- color
      cell.Background <- Color.Black
   animation
      

let createGameObject state id creature =
   let glyph, color = raceToGlyph creature.race
   let go = new GameObject()
   let loc = getLocation id state.locations
   go.Position <- new Point(loc.X, loc.Y)
   let x,y = match creature.race.size with | Small | Medium -> 1,1 | Large -> 2,2 | Huge -> 3,3
   let animation = createTextSurfaceBySize x y glyph color
   go.Animation <- animation
   go

let floorGlyph () = if mainRandom.Next 4 = 0 then int ',' else int '.'

let getGlyphForTile tileType =
   match tileType with
   | TileType.Wall -> int '#', Color.DarkGray
   | TileType.Floor -> floorGlyph (), Color.LightGray

let setTileGlyph (console: Console) x y tile =
   let glyph, color = getGlyphForTile tile.t
   console.SetGlyph(x, y, glyph, color)

type MapConsole (screenWidth, screenHeight, state) as this =
   inherit Console(state.map.width, state.map.height)
   let mutable state = state
   do this.TextSurface.RenderArea <- new Rectangle(0,0,screenWidth, screenHeight)

   let mutable gameObjects = state.creatures |> Map.map (createGameObject state)
   
   do state.map.tiles |> Array2D.iteri (setTileGlyph this)
   
   let screenOffset = Vector(screenWidth/2, screenHeight/2)
   member this.UpdateState newState =
      state <- newState
      let playerLoc = getLocation state.playerId state.locations
      let viewPos = playerLoc - screenOffset
      this.TextSurface.RenderArea <- 
         new Rectangle(
            max viewPos.X 0,
            max viewPos.Y 0,
            this.TextSurface.RenderArea.Width,
            this.TextSurface.RenderArea.Height)
      gameObjects.[state.playerId].Position <- new Point(playerLoc.X, playerLoc.Y)
      gameObjects |> Map.iter (fun _ go -> 
         go.IsVisible <- this.TextSurface.RenderArea.Contains go.Position
         go.RenderOffset <- this.Position - this.TextSurface.RenderArea.Location)
      ()

   override this.Render() =
      base.Render()
      gameObjects |> Map.iter (fun _ go -> go.Render())

type MessageConsole (width, height) =
   inherit Console(width, height)

   member this.PrintMessage (text: string) =
      this.ShiftDown(1)
      this.VirtualCursor.Print(text).CarriageReturn() |> ignore
   member this.PrintMessage (text: ColoredString) =
      this.ShiftDown(1)
      this.VirtualCursor.Print(text).CarriageReturn() |> ignore

let headerConsole width (text: string) =
   let console = new Console(width, 1)
   console.DoUpdate <- false
   console.CanUseKeyboard <- false
   console.CanUseMouse <- false
   console.Fill(Nullable (), Nullable Color.Black, Nullable 196, Nullable ()) |> ignore
   //console.SetGlyph (56, 0, 193)
   console.Print (2, 0, text)
   console

let messageConsole width height =
   let console = MessageConsole(width, height)
   console

type GameConsole(screenWidth, screenHeight, state) as this =
   inherit ConsoleList()//state.map.width, state.map.height)

   let mapConsole = MapConsole(screenWidth, screenHeight - 10, state)
   let messageConsole = messageConsole screenWidth 8
   let messageHeaderConsole = headerConsole screenWidth "Messages"

   do messageConsole.Position <- new Point(0, screenHeight - 9)
   do messageHeaderConsole.Position <- new Point(0, screenHeight - 10)

   do this.Add mapConsole
   do this.Add messageConsole
   do this.Add messageHeaderConsole

   do Engine.Keyboard.RepeatDelay <- 0.07f
   do Engine.Keyboard.InitialRepeatDelay <- 0.1f

   let mutable state = state

   let updateState newWorld =
      if newWorld <> state then
         state <- newWorld
         
         mapConsole.UpdateState(state)
         true
      else false

   override this.ProcessKeyboard info =
      match checkDirection true info with
      | Some dir ->
         let playerLoc = getLocation state.playerId state.locations
         let moveResult = GameLogic.tryMove state state.playerId playerLoc dir
         match moveResult with
         | Blocked -> 
            messageConsole.PrintMessage "You can't go that way"
            false
         | MoveTo newLoc -> 
            let newState = moveEntityTo state state.playerId newLoc
            updateState newState
         | AttackEntity targetId ->
            let newState, msgs = attackEntity state state.playerId targetId
            msgs |> List.iter (fun (LogMessage msg) -> messageConsole.PrintMessage msg)
            updateState newState
      | None -> false





let gameConsole width height state =
   let console = GameConsole(width, height, state)

   console



let createNewGame (console: Console) s =
   console.Print(3, 3, "Starting a new game", Color.Green)
   ()

let loadGame (console: Console) s =
   console.Print(3, 5, "Loading an old game", Color.Green)
   ()

let continueGame (console: Console) s =
   console.Print(3, 7, "Continueing the prior game", Color.Green)
   ()

let setEnabled e (b: Button) = b.IsEnabled <- e; b

let mainMenuConsole savedGames width height =
   let console = new ControlsConsole(width, height)
   let anySaves = (savedGames |> List.isEmpty |> not)
   let buttons =
      [ "Continue Game", (continueGame console), anySaves
        "New Game", (createNewGame console), true
        "Load Game", (loadGame console), anySaves]
      |> arrangeButtons width height (new Point(0, -5))

   buttons |> List.iter console.Add

   console

//type MainMenuConsole (width, height) as this=
//   inherit ControlsConsole(width, height)
//   do this.Add (button width "New Game" createNewGame)