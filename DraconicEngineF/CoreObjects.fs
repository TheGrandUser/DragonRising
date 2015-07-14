module DraconicEngineF.CoreObjects

type Direction =
   | East
   | Southeast
   | South
   | Southwest
   | West
   | Northwest
   | North
   | Northeast
   
[<Struct>]
type Vector(x: int, y: int) =
   member this.X = x
   member this.Y = y

   member this.KingLength = 
      let x' = abs x
      let y' = abs y
      max x' y'
   member this.RookLength = abs x + abs y
   member this.Length = sqrt(double (this.X * this.X + this.Y * this.Y))
   static member (+) (v1: Vector, v2: Vector) = Vector(v1.X + v2.X, v1.Y + v2.Y)

[<Struct>]
type Loc(x: int, y: int) =
   member this.X = x
   member this.Y = y
   static member (+) (l: Loc, v: Vector) = Loc(l.X + v.X, l.Y + v.Y)
   static member (+) (v: Vector, l: Loc) = Loc(l.X + v.X, l.Y + v.Y)
   static member (-) (l: Loc, v: Vector) = Loc(l.X - v.X, l.Y - v.Y)
   static member (-) (l1: Loc, l2: Loc) = Vector(l1.X - l2.X, l1.Y - l2.Y)

type Vector with
   member this.Zero = Vector(0, 0)
   member this.Add (a: Vector, b: Vector) = a + b

let AreAdjacent (a: Loc) (b: Loc) =
   let diff = a - b
   diff.KingLength = 1

let RectContains (corner1: Loc) (corner2: Loc) (point: Loc) =
   let maxX = max corner1.X corner2.X
   let minX = min corner1.X corner2.X
   let maxY = max corner1.Y corner2.Y
   let minY = min corner1.Y corner2.Y
   minX <= point.X && point.X <= maxX && minY <= point.Y && point.Y <= maxY

type TerminalRect(position, size) =
   member this.Position = position
   member this.Size = size
   member this.Contains point = RectContains position (position + size) point
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

type RogueColor = { Red: byte; Green: byte; Blue: byte }

let makeColor r g b = { Red = byte r; Green = byte g; Blue = byte b }

let c = makeColor

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
   | Cirlce of Loc * int
   | Rectangle of Loc * Vector
   | Cone of Loc * Vector * int * int
   | Combined of Area * Area