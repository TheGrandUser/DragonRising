module DraconicEngineF.CoreObjects
open System
open TryParser

//let compGroup = AsyncGroup

type RepoReply<'T> = AsyncReplyChannel<'T option>
type RepositoryOp<'K,'T when 'K: comparison> =
| SetItem of 'K * 'T
| GetItem of 'K * RepoReply<'T>
| GetMap of AsyncReplyChannel<Map<'K, 'T>>
| RemoveItem of 'K

let makeSetter (d: MailboxProcessor<RepositoryOp<'K, 'T>>) =
   let setter k v = SetItem (k, v) |> d.Post
   setter

let makeMapGetter (d: MailboxProcessor<RepositoryOp<'K, 'T>>)=
   let getter () = d.PostAndReply (fun r -> GetMap r)
   getter
let makeGetter (d: MailboxProcessor<RepositoryOp<'K, 'T>>) =
   let getter k = d.PostAndReply (fun r-> GetItem (k, r))
   getter

let makeGetterAsync (d: MailboxProcessor<RepositoryOp<'K, 'T>>) =
   let getter k = d.PostAndAsyncReply (fun r-> GetItem (k, r))
   getter

let makeRemover (d: MailboxProcessor<RepositoryOp<'K, 'T>>) =
   let remover k = RemoveItem k |> d.Post
   remover

let makeRepository<'K, 'T when 'K: comparison> initial ct = 
   MailboxProcessor<RepositoryOp<'K, 'T>>.Start((fun inbox ->

      let items = new System.Collections.Generic.Dictionary<'K, 'T> ()
      let setMap () = Lazy (fun () -> items |> Seq.map (fun kvp -> (kvp.Key, kvp.Value))  |> Map.ofSeq)
      let mutable asMap = setMap ()
      for (k, v) in initial do items.Add(k, v)

      let rec messageLoop () = async {
         let! msg = inbox.Receive()
         let newMap =
            match msg with
            | SetItem (k, v) -> items.[k] <- v
                                if asMap.IsValueCreated then asMap <- setMap ()
            | RemoveItem k -> items.Remove(k) |> ignore
                              if asMap.IsValueCreated then asMap <- setMap ()
            | GetItem (k, r) ->
                  let (found, item) = items.TryGetValue(k)
                  r.Reply <| if found then Some item else None
            | GetMap r -> asMap.Value |> r.Reply 

         return! messageLoop ()
      }

      messageLoop ()
   ), ct)

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
type Vector(x: int, y: int) =
   member this.X = x
   member this.Y = y

   member this.LengthSquard = this.X * this.X + this.Y * this.Y
   member this.Length = sqrt(double (this.X * this.X + this.Y * this.Y))
   member this.RookLength = abs this.X + abs this.Y
   member this.KingLength = max (abs this.X) (abs this.Y)
   override this.ToString() = this.X.ToString() + ", " + this.Y.ToString()
   static member (+) (v1: Vector, v2: Vector) = Vector(v1.X + v2.X, v1.Y + v2.Y)
   static member (-) (a: Vector, b: Vector) = Vector(a.X - b.X, a.Y - b.Y)
   static member (/) (v: Vector, i) = Vector(v.X / i, v.Y / i)
   new (d: Direction) =
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
      | ( 1,  0) -> Some(East)
      | ( 1,  1) -> Some(Southeast)
      | ( 0,  1) -> Some(South)
      | (-1,  1) -> Some(Southwest)
      | (-1,  0) -> Some(West)
      | (-1, -1) -> Some(Northwest)
      | ( 0, -1) -> Some(North)
      | ( 1, -1) -> Some(Northeast)
      | ( _,  _) -> None


[<Struct>]
type Loc(x: int, y: int) =
   member this.X = x
   member this.Y = y
   override this.ToString() = this.X.ToString() + ", " + this.Y.ToString()
   static member (+) (l: Loc, v: Vector) = Loc(l.X + v.X, l.Y + v.Y)
   static member (+) (v: Vector, l: Loc) = Loc(l.X + v.X, l.Y + v.Y)
   static member (-) (l: Loc, v: Vector) = Loc(l.X - v.X, l.Y - v.Y)
   static member (-) (l1: Loc, l2: Loc) = Vector(l1.X - l2.X, l1.Y - l2.Y)

type Vector with
   member this.Zero = Vector(0, 0)
   member this.Add (a: Vector, b: Vector) = a + b

type RangeLimits = | LineOfEffect | LineOfSight | Both
type SelectionRange = { range: int option; limits: RangeLimits option }

let IsDistanceWithin (a: Loc) (b: Loc) (distance: int) =
   let offset = a - b
   offset.LengthSquard <= distance * distance

let IsInRectangle (point1: Loc) (point2: Loc) (point: Loc) =
   let minX = min point1.X point2.X
   let maxX = max point1.X point2.X
   let minY = min point1.Y point2.Y
   let maxY = max point1.Y point2.Y
   minX <= point.X && point.X <= maxX && minY <= point.Y && point.Y <= maxY

let AreAdjacent (a: Loc) (b: Loc) =
   let diff = a - b
   diff.KingLength = 1

let getLineFromToOld (a: Loc) (b: Loc) =
   let dx = abs(a.X - b.X)
   let dy = abs(a.Y - b.Y)
   let sx = sign (b.X - a.X)
   let sy = sign (b.Y - a.Y)
      
   let length = (max dx dy)-1

   let folder s i = 
      let (err, x0, y0) = s
      let e2 = err * 2
      let err2 = if e2 > -dy then err-dy else err
      let x1 = if e2 > -dy then x0+sx else x0
      if x1 = b.X && y0 = b.Y
      then (err2, x1, y0)
      else
         let err3 = if e2 < dx then err2 + dx else err2
         let y2 = if e2 < dx then y0 + sy else y0
         (err3, x1, y2)

   [0..length]
   |> List.scan folder (dx-dy, a.X, a.Y)
   |> List.map (fun (_, x, y) -> Loc(x, y))

let getLineFromTo (a: Loc) (b: Loc) =
   let dx = abs(a.X - b.X)
   let dy = abs(a.Y - b.Y)
   let sx = sign (b.X - a.X)
   let sy = sign (b.Y - a.Y)
   Seq.append (Seq.singleton a) (Seq.unfold (fun (err, x0, y0) ->
      if x0 = b.X && y0 = b.Y then None
      else 
         let e2 = err * 2
         let (err2, x1) = if e2 > -dy then (err-dy, x0+sx) else (err, x0)
         if x1 = b.X && y0 = b.Y then Some(Loc(x1, y0), (err2, x1, y0))
         else
            let (err3, y1) = if e2 < dx then (err2+dx, y0+sy) else (err2, y0)
            Some(Loc(x1, y1), (err3, x1, y1))) (dx-dy, a.X, a.Y))
      

type TerminalRect(position, size) =
   new (x, y, width, height) = TerminalRect(Loc(x, y), Vector(width, height))

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
   
   member this.Row s = TerminalRect(0, 0, s, 1)
   member this.Row (x, y, s) = TerminalRect(x, y, s, 1)
   member this.Row (pos, s) = TerminalRect(pos, Vector(s, 1))

   member this.Contains point = IsInRectangle position (position + size) point
   
let Intersection (a: TerminalRect) (b: TerminalRect) =
   let left = max a.Left b.Left
   let right = min a.Right b.Right
   let top = max a.Top b.Top
   let bottom = min a.Bottom b.Bottom
   let width = max 0 (right-left)
   let height = max 0 (bottom - top)

   TerminalRect(left, top, width, height)

let Intersects (a: TerminalRect) (b: TerminalRect) =
   a.Left <= b.Right && a.Right >= b.Left &&
   a.Top <= b.Bottom && a.Bottom >= b.Top

let CenterIn (main: TerminalRect) (toCenter: TerminalRect) =
   TerminalRect(main.Position + (main.Size - toCenter.Size) / 2, toCenter.Size)


[<Struct>]
type RogueColor(r: byte, g: byte, b: byte) = 
   member this.Red = r
   member this.Green = g
   member this.Blue = b
   member this.ToInt32() = BitConverter.ToInt32([|byte 0; this.Red; this.Green; this.Blue|], 0)
   override this.ToString() = r.ToString("x2") + g.ToString("x2") + b.ToString("x2")
   new(color: int) =
      let bytes = BitConverter.GetBytes(color)
      RogueColor(bytes.[2], bytes.[1], bytes.[0])

   static member Parse (str: string) =
      match str with
      | Int (s) -> Some(RogueColor(s))
      | Hex (s) -> Some(RogueColor(s))
      | _ -> let parts = str.Split([|','|])
             if parts.Length = 3
             then
                let p = (parts.[0], parts.[1], parts.[2])
                match p with
                | Int r, Int g, Int b -> Some(RogueColor(byte r, byte g, byte b))
                | Hex r, Hex g, Hex b -> Some(RogueColor(byte r, byte g, byte b))
                | _ -> None
             else None

//let makeColor r g b = { Red = byte r; Green = byte g; Blue = byte b }

type RogueMessage = { message: string; color: RogueColor }

let c r g b = RogueColor(byte r, byte g, byte b)

let lightRed = c 255 160 160
let red = c 220 0 0
let darkRed = c 100 0 0

let lightOrange = c 255 200 170
let orange = c 100 0 0
let darkOrange = c 128 64 0

let lightYellow = c 255 255 150
let yellow = c 255 255 0
let darkYellow = c 128 128 0

let lightGreen = c 130 255 90
let green = c 0 200 0
let darkGreen = c 0 100 0

let lightCyan = c 200 255 255
let cyan = c 0 255 255
let darkCyan = c 0 128 128

let lightBlue = c 128 160 255
let blue = c 0 64 255
let darkBlue = c 0 37 168

let lightViolet = c 159 63 255
let purple = c 128 0 255
let darkPurple = c 64 0 128

let lightGold = c 255 230 150
let gold = c 255 192 0
let darkGold = c 128 96 0

let flesh = c 255 200 170
let pink = c 255 160 160
   
let lightGray = c 192 192 192
let gray = c 128 128 128
let darkGray = c 48 48 48

let lightBrown = c 190 150 100
let brown = c 160 110 60
let darkBrown = c 100 64 32

let black = c 0 0 0
let white = c 255 255 255

let fromName name =
   match name with
   | "Black" -> Some black
   | "White" -> Some white
   | _ -> None

let fromEscapeChar ch =
   match ch with
   | 'k' -> darkGray
   | 'K' -> black
   | 'm' -> gray
   | 'w' -> white
   | 'W' -> lightGray
   | 'r' -> red
   | 'R' -> darkRed
   | 'o' -> orange
   | 'O' -> darkOrange
   | 'l' -> gold
   | 'L' -> darkGold
   | 'y' -> yellow
   | 'Y' -> darkYellow
   | 'g' -> green
   | 'G' -> darkGreen
   | 'c' -> cyan
   | 'C' -> darkCyan
   | 'b' -> blue
   | 'B' -> darkBlue
   | 'p' -> purple
   | 'P' -> darkPurple
   | 'f' -> flesh
   | 'F' -> brown
   | _ -> white

type Character = { glyph: char; foreColor: RogueColor; backColor: RogueColor option }

type Area =
   | Circle of Loc * int
   | Rectangle of Loc * Vector
   | Cone of Loc * Vector * int * int
   | Combined of Area * Area