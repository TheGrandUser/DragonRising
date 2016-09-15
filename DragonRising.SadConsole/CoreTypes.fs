[<AutoOpen>]
module CoreTypes

open System
open FSharpx.Collections
open FSharp.Control
open Microsoft.Xna.Framework

let future<'T> () =
   let tcs = new System.Threading.Tasks.TaskCompletionSource<'T>()
   let finish x = tcs.SetResult(x)
   let ta = Async.AwaitTask tcs.Task
   (finish, ta)

type Agent<'T> = MailboxProcessor<'T>
type AgentResponse<'T> = AsyncReplyChannel<'T>

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
   
   static member (+) (l : Point, v : Vector) = Point(l.X + v.X, l.Y + v.Y)
   static member (+) (v : Vector, l : Point) = Point(l.X + v.X, l.Y + v.Y)
   static member (-) (l : Point, v : Vector) = Point(l.X - v.X, l.Y - v.Y)
   
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

let toDirection (v: Vector) = v.ToDirection

let getRandomDirection (r: System.Random) =
   let i = r.Next(8)
   match i with
   | 0 -> East
   | 1 -> Southeast
   | 2 -> South
   | 3 -> Southwest
   | 4 -> West
   | 5 -> Northwest
   | 6 -> North
   | 7 -> Southeast
   | 8 -> Northeast
   | _ -> East

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
   static member Zero = Vector(0, 0)
   static member Add(a : Vector, b : Vector) = a + b

let toLoc (v:Vector) = Point(v.X, v.Y)
let toVector (l:Point) = Vector(l.X, l.Y)
   
type RangeLimits = 
   | LineOfEffect
   | LineOfSight
   | Both

type SelectionRange = 
   { range : int option
     limits : RangeLimits option }

type TerminalRect = { position: Point; size: Vector } with 
   member this.X = this.position.X
   member this.Y = this.position.Y
   member this.Width = this.size.X
   member this.Height = this.size.Y
   member this.Left = this.X
   member this.Top = this.Y
   member this.Right = this.X + this.Width
   member this.Bottom = this.Y + this.Height
   member this.TopLeft = Point(this.Left, this.Top)
   member this.TopRight = Point(this.Right, this.Top)
   member this.BottomLeft = Point(this.Left, this.Bottom)
   member this.BottomRight = Point(this.Right, this.Bottom)
   member this.Center = Point((this.Left + this.Right) / 2, (this.Top + this.Bottom) / 2)
   member this.Area = this.size.X * this.size.Y
   static member Row s = { position = Point(); size = Vector(s, 1) }
   static member Row(x, y, s) = { position = Point(x, y); size = Vector(s, 1) }
   static member Row(pos, s) = { position = pos; size = Vector(s, 1) }
   static member Column s = { position = Point(0, 0); size= Vector(1, s)}
   static member Column(x, y, s) = { position = Point(x, y); size = Vector(1, s) }
   static member Column(pos, s) = { position = pos; size = Vector(1, s) }

let rect x y w h = { position = Point(x,y); size = Vector(w,h) }

type Area = 
   | Circle of Point * int
   | Rectangle of Point * Vector
   | Cone of Point * Vector * int * int
   | Combined of Area * Area

type RogueMessage = RogueMessage of string * Color
type MessageOp =
   | PostMessage of RogueMessage
   | GetMessages of AgentResponse<RogueMessage list>


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

type TimedEventMessage<'T> =
   | AddTimer of TimedEvents<'T>
   | Tick of AgentResponse<'T list>
   
type GameViewType = | WholeScreen | PartialScreen
type TickResult = | TickContinue | TickFinished
type OutOfTurnEvent = | RefreshDisplay | DoesNothing | CloseState

type DrawingInfo = { kind: GameViewType; draw: (unit -> Async<unit>) }
type GameState<'T> = { doLogic: (unit -> Async<'T>); drawInfo: DrawingInfo }

type StateDrawAgentOp =
   | Draw of unit AgentResponse
   | AddDrawInfo of DrawingInfo
   | PopDrawInfo
type DrawStack = DrawStack of DrawingInfo * DrawingInfo list * DrawStack option

let lengthSq (p: Point) = p.X * p.X + p.Y * p.Y

let IsDistanceWithin (a : Point) (b : Point) (distance : int) = 
   let offset = a - b
   lengthSq offset <= distance * distance

let IsInRectangle (point1 : Point) (point2 : Point) (point : Point) = 
   let minX = min point1.X point2.X
   let maxX = max point1.X point2.X
   let minY = min point1.Y point2.Y
   let maxY = max point1.Y point2.Y
   minX <= point.X && point.X <= maxX && minY <= point.Y && point.Y <= maxY

let isInTerminalRect { position = p; size = s } point = IsInRectangle p (p + s) point

type Point with
   member this.LengthSquard = this.X * this.X + this.Y * this.Y
   member this.RookLength = abs this.X + abs this.Y
   member this.KingLength = max (abs this.X) (abs this.Y)
   //static member (-) (l1 : Point, l2 : Point) = Vector(l1.X - l2.X, l1.Y - l2.Y)
   //static member (/) (l : Point, i) = Point(l.X / i, l.Y / i)

let areAdjacent (a : Point) (b : Point) = 
   let diff = a - b
   diff.KingLength = 1

let getLineFromTo (a : Point) (b : Point) = 
   let dx = abs (a.X - b.X)
   let dy = abs (a.Y - b.Y)
   let sx = sign (b.X - a.X)
   let sy = sign (b.Y - a.Y)
   Seq.append (Seq.singleton a) (Seq.unfold 
      (fun (err, x0, y0) -> 
         if x0 = b.X && y0 = b.Y then None
         else 
            let e2 = err * 2           
            let (err2, x1) = 
               if e2 > -dy then (err - dy, x0 + sx)
               else (err, x0)
            if x1 = b.X && y0 = b.Y then Some(Point(x1, y0), (err2, x1, y0))
            else 
               let (err3, y1) = 
                  if e2 < dx then (err2 + dx, y0 + sy)
                  else (err2, y0)
               Some(Point(x1, y1), (err3, x1, y1))) (dx - dy, a.X, a.Y))

let adjustForBounds { position = p; size=s} (pos: Point) =
   Point(p.X + pos.X, p.Y+pos.Y)

let Intersection (a : TerminalRect) (b : TerminalRect) = 
   let left = max a.Left b.Left
   let right = min a.Right b.Right
   let top = max a.Top b.Top
   let bottom = min a.Bottom b.Bottom
   let width = max 0 (right - left)
   let height = max 0 (bottom - top)
   rect left top width height

let Intersects (a : TerminalRect) (b : TerminalRect) = 
   a.Left <= b.Right && a.Right >= b.Left && a.Top <= b.Bottom && a.Bottom >= b.Top
let CenterIn (main : TerminalRect) (toCenter : TerminalRect) = 
   { position = main.position + (main.size - toCenter.size) / 2; size = toCenter.size }
   
let makeSubBounds parentBounds (childBounds: TerminalRect) =
   let pos = adjustForBounds parentBounds childBounds.position
   { position = pos; size = childBounds.size } |> Intersection parentBounds

let originPos = Point()
let offsetX x (l : Point) = Point(l.X + x, l.Y)
let offsetY y (l : Point) = Point(l.X, l.Y + y)
let offset (x, y) (l : Point) = Point(l.X+x, l.Y+y)

let getPositionsInRect (rect : TerminalRect) = 
   seq { 
      for y = rect.Top to rect.Bottom do
         for x = rect.Left to rect.Right do
            yield Point(x, y)
   }

let private rand = new System.Random();
let swap (a: _[]) x y =
   let tmp = a.[x]
   a.[x] <- a.[y]
   a.[y] <- tmp

let shuffle a =
   for n = Array.length a-1 downto 0 do
      let i = rand.Next(0, n)
      let t = a.[n]
      a.[n] <- a.[i]
      a.[i] <- t
   a

let randomPairOrder a b = if rand.Next(2) = 0 then [a; b] else [b; a]

let pathFindAttempts v =
   if v = Vector(0,0) then []
   else
      let (absX, absY) = (abs v.X, abs v.Y)
      let (sX, sY) = (sign v.X, sign v.Y)
      let (main, secondaries, tertiaries) = 
         match (absX, absY) with
         | (x, 0) -> (Vector(sX, 0), randomPairOrder (Vector(sX, 1)) (Vector(sX, -1)), randomPairOrder (Vector(0, 1)) (Vector(0, -1)))
         | (x, y) when x > y -> (Vector(sX, 0), [Vector(sX, sY)], [Vector(0, sY)])
         | (0, y) -> (Vector(0, sY), randomPairOrder (Vector(1, sY)) (Vector(-1, sY)), randomPairOrder (Vector(1, 0)) (Vector(-1, 0)))
         | (x, y) when x < y -> (Vector(0, sY), [Vector(sX, sY)], [Vector(sX, 0)])
         | (x, y) -> (Vector(sX, sY), [Vector(sX, 0); Vector(0, sY)], [])
      main :: secondaries |> List.append tertiaries

