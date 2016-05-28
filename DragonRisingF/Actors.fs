module Actors

open Akka.Actor
open Akka.FSharp
open DraconicEngineF
open DraconicEngineF.Game
open DraconicEngineF.Terminal
open DragonRisingF

type RenderMessage = | RenderMessage of TerminalBounds * IActorRef
type MessagesMessage = 
   | InfoMessage of RogueMessage
   | ClearInfoMessages
   | EventMessage of RogueMessage
   | RenderMessages of IActorRef

type InputMessage =
   | InputRequest of int

let maxEventMessages = 50

let messageActor =
   spawn actorSystem "Messages"
      (fun (mailbox: Actor<MessagesMessage>) ->
         let eventMessages = new System.Collections.Generic.Queue<RogueMessage>()
         
         let rec loop infoMsgs = actor {
            let! message = mailbox.Receive()
            let result = 
               match message with
               | InfoMessage msg -> msg :: infoMsgs
               | ClearInfoMessages -> []
               | EventMessage msg ->
                  eventMessages.Enqueue msg
                  if eventMessages.Count > maxEventMessages then
                     eventMessages.Dequeue() |> ignore
                  infoMsgs
               | RenderMessages rcv ->
                  rcv <! (eventMessages |> Seq.toList, infoMsgs)
                  infoMsgs
            return! loop result
         }
         loop [])

let makeInputActor inputStreams =
   spawn actorSystem "Input"
      (fun (mailbox: Actor<InputMessage>) ->
         let rec loop () = actor {
            let! message = mailbox.Receive()
            return! loop ()
         }
         loop ())
