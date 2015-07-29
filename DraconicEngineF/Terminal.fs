#if INTERACTIVE
module Terminal
#else
module DraconicEngineF.Terminal
#endif
open System
open System.Collections.Generic

type DrawBoxOptions = { doubleLines: bool; continueLines: bool }
let doubleLines = { doubleLines = true; continueLines = false }

type Terminal =
| WindowTerminal of Terminal * TileColor * TerminalRect
| MainTerminal of Character array * Vector
      
let makeTerminal width height =
   let blank = character ' '
   let term = MainTerminal ((Array.init (width * height) (fun i -> blank)), Vector(width, height))
   term

let flipNegativePosition t (pos: Loc) =
   let (width, height) = 
      match t with
      | WindowTerminal (_, _, bounds) -> (bounds.Size.X, bounds.Size.Y)
      | MainTerminal (_, size) -> (size.X, size.Y)
   let x = if pos.X < 0 then width + pos.X else pos.X
   let y = if pos.Y < 0 then height + pos.Y else pos.Y
   Loc (x, y)
let getSize = function
| WindowTerminal (_,_,bounds) -> bounds.Size
| MainTerminal (_, size) -> size

let rec getValue t (pos: Loc) =
   let adjustPos = flipNegativePosition t pos
   match t with
   | WindowTerminal (parent, _, bounds) -> getValue parent (adjustForBounds bounds adjustPos)
   | MainTerminal (characters, size) -> characters.[adjustPos.Y * size.X + adjustPos.X]

let rec setValue (pos: Loc) t (value: Character) =
   let adjustPos = flipNegativePosition t pos
   match t with
   | WindowTerminal (parent, _, bounds) -> setValue (adjustForBounds bounds adjustPos) parent value
   | MainTerminal (characters, size) ->
      let index = adjustPos.Y * size.X + adjustPos.X
      if characters.[index] = value then false
      else
         characters.[index] <-
            if value.color.backColor.IsNone 
            then 
               let backColor = characters.[index].color.backColor
               { value with color = { value.color with backColor = backColor } }
            else
               value
         true

let writeCharacter = setValue originPos
let writeCharacterString t (cs: CharacterString) = 
   for i = 0 to cs.Characters.Length do
      let pos = Loc(i, 0)
      setValue pos t cs.Characters.[i] |> ignore
let writeGlyph t glyph =
   match t with
   | WindowTerminal (parent, color, bounds) -> writeCharacter t { glyph = glyph; color = color }
   | MainTerminal (characters, size) -> writeCharacter t { glyph = glyph; color = defaultTileColor }

let fill t glyph =
   match t with
   | WindowTerminal (_, color, bounds) ->
      let c = { glyph = glyph; color = color }
      for pos in (bounds |> getPositionsInRect) do
         setValue pos t c |> ignore
   | MainTerminal (characters, size) ->
      let c = { glyph = glyph; color = defaultTileColor }
      Array.fill characters 0 characters.Length c

let clear t = fill t Glyph.Space
let checkBounds t x y =
   let size = 
      match t with
      | WindowTerminal (_, _, bounds) -> bounds.Size
      | MainTerminal (_, size) -> size
   x >= 0 && x < size.X && y >= 0 && y < size.Y

let getTerminalSize t =
   match t with
   | WindowTerminal (_, _, bounds) -> (bounds.Size.X, bounds.Size.Y)
   | MainTerminal (_, size) -> (size.X, size.Y)

type Terminal with
   member this.Color =
      match this with
      | WindowTerminal (_, color, _) -> color
      | MainTerminal _ -> defaultTileColor
   member this.Size =
      match this with
      | WindowTerminal (_, _, bounds) -> bounds.Size
      | MainTerminal (_, size) -> size
   member this.LowerRight =
      match this with
      | WindowTerminal (_, _, bounds) -> bounds.BottomRight
      | MainTerminal (_, size) -> originPos + size
   member private this.createWindowCore (color, newBounds) =
      match this with
      | WindowTerminal (parent, _, oldBounds) -> parent.createWindowCore(color, (makeSubBounds oldBounds newBounds))
      | MainTerminal (_, size) -> WindowTerminal (this, color, newBounds)

   member this.Item(foreColor: RogueColor) = 
      let color = { this.Color with foreColor = foreColor }
      this.createWindowCore(color, TerminalRect(Loc(), this.Size))

   member this.Item(foreColor: RogueColor, backColor: RogueColor option) = 
      let color = { foreColor = foreColor; backColor = backColor }
      this.createWindowCore(color, TerminalRect(Loc(), this.Size))
      
   member this.Item(color) = 
      this.createWindowCore(color, TerminalRect(Loc(), this.Size))
   member this.Item(x, y, width, height) =
      let pos = Loc (x, y)
      let size = Vector(width, height)
      this.createWindowCore(this.Color, TerminalRect(flipNegativePosition this pos, size))
   member this.Item(pos, size) =
      this.createWindowCore(this.Color, TerminalRect(flipNegativePosition this pos, size))
   member this.Item(pos) =
      this.createWindowCore(this.Color, TerminalRect(flipNegativePosition this pos, this.LowerRight - pos))
   member this.Item(x, y) =
      let pos = Loc(x, y)
      this.createWindowCore(this.Color, TerminalRect(flipNegativePosition this pos, this.LowerRight - pos))

let t = MainTerminal (Array.init 16 (fun i -> character 'A'), Vector(4, 4))

let writeLineChar (t: Terminal) (pos: Loc) glyph =
   writeGlyph t.[pos] glyph

let drawHorizontalLine t pos length (options: DrawBoxOptions) =
   let (left, middle, right) =
      match options with
      | {doubleLines = false; continueLines = false } -> (Glyph.BarRight, Glyph.BarLeftRight, Glyph.BarLeft)
      | {doubleLines = true; continueLines = true } -> (Glyph.BarDoubleLeftRight, Glyph.BarDoubleLeftRight, Glyph.BarDoubleLeftRight)
      | {doubleLines = true; continueLines = false } -> (Glyph.BarDoubleRight, Glyph.BarDoubleLeftRight, Glyph.BarDoubleLeft)
      | {doubleLines = false; continueLines = true } -> (Glyph.BarLeftRight, Glyph.BarLeftRight, Glyph.BarLeftRight)
   writeLineChar t pos left |> ignore
   writeLineChar t (pos |> offsetX (length - 1)) right |> ignore
   for r in TerminalRect.Row(pos.X+1, pos.Y, length-2) |> getPositionsInRect do
      writeLineChar t r middle |> ignore

let drawVerticalLine t pos length options =
   let (top, middle, bottom) =
      match options with
      | {doubleLines = false; continueLines = false } -> (Glyph.BarDown, Glyph.BarUpDown, Glyph.BarUp)
      | {doubleLines = true; continueLines = true } -> (Glyph.BarDoubleUpDown, Glyph.BarDoubleUpDown, Glyph.BarDoubleUpDown)
      | {doubleLines = true; continueLines = false } -> (Glyph.BarDoubleDown, Glyph.BarDoubleUpDown, Glyph.BarDoubleUp)
      | {doubleLines = false; continueLines = true } -> (Glyph.BarUpDown, Glyph.BarUpDown, Glyph.BarUpDown)
   writeLineChar t pos top |> ignore
   writeLineChar t (pos |> offsetY (length - 1)) bottom |> ignore
   for r in TerminalRect.Row(pos.X+1, pos.Y, length-2) |> getPositionsInRect do
      writeLineChar t r middle |> ignore

let drawBox t options =
   let size = getTerminalSize t
   match size with
   | (1, sy) -> drawVerticalLine t originPos sy options
   | (sx, 1) -> drawHorizontalLine t originPos sx options
   | sx, sy ->
      let writeChar = writeLineChar t
      let { doubleLines=drawDoubles } = options
      let (horizontal, verical) =
         if drawDoubles 
         then (Glyph.BarDoubleLeftRight, Glyph.BarDoubleUpDown)
         else (Glyph.BarLeftRight, Glyph.BarUpDown)
      writeChar originPos (if drawDoubles then Glyph.BarDoubleDownRight else Glyph.BarDownRight) |> ignore
      writeChar (originPos |> offsetX (sx-1)) (if drawDoubles then Glyph.BarDoubleDownLeft else Glyph.BarDownLeft) |> ignore
      writeChar (originPos |> offsetY (sy-1)) (if drawDoubles then Glyph.BarDoubleUpRight else Glyph.BarUpRight) |> ignore
      writeChar (originPos |> offset size) (if drawDoubles then Glyph.BarDoubleUpLeft else Glyph.BarUpLeft) |> ignore

      for l in TerminalRect.Row(1, 0, sx) |> getPositionsInRect do
         writeLineChar t l horizontal |> ignore
      for l in TerminalRect.Column(1, 0, sy) |> getPositionsInRect do
         writeLineChar t l verical |> ignore
      ()