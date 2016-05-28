#if INTERACTIVE
module Game
#else
module DraconicEngineF.Game
#endif

open FSharp.Control
open FSharpx.Collections
open Terminal
open InputTypes
open System.Threading
open System.Reactive.Subjects
open Akka.FSharp

let actorSystem = System.create "DraconicEngine" (Configuration.load())

let messengingService = Agent<MessageOp>.Start(fun inbox ->
   let rec loop messages = async{
      let! command = inbox.Receive ()
      let newMessages = 
         match command with
         | PostMessage rmsg -> (rmsg :: messages) |> List.truncate 10
         | GetMessages r ->
            r.Reply(messages)
            messages
      return! loop newMessages
      }
   loop [])

let rec getAllOfPriotity p queue soFar = 
   match PriorityQueue.tryPeek queue with
   | Some { triggerTime = tick; events = items } when tick = p -> 
      let (_, q') = PriorityQueue.pop queue
      getAllOfPriotity p q' (List.append soFar items)
   | _ -> (queue, soFar)

let makeTimedEventsService<'T> () =
   Agent<TimedEventMessage<'T>>.Start(fun inbox ->
   
      let rec loop (tick, events) = async {
         let! op = inbox.Receive ()
         let next =
            match op with
            | AddTimer timedEvents -> (tick, PriorityQueue.insert timedEvents events)
            | Tick r ->
               let newTick = tick+1
               let (q', currentItems) = getAllOfPriotity newTick events []
               r.Reply currentItems
               (newTick, q')

         return! loop next
         }
      loop (0, PriorityQueue.empty false)
      )

let private repositoryAgent initial (inbox : Agent<RepositoryOp<'K, 'T>>) = 
   let items = new System.Collections.Generic.Dictionary<'K, 'T>()
   
   let setMap() = 
      Lazy(fun () -> 
         items
         |> Seq.map (fun kvp -> (kvp.Key, kvp.Value))
         |> Map.ofSeq)
   
   let mutable asMap = setMap()
   for (k, v) in initial do
      items.Add(k, v)
   let rec messageLoop() = 
      async { 
         let! msg = inbox.Receive()
         let newMap = 
            match msg with
            | SetItem(k, v) -> 
               items.[k] <- v
               if asMap.IsValueCreated then asMap <- setMap()
            | RemoveItem k -> 
               items.Remove(k) |> ignore
               if asMap.IsValueCreated then asMap <- setMap()
            | GetItem(k, r) -> 
               let (found, item) = items.TryGetValue(k)
               r.Reply <| if found then Some item
                          else None
            | GetMap r -> asMap.Value |> r.Reply
         return! messageLoop()
      }
   messageLoop()

let makeRepository<'K, 'T when 'K : comparison> initial ct = 
   Agent<RepositoryOp<'K, 'T>>.Start(repositoryAgent initial, ct)

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

   let (resultFuture, result) = future<bool>() 
   let rec inputLoop () = async {
      let! {key= key; modifiers=modifiers} = inputStreams.keyDown |> obsToAsync CancellationToken.None
      let input = 
         match key with
         | RogueKey.Y -> Some true
         | RogueKey.N -> Some false
         | _ -> None
      match input with
      | Some r -> 
         resultFuture r
         return TickFinished
      | None -> return! inputLoop ()
      }
   let draw () = async {
      Terminal.clear dialogPanel
      Terminal.drawBox dialogPanel Terminal.doubleLines
      let margin = 2
      for (i, line) in Seq.indexed lines do
         dialogPanel.[margin, margin+i] |> Terminal.writeCharacterString line
      return ()
      }
   
   { doLogic = inputLoop; drawInfo = { draw = draw; kind = PartialScreen } }, result

let drawAgent = Agent.Start (fun inbox ->
   let rec drawLoop state = 
      async {
      let! msg = inbox.Receive ()
      let! newStates = 
         match msg with
         | Draw r -> 
            match state with
            | None -> async { return state }
            | Some (DrawStack (s, f, _)) -> 
               async {
                  do! s.draw ()
                  for f' in f do
                     do! f'.draw ()
                  r.Reply()
                  return state
               }
         | AddDrawInfo newState -> 
            async { return 
               match newState.kind with
               | WholeScreen -> Some <| DrawStack (newState, [], state)
               | PartialScreen -> 
                  match state with
                  | Some (DrawStack (s, f, b)) -> Some <| DrawStack (s, newState :: f, b)
                  | None -> failwith "Tried to add a floater state without a background screen" 
            }
         | PopDrawInfo -> 
            async { 
               return 
                  match state with
                  | Some (DrawStack (s, f, b)) -> 
                     match f with
                     | h :: t -> Some <| DrawStack (s, t, b)
                     | [] -> b
                  | None -> None }

      return! drawLoop newStates
      }
   drawLoop None
   )

let startDraw present frameTime (ct: CancellationToken) = async {
   let watch = new System.Diagnostics.Stopwatch();
   while ct.IsCancellationRequested = false do
      watch.Restart()
      do! drawAgent.PostAndAsyncReply(fun r -> Draw r)
      present ()
      watch.Stop()
      let sleepTime = max 10 (frameTime - (int watch.ElapsedMilliseconds))
      do! Async.Sleep sleepTime
   }
let runGameState { doLogic = tick; drawInfo = drawInfo } =
   do drawAgent.Post <| AddDrawInfo drawInfo
   async {
      let! result = tick ()
      drawAgent.Post PopDrawInfo
      return result
   }

//let runResultGameState { getResult = getResult; drawInfo = drawInfo } =
//   do drawAgent.Post <| AddDrawInfo drawInfo
//   async {
//      let! result = getResult ()
//      drawAgent.Post PopDrawInfo
//      return result
//   }

//let runGameStateSeq { ticks = ticks; drawInfo = drawInfo } =
//   do drawAgent.Post <| AddDrawInfo drawInfo
//   async {
//      for tickResult in ticks() do
//         if tickResult = TickFinished then return ()
//   }