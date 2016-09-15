// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open SadConsole
open SadConsole.Game
open Consoles

open DragonRising
open GameTypes
open GameLogic

let screenWidth = 80
let screenHeight = 60

let engineStart s =

   let game = createNewGameWorld "Adraka" 100 100

   let mainConsole = gameConsole screenWidth screenHeight game
   Engine.ConsoleRenderStack.Clear()
   Engine.ConsoleRenderStack.Add(mainConsole)
   Engine.ActiveConsole <- mainConsole

[<EntryPoint>]
let main argv = 
   SadConsole.Engine.Initialize ("Cheepicus12.font", screenWidth, screenHeight)
   SadConsole.Engine.EngineStart.Add engineStart
   SadConsole.Engine.Run ()
   0 // return an integer exit code
