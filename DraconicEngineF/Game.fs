#if INTERACTIVE
module Game
#else
module DraconicEngineF.Game
#endif

open Terminal
open InputTypes
open System.Threading

type GameViewType = | Screen | Dialog | Tool | Effect
type TickResult = | TickContinue | TickFinished
type OutOfTurnEvent = | RefreshDisplay | DoesNothing | CloseState

let CreateConfirmDialog (inputStreams: InputStreams) (hostPanel: Terminal) msg =
   let message = msg + " (y / n)"
   let (lines, dialogPanel) =
      let boarderSize = 4
      let maxSize = hostPanel.Size.X - boarderSize
      let ls = 
         let fullStr = new CharacterString(message)
         if message.Length > maxSize
         then fullStr.WordWrap maxSize
         else [new CharacterString(message)]
      let width = ls |> Seq.map (fun line -> line.Count) |> Seq.max |> (+) boarderSize
      let height = ls.Length
      let size = Vector(width, height)
      let position = (hostPanel.LowerRight - size) / 2
      (ls, hostPanel.[position, size].[RogueColors.white, Some RogueColors.black])

   let rec inputLoop () = async {
      let! (key, modifiers) = inputStreams.keyDown |> obsToAsync CancellationToken.None
      let input = 
         match key with
         | RogueKey.Y -> Some true
         | RogueKey.N -> Some false
         | _ -> None
      match input with
      | Some r -> return (TickFinished, r)
      | None -> return! inputLoop ()
      }
   let draw () = async {
      Terminal.clear dialogPanel
      Terminal.drawBox dialogPanel Terminal.doubleLines
      let margin = 2
      for (i, line) in Seq.indexed lines do
         Terminal.writeCharacterString dialogPanel.[margin, margin+i] line
      return ()
      }
   (Dialog, inputLoop, draw)