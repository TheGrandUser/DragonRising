[<AutoOpen>]
#if INTERACTIVE
module CoreObjects
#else
module DraconicEngineF.CoreObjects
#endif

open System
open TryParser
open FSharpx.Collections

type Agent<'T> = MailboxProcessor<'T>
type AgentResponse<'T> = AsyncReplyChannel<'T>

//let compGroup = AsyncGroup
type RepoReply<'T> = AgentResponse<'T option>

type RepositoryOp<'K, 'T when 'K : comparison> = 
   | SetItem of 'K * 'T
   | GetItem of 'K * RepoReply<'T>
   | GetMap of AgentResponse<Map<'K, 'T>>
   | RemoveItem of 'K

let makeSetter (d : Agent<RepositoryOp<'K, 'T>>) = 
   let setter k v = SetItem(k, v) |> d.Post
   setter

let makeMapGetter (d : Agent<RepositoryOp<'K, 'T>>) = 
   let getter() = d.PostAndReply(fun r -> GetMap r)
   getter

let makeGetter (d : Agent<RepositoryOp<'K, 'T>>) = 
   let getter k = d.PostAndReply(fun r -> GetItem(k, r))
   getter

let makeGetterAsync (d : Agent<RepositoryOp<'K, 'T>>) = 
   let getter k = d.PostAndAsyncReply(fun r -> GetItem(k, r))
   getter

let makeRemover (d : Agent<RepositoryOp<'K, 'T>>) = 
   let remover k = RemoveItem k |> d.Post
   remover

let private repsoitoryAgent initial (inbox : Agent<RepositoryOp<'K, 'T>>) = 
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
   Agent<RepositoryOp<'K, 'T>>.Start(repsoitoryAgent initial, ct)


type Stuff = Todo

type Direction = 
   | East
   | Southeast
   | South
   | Southwest
   | West
   | Northwest
   | North
   | Northeast

type DirectionLimit = 
   | Eightway
   | Cardinal
   | FullVector

[<Struct>]
type Vector(x : int, y : int) = 
   member this.X = x
   member this.Y = y
   member this.LengthSquard = this.X * this.X + this.Y * this.Y
   member this.Length = sqrt (double (this.X * this.X + this.Y * this.Y))
   member this.RookLength = abs this.X + abs this.Y
   member this.KingLength = max (abs this.X) (abs this.Y)
   override this.ToString() = this.X.ToString() + ", " + this.Y.ToString()
   static member (+) (v1 : Vector, v2 : Vector) = Vector(v1.X + v2.X, v1.Y + v2.Y)
   static member (-) (a : Vector, b : Vector) = Vector(a.X - b.X, a.Y - b.Y)
   static member (/) (v : Vector, i) = Vector(v.X / i, v.Y / i)
   
   new(d : Direction) = 
      match d with
      | East -> Vector(1, 0)
      | Southeast -> Vector(1, 1)
      | South -> Vector(0, 1)
      | Southwest -> Vector(-1, 0)
      | West -> Vector(-1, 0)
      | Northwest -> Vector(-1, -1)
      | North -> Vector(0, -1)
      | Northeast -> Vector(1, -1)
   
   member this.ToDirection = 
      let sx = sign x
      let sy = sign y
      match (sx, sy) with
      | (1, 0) -> Some(East)
      | (1, 1) -> Some(Southeast)
      | (0, 1) -> Some(South)
      | (-1, 1) -> Some(Southwest)
      | (-1, 0) -> Some(West)
      | (-1, -1) -> Some(Northwest)
      | (0, -1) -> Some(North)
      | (1, -1) -> Some(Northeast)
      | (_, _) -> None

[<Struct>]
type Loc(x : int, y : int) = 
   member this.X = x
   member this.Y = y
   override this.ToString() = this.X.ToString() + ", " + this.Y.ToString()
   static member (+) (l : Loc, v : Vector) = Loc(l.X + v.X, l.Y + v.Y)
   static member (+) (v : Vector, l : Loc) = Loc(l.X + v.X, l.Y + v.Y)
   static member (-) (l : Loc, v : Vector) = Loc(l.X - v.X, l.Y - v.Y)
   static member (-) (l1 : Loc, l2 : Loc) = Vector(l1.X - l2.X, l1.Y - l2.Y)
   static member (/) (l : Loc, i) = Loc(l.X / i, l.Y / i)

type Vector with
   member this.Zero = Vector(0, 0)
   member this.Add(a : Vector, b : Vector) = a + b

type RangeLimits = 
   | LineOfEffect
   | LineOfSight
   | Both

type SelectionRange = 
   { range : int option
     limits : RangeLimits option }

let IsDistanceWithin (a : Loc) (b : Loc) (distance : int) = 
   let offset = a - b
   offset.LengthSquard <= distance * distance

let IsInRectangle (point1 : Loc) (point2 : Loc) (point : Loc) = 
   let minX = min point1.X point2.X
   let maxX = max point1.X point2.X
   let minY = min point1.Y point2.Y
   let maxY = max point1.Y point2.Y
   minX <= point.X && point.X <= maxX && minY <= point.Y && point.Y <= maxY

let AreAdjacent (a : Loc) (b : Loc) = 
   let diff = a - b
   diff.KingLength = 1

let getLineFromToOld (a : Loc) (b : Loc) = 
   let dx = abs (a.X - b.X)
   let dy = abs (a.Y - b.Y)
   let sx = sign (b.X - a.X)
   let sy = sign (b.Y - a.Y)
   let length = (max dx dy) - 1
   
   let folder s i = 
      let (err, x0, y0) = s
      let e2 = err * 2
      
      let err2 = 
         if e2 > -dy then err - dy
         else err
      
      let x1 = 
         if e2 > -dy then x0 + sx
         else x0
      
      if x1 = b.X && y0 = b.Y then (err2, x1, y0)
      else 
         let err3 = 
            if e2 < dx then err2 + dx
            else err2
         
         let y2 = 
            if e2 < dx then y0 + sy
            else y0
         
         (err3, x1, y2)
   [ 0..length ]
   |> List.scan folder (dx - dy, a.X, a.Y)
   |> List.map (fun (_, x, y) -> Loc(x, y))

let getLineFromTo (a : Loc) (b : Loc) = 
   let dx = abs (a.X - b.X)
   let dy = abs (a.Y - b.Y)
   let sx = sign (b.X - a.X)
   let sy = sign (b.Y - a.Y)
   Seq.append (Seq.singleton a) (Seq.unfold (fun (err, x0, y0) -> 
                                    if x0 = b.X && y0 = b.Y then None
                                    else 
                                       let e2 = err * 2
                                       
                                       let (err2, x1) = 
                                          if e2 > -dy then (err - dy, x0 + sx)
                                          else (err, x0)
                                       if x1 = b.X && y0 = b.Y then Some(Loc(x1, y0), (err2, x1, y0))
                                       else 
                                          let (err3, y1) = 
                                             if e2 < dx then (err2 + dx, y0 + sy)
                                             else (err2, y0)
                                          Some(Loc(x1, y1), (err3, x1, y1))) (dx - dy, a.X, a.Y))

type TerminalRect(position, size) = 
   new(x, y, width, height) = TerminalRect(Loc(x, y), Vector(width, height))
   member this.Position = position
   member this.Size = size
   member this.X = position.X
   member this.Y = position.Y
   member this.Width = size.X
   member this.Height = size.Y
   member this.Left = this.X
   member this.Top = this.Y
   member this.Right = this.X + this.Width
   member this.Bottom = this.Y + this.Height
   member this.TopLeft = Loc(this.Left, this.Top)
   member this.TopRight = Loc(this.Right, this.Top)
   member this.BottomLeft = Loc(this.Left, this.Bottom)
   member this.BottomRight = Loc(this.Right, this.Bottom)
   member this.Center = new Loc((this.Left + this.Right) / 2, (this.Top + this.Bottom) / 2)
   member this.Area = size.X * size.Y
   static member Row s = TerminalRect(0, 0, s, 1)
   static member Row(x, y, s) = TerminalRect(x, y, s, 1)
   static member Row(pos, s) = TerminalRect(pos, Vector(s, 1))
   static member Column s = TerminalRect(0, 0, 1, s)
   static member Column(x, y, s) = TerminalRect(x, y, 1, s)
   static member Column(pos, s) = TerminalRect(pos, Vector(1, s))
   member this.Contains point = IsInRectangle position (position + size) point

let adjustForBounds (bounds: TerminalRect) (pos: Loc) =
   Loc(bounds.Position.X + pos.X, bounds.Position.Y+pos.Y)

let Intersection (a : TerminalRect) (b : TerminalRect) = 
   let left = max a.Left b.Left
   let right = min a.Right b.Right
   let top = max a.Top b.Top
   let bottom = min a.Bottom b.Bottom
   let width = max 0 (right - left)
   let height = max 0 (bottom - top)
   TerminalRect(left, top, width, height)

let Intersects (a : TerminalRect) (b : TerminalRect) = 
   a.Left <= b.Right && a.Right >= b.Left && a.Top <= b.Bottom && a.Bottom >= b.Top
let CenterIn (main : TerminalRect) (toCenter : TerminalRect) = 
   TerminalRect(main.Position + (main.Size - toCenter.Size) / 2, toCenter.Size)
   
let makeSubBounds parentBounds (childBounds: TerminalRect) =
   let pos = adjustForBounds parentBounds childBounds.Position
   TerminalRect(pos, childBounds.Size) |> Intersection parentBounds 


type Area = 
   | Circle of Loc * int
   | Rectangle of Loc * Vector
   | Cone of Loc * Vector * int * int
   | Combined of Area * Area

let originPos = Loc()
let offsetX x (l : Loc) = Loc(l.X + x, l.Y)
let offsetY y (l : Loc) = Loc(l.X, l.Y + y)
let offset (x, y) (l : Loc) = Loc(l.X+x, l.Y+y)

let getPositionsInRect (rect : TerminalRect) = 
   seq { 
      for y = rect.Top to rect.Bottom do
         for x = rect.Left to rect.Right do
            yield Loc(x, y)
   }


type MessageOp =
   | PostMessage of string * RogueColor
   | GetMessages of AgentResponse<(string * RogueColor) list>

let messengingService = Agent<MessageOp>.Start(fun inbox ->
   let rec loop messages = async{
      let! command = inbox.Receive ()
      let newMessages = 
         match command with
         | PostMessage (msg, color) -> ((msg, color) :: messages) |> List.truncate 10
         | GetMessages r ->
            r.Reply(messages)
            messages
      return! loop newMessages
      }
   loop [])


#nowarn "0342"
[<CustomComparison>]
[<StructuralEquality>]
type TimedEvents<'T> = { triggerTime: int; events: 'T list }
   with 
   interface System.IComparable with
      member x.CompareTo y = 
         match y with
         | :? TimedEvents<'T> as y' -> x.triggerTime - y'.triggerTime
         | _ -> 0
   end

let rec getAllOfPriotity p queue soFar = 
   match PriorityQueue.tryPeek queue with
   | Some { triggerTime = tick; events = items } when tick = p -> 
      let (_, q') = PriorityQueue.pop queue
      getAllOfPriotity p q' (List.append soFar items)
   | _ -> (queue, soFar)

type TimedEventMessage<'T> =
   | AddTimer of TimedEvents<'T>
   | Tick of AgentResponse<'T list>

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