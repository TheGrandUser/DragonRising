[<AutoOpen>]
#if INTERACTIVE
module DisplayCore
#else
module DraconicEngineF.DisplayCore
#endif

open System
open TryParser

[<Struct>]
type RogueColor(r : byte, g : byte, b : byte) = 
   member this.Red = r
   member this.Green = g
   member this.Blue = b
   
   member this.ToInt32() = 
      BitConverter.ToInt32([| byte 0
                              this.Red
                              this.Green
                              this.Blue |], 0)
   
   override this.ToString() = r.ToString("x2") + g.ToString("x2") + b.ToString("x2")
   
   new(color : int) = 
      let bytes = BitConverter.GetBytes(color)
      RogueColor(bytes.[2], bytes.[1], bytes.[0])
   
   static member Parse(str : string) = 
      match str with
      | Int(s) -> Some(RogueColor(s))
      | Hex(s) -> Some(RogueColor(s))
      | _ -> 
         let parts = str.Split([| ',' |])
         if parts.Length = 3 then 
            let p = (parts.[0], parts.[1], parts.[2])
            match p with
            | Int r, Int g, Int b -> Some(RogueColor(byte r, byte g, byte b))
            | Hex r, Hex g, Hex b -> Some(RogueColor(byte r, byte g, byte b))
            | _ -> None
         else None

type TileColor = { foreColor: RogueColor; backColor: RogueColor option }

//let makeColor r g b = { Red = byte r; Green = byte g; Blue = byte b }
type RogueMessage = 
   { message : string
     color : RogueColor }
module RogueColors =
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
   | "Black" -> Some RogueColors.black
   | "White" -> Some RogueColors.white
   | _ -> None

let fromEscapeChar ch = 
   match ch with
   | 'k' -> RogueColors.darkGray
   | 'K' -> RogueColors.black
   | 'm' -> RogueColors.gray
   | 'w' -> RogueColors.white
   | 'W' -> RogueColors.lightGray
   | 'r' -> RogueColors.red
   | 'R' -> RogueColors.darkRed
   | 'o' -> RogueColors.orange
   | 'O' -> RogueColors.darkOrange
   | 'l' -> RogueColors.gold
   | 'L' -> RogueColors.darkGold
   | 'y' -> RogueColors.yellow
   | 'Y' -> RogueColors.darkYellow
   | 'g' -> RogueColors.green
   | 'G' -> RogueColors.darkGreen
   | 'c' -> RogueColors.cyan
   | 'C' -> RogueColors.darkCyan
   | 'b' -> RogueColors.blue
   | 'B' -> RogueColors.darkBlue
   | 'p' -> RogueColors.purple
   | 'P' -> RogueColors.darkPurple
   | 'f' -> RogueColors.flesh
   | 'F' -> RogueColors.brown
   | _ -> RogueColors.white

type Glyph = 
   | Space = 0
   | ExclamationMark = 1
   | Quote = 2
   | Pound = 3
   | DollarSign = 4
   | Percent = 5
   | Ampersand = 6
   | Apostrophe = 7
   | OpenParenthesis = 8
   | CloseParenthesis = 9
   | Asterisk = 10
   | Plus = 11
   | Comma = 12
   | Dash = 13
   | Period = 14
   | Slash = 15
   | Digit0 = 16
   | Digit1 = 17
   | Digit2 = 18
   | Digit3 = 19
   | Digit4 = 20
   | Digit5 = 21
   | Digit6 = 22
   | Digit7 = 23
   | Digit8 = 24
   | Digit9 = 25
   | Colon = 26
   | Semicolon = 27
   | LessThan = 28
   | Equals = 29
   | GreaterThan = 30
   | QuestionMark = 31
   | At = 32
   | AUpper = 33
   | BUpper = 34
   | CUpper = 35
   | DUpper = 36
   | EUpper = 37
   | FUpper = 38
   | GUpper = 39
   | HUpper = 40
   | IUpper = 41
   | JUpper = 42
   | KUpper = 43
   | LUpper = 44
   | MUpper = 45
   | NUpper = 46
   | OUpper = 47
   | PUpper = 48
   | QUpper = 49
   | RUpper = 50
   | SUpper = 51
   | TUpper = 52
   | UUpper = 53
   | VUpper = 54
   | WUpper = 55
   | XUpper = 56
   | YUpper = 57
   | ZUpper = 58
   | OpenBracket = 59
   | Backslash = 60
   | CloseBracket = 61
   | Caret = 62
   | Underscore = 63
   | Accent = 64
   | ALower = 65
   | BLower = 66
   | CLower = 67
   | DLower = 68
   | ELower = 69
   | FLower = 70
   | GLower = 71
   | HLower = 72
   | ILower = 73
   | JLower = 74
   | KLower = 75
   | LLower = 76
   | MLower = 77
   | NLower = 78
   | OLower = 79
   | PLower = 80
   | QLower = 81
   | RLower = 82
   | SLower = 83
   | TLower = 84
   | ULower = 85
   | VLower = 86
   | WLower = 87
   | XLower = 88
   | YLower = 89
   | ZLower = 90
   | OpenBrace = 91
   | Pipe = 92
   | CloseBrace = 93
   | Tilde = 94
   | Bullet = 95
   | BarUpDown = 96
   | BarUpDownLeft = 97
   | BarUpDownDoubleLeft = 98
   | BarDoubleUpDownSingleLeft = 99
   | BarDoubleDownSingleLeft = 100
   | BarDownDoubleLeft = 101
   | BarDoubleUpDownLeft = 102
   | BarDoubleUpDown = 103
   | BarDoubleDownLeft = 104
   | BarDoubleUpLeft = 105
   | BarDoubleUpSingleLeft = 106
   | BarUpDoubleLeft = 107
   | BarDownLeft = 108
   | BarUpRight = 109
   | BarUpLeftRight = 110
   | BarDownLeftRight = 111
   | BarUpDownRight = 112
   | BarLeftRight = 113
   | BarUpDownLeftRight = 114
   | BarUpDownDoubleRight = 115
   | BarDoubleUpDownSingleRight = 116
   | BarDoubleUpRight = 117
   | BarDoubleDownRight = 118
   | BarDoubleUpLeftRight = 119
   | BarDoubleDownLeftRight = 120
   | BarDoubleUpDownRight = 121
   | BarDoubleLeftRight = 122
   | BarDoubleUpDownLeftRight = 123
   | BarUpDoubleLeftRight = 124
   | displayableCharToInt = 125
   | BarDoubleUpSingleLeftRight = 126
   | BarDownDoubleLeftRight = 127
   | BarDoubleDownSingleLeftRight = 128
   | BarDoubleUpSingleRight = 129
   | BarUpDoubleRight = 130
   | BarDownDoubleRight = 131
   | BarDoubleDownSingleRight = 132
   | BarDoubleUpDownSingleLeftRight = 133
   | BarUpDownDoubleLeftRight = 134
   | BarUpLeft = 135
   | BarDownRight = 136
   | BarDown = 137
   | BarLeft = 138
   | BarRight = 139
   | BarUp = 140
   | BarDoubleDown = 141
   | BarDoubleLeft = 142
   | BarDoubleRight = 143
   | BarDoubleUp = 144
   | TriangleUp = 145
   | TriangleDown = 146
   | TriangleRight = 147
   | TriangleLeft = 148
   | ArrowUp = 149
   | ArrowDown = 150
   | ArrowRight = 151
   | ArrowLeft = 152
   | Solid = 153
   | SolidFill = 154
   | Dark = 155
   | DarkFill = 156
   | Gray = 157
   | GrayFill = 158
   | Light = 159
   | LightFill = 160
   | HorizontalBars = 161
   | HorizontalBarsFill = 162
   | VerticalBars = 163
   | VerticalBarsFill = 164
   | Face = 165
   | Mountains = 167
   | Grass = 168
   | TreeConical = 169
   | TreeRound = 170
   | Tombstone = 171
   | Hill = 172
   | TreeDots = 173
   | TwoDots = 174
   | Dashes = 175
   | Door = 176
   | Box = 177

let glyphToChar g = 
      match g with
      | Glyph.Space -> ' '
      | Glyph.ExclamationMark -> '!'
      | Glyph.Quote -> '"'
      | Glyph.Pound -> '#'
      | Glyph.DollarSign -> '$'
      | Glyph.Percent -> '%'
      | Glyph.Ampersand -> '&'
      | Glyph.Apostrophe -> '\''
      | Glyph.OpenParenthesis -> '('
      | Glyph.CloseParenthesis -> ')'
      | Glyph.Asterisk -> '*'
      | Glyph.Plus -> '+'
      | Glyph.Comma -> ','
      | Glyph.Dash -> '-'
      | Glyph.Period -> '.'
      | Glyph.Slash -> '/'
      | Glyph.Digit0 -> '0'
      | Glyph.Digit1 -> '1'
      | Glyph.Digit2 -> '2'
      | Glyph.Digit3 -> '3'
      | Glyph.Digit4 -> '4'
      | Glyph.Digit5 -> '5'
      | Glyph.Digit6 -> '6'
      | Glyph.Digit7 -> '7'
      | Glyph.Digit8 -> '8'
      | Glyph.Digit9 -> '9'
      | Glyph.Colon -> ':'
      | Glyph.Semicolon -> ';'
      | Glyph.LessThan -> '<'
      | Glyph.Equals -> '='
      | Glyph.GreaterThan -> '>'
      | Glyph.QuestionMark -> ' '
      | Glyph.At -> '@'
      | Glyph.AUpper -> 'A'
      | Glyph.BUpper -> 'B'
      | Glyph.CUpper -> 'C'
      | Glyph.DUpper -> 'D'
      | Glyph.EUpper -> 'E'
      | Glyph.FUpper -> 'F'
      | Glyph.GUpper -> 'G'
      | Glyph.HUpper -> 'H'
      | Glyph.IUpper -> 'I'
      | Glyph.JUpper -> 'J'
      | Glyph.KUpper -> 'K'
      | Glyph.LUpper -> 'L'
      | Glyph.MUpper -> 'M'
      | Glyph.NUpper -> 'N'
      | Glyph.OUpper -> 'O'
      | Glyph.PUpper -> 'P'
      | Glyph.QUpper -> 'Q'
      | Glyph.RUpper -> 'R'
      | Glyph.SUpper -> 'S'
      | Glyph.TUpper -> 'T'
      | Glyph.UUpper -> 'U'
      | Glyph.VUpper -> 'V'
      | Glyph.WUpper -> 'W'
      | Glyph.XUpper -> 'X'
      | Glyph.YUpper -> 'Y'
      | Glyph.ZUpper -> 'Z'
      | Glyph.OpenBracket -> '['
      | Glyph.Backslash -> '\\'
      | Glyph.CloseBracket -> ']'
      | Glyph.Caret -> '^'
      | Glyph.Underscore -> '_'
      | Glyph.Accent -> '`'
      | Glyph.ALower -> 'a'
      | Glyph.BLower -> 'b'
      | Glyph.CLower -> 'c'
      | Glyph.DLower -> 'd'
      | Glyph.ELower -> 'e'
      | Glyph.FLower -> 'f'
      | Glyph.GLower -> 'g'
      | Glyph.HLower -> 'h'
      | Glyph.ILower -> 'i'
      | Glyph.JLower -> 'j'
      | Glyph.KLower -> 'k'
      | Glyph.LLower -> 'l'
      | Glyph.MLower -> 'm'
      | Glyph.NLower -> 'n'
      | Glyph.OLower -> 'o'
      | Glyph.PLower -> 'p'
      | Glyph.QLower -> 'q'
      | Glyph.RLower -> 'r'
      | Glyph.SLower -> 's'
      | Glyph.TLower -> 't'
      | Glyph.ULower -> 'u'
      | Glyph.VLower -> 'v'
      | Glyph.WLower -> 'w'
      | Glyph.XLower -> 'x'
      | Glyph.YLower -> 'y'
      | Glyph.ZLower -> 'z'
      | Glyph.OpenBrace -> '{'
      | Glyph.Pipe -> '|'
      | Glyph.CloseBrace -> '}'
      | Glyph.Tilde -> '~'
      | Glyph.Bullet -> '◦'
      | Glyph.BarUpDown -> '│'
      | Glyph.BarUpDownLeft -> '┤'
      | Glyph.BarUpDownDoubleLeft -> '╡'
      | Glyph.BarDoubleUpDownSingleLeft -> '╢'
      | Glyph.BarDoubleDownSingleLeft -> '╖'
      | Glyph.BarDownDoubleLeft -> '╕'
      | Glyph.BarDoubleUpDownLeft -> '╣'
      | Glyph.BarDoubleUpDown -> '║'
      | Glyph.BarDoubleDownLeft -> '╗'
      | Glyph.BarDoubleUpLeft -> '╝'
      | Glyph.BarDoubleUpSingleLeft -> '╜'
      | Glyph.BarUpDoubleLeft -> '╛'
      | Glyph.BarDownLeft -> '┐'
      | Glyph.BarUpRight -> '└'
      | Glyph.BarUpLeftRight -> '┴'
      | Glyph.BarDownLeftRight -> '┬'
      | Glyph.BarUpDownRight -> '├'
      | Glyph.BarLeftRight -> '─'
      | Glyph.BarUpDownLeftRight -> '┼'
      | Glyph.BarUpDownDoubleRight -> '╞'
      | Glyph.BarDoubleUpDownSingleRight -> '╟'
      | Glyph.BarDoubleUpRight -> '╚'
      | Glyph.BarDoubleDownRight -> '╔'
      | Glyph.BarDoubleUpLeftRight -> '╩'
      | Glyph.BarDoubleDownLeftRight -> '╦'
      | Glyph.BarDoubleUpDownRight -> '╠'
      | Glyph.BarDoubleLeftRight -> '═'
      | Glyph.BarDoubleUpDownLeftRight -> '╬'
      | Glyph.BarUpDoubleLeftRight -> '╧'
      | Glyph.BarDoubleUpSingleLeftRight -> '╨'
      | Glyph.BarDownDoubleLeftRight -> '╤'
      | Glyph.BarDoubleDownSingleLeftRight -> '╥'
      | Glyph.BarDoubleUpSingleRight -> '╙'
      | Glyph.BarUpDoubleRight -> '╘'
      | Glyph.BarDownDoubleRight -> '╒'
      | Glyph.BarDoubleDownSingleRight -> '╓'
      | Glyph.BarDoubleUpDownSingleLeftRight -> '╫'
      | Glyph.BarUpDownDoubleLeftRight -> '╪'
      | Glyph.BarUpLeft -> '┘'
      | Glyph.BarDownRight -> '┌'
      | Glyph.BarDown -> ' '
      | Glyph.BarLeft -> ' '
      | Glyph.BarRight -> ' '
      | Glyph.BarUp -> ' '
      | Glyph.BarDoubleDown -> ' '
      | Glyph.BarDoubleLeft -> ' '
      | Glyph.BarDoubleRight -> ' '
      | Glyph.BarDoubleUp -> ' '
      | Glyph.TriangleUp -> '▴'
      | Glyph.TriangleDown -> '▾'
      | Glyph.TriangleRight -> '▸'
      | Glyph.TriangleLeft -> '◂'
      | Glyph.ArrowUp -> '↑'
      | Glyph.ArrowDown -> '↓'
      | Glyph.ArrowRight -> '→'
      | Glyph.ArrowLeft -> '←'
      | Glyph.Solid -> ' '
      | Glyph.SolidFill -> '█'
      | Glyph.Dark -> ' '
      | Glyph.DarkFill -> '▓'
      | Glyph.Gray -> ' '
      | Glyph.GrayFill -> '▒'
      | Glyph.Light -> ' '
      | Glyph.LightFill -> '░'
      | Glyph.HorizontalBars -> ' '
      | Glyph.HorizontalBarsFill -> ' '
      | Glyph.VerticalBars -> ' '
      | Glyph.VerticalBarsFill -> ' '
      | Glyph.Face -> '☺'
      | Glyph.Mountains -> ' '
      | Glyph.Grass -> ' '
      | Glyph.TreeConical -> ' '
      | Glyph.TreeRound -> ' '
      | Glyph.Tombstone -> ' '
      | Glyph.Hill -> ' '
      | Glyph.TreeDots -> ' '
      | Glyph.TwoDots -> ' '
      | Glyph.Dashes -> ' '
      | Glyph.Door -> ' '
      | Glyph.Box -> ' '
      | _ -> ' '

type Character = 
   { glyph : Glyph
     color : TileColor }


let displayableCharToInt (c: char) = (int c) - (int ' ')

let glyph (c:char) = if c >= ' ' && c <= '~' then enum<Glyph>(displayableCharToInt c) else Glyph.Space
let toGlyphs (str:string) =
   str.ToCharArray() |> Seq.map glyph
   
let defaultForeColor = RogueColors.white
let defaultTileColor = { foreColor= RogueColors.white; backColor=Some RogueColors.black}
let character char = { glyph = (glyph char); color = defaultTileColor }
let makeForeOnlyChar glyph color = { glyph = glyph; color = { foreColor = color; backColor = None } } 

type CharacterString (text: string) =
   let characters = text |> toGlyphs |> Seq.map (fun g -> { glyph = g; color = { foreColor = defaultForeColor; backColor = None } }) |> Seq.toArray
   member this.WordWrap lineWidth: CharacterString list = []
   member this.Count = characters.Length
   member this.Characters = characters