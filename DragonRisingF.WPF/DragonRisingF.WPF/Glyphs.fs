module Glyphs

open DraconicEngineF
open DraconicEngineF.DisplayCore
open DraconicEngineF.Terminal
open System
open System.ComponentModel
open System.Windows
open System.Windows.Controls
open System.Windows.Media

let isDrawable c = c.glyph <> Glyph.Space


let blackBrush = new SolidColorBrush(Colors.Black)
let padding = 0.0

let glyphToGeometry fontSize (glyphTypeface: GlyphTypeface) (offsetPoint: Point) scale glyph =
   let c = glyphToChar glyph |> int
   let index = glyphTypeface.CharacterToGlyphMap.[c]
   let geometry: Geometry = glyphTypeface.GetGlyphOutline(index, fontSize, fontSize)
   let transformGroup = new TransformGroup()
   do transformGroup.Children.Add(new TranslateTransform(offsetPoint.X, offsetPoint.Y))
   do transformGroup.Children.Add(new ScaleTransform(1.0, scale))
   geometry.Transform <- transformGroup
   if geometry.CanFreeze then geometry.Freeze()
   geometry

type GlyphSheet (typeface: Typeface, fontSize: float) =

   let glyphTypeface =
      let mutable gt: GlyphTypeface = null
      let r = typeface.TryGetGlyphTypeface (&gt)
      if r <> true then raise (ArgumentException("typeface does not have a GlyphTypeface"))
      gt

   let offsetPoint, glyphWidth, glyphHeight, scale =
      let testIndex = glyphTypeface.CharacterToGlyphMap.[int '╬']
      let testCrossGlyph = glyphTypeface.GetGlyphOutline(testIndex, fontSize, fontSize)
      let op = new Point(-testCrossGlyph.Bounds.TopLeft.X, -testCrossGlyph.Bounds.TopLeft.Y)
      let gw = testCrossGlyph.Bounds.Width
      let gh = testCrossGlyph.Bounds.Height
      let s = 1.5 * gw / gh
      op, gw, gh * s, s

   let geometryCache =
      let gtg = glyphToGeometry fontSize glyphTypeface offsetPoint scale
      Enum.GetValues(typedefof<Glyph>) :?> array<Glyph>
      |> Array.map gtg

   member this.Width = floor glyphWidth
   member this.Height = floor glyphHeight
   member this.GetGeometry character: Geometry =
      if isDrawable character then
         let glyphGeo = geometryCache.[int character.glyph]
         if glyphGeo <> null then glyphGeo
         else
            let c = glyphToChar character.glyph
            let index = glyphTypeface.CharacterToGlyphMap.[int c]
            let glyphGeometry = glyphTypeface.GetGlyphOutline(index, fontSize, fontSize)
            glyphGeometry.Transform <- new TranslateTransform(offsetPoint.X, offsetPoint.Y)
            if glyphGeometry.CanFreeze then glyphGeometry.Freeze()
            geometryCache.[int character.glyph] <- glyphGeometry
            glyphGeometry
      else null

let glyphPosToRect (terminal: Terminal) (glyphSheet: GlyphSheet) x y =
   let fillLeft = 
      let fl = float x * glyphSheet.Width + padding
      if x = 0 then fl - padding else fl
   let fillTop =
      let fr = float y * glyphSheet.Height + padding
      if y = 0 then fr - padding else fr
   let width = 
      if x = 0 || x = terminal.Size.X - 1 then glyphSheet.Width + padding 
      else glyphSheet.Width
   let height =
      if y = 0 || y = terminal.Size.Y then  glyphSheet.Height + padding
      else glyphSheet.Height
   new Rect(fillLeft, fillTop, width, height)

let drawBackground (dc: DrawingContext) workingRect =
   dc.DrawRectangle (blackBrush, null, workingRect)

let drawBlock (dc: DrawingContext) (terminal: Terminal) (glyphSheet: GlyphSheet) c x y =
   let blockRect = glyphPosToRect terminal glyphSheet x y
   dc.DrawRectangle(new SolidColorBrush(toSystemColor c), null, blockRect);

let drawGlyph (dc: DrawingContext) (terminal: Terminal) (glyphSheet: GlyphSheet) c x y =
   let x' = x * glyphSheet.Width + padding
   let y' = y * glyphSheet.Height + padding
   let glyphGeo = glyphSheet.GetGeometry c
   dc.PushTransform(new TranslateTransform(x', y'))
   dc.DrawGeometry(new SolidColorBrush(toSystemColor c.color.foreColor), null, glyphGeo)
   dc.Pop()

let drawTerminal dc t glyphSheet workingRect =

   let drawBlock' = drawBlock dc t glyphSheet
   let drawGlyph' = drawGlyph dc t glyphSheet

   do drawBackground dc workingRect
   let left = max 0 ((workingRect.Left - padding) / glyphSheet.Width |> int)
   let top = max 0 ((workingRect.Top - padding) / glyphSheet.Height |> int)
   let right = min t.Size.X ((workingRect.Right - padding) / glyphSheet.Width + 1.0 |> int)
   let bottom = min t.Size.Y ((workingRect.Bottom - padding) / glyphSheet.Height + 1.0 |> int)

   let get x y = getValue t (new Loc(x, y))

   for y = top to bottom do
      for x = left to right do
         let character = get x y
         match character.color.backColor with
            | Some bc -> drawBlock' bc x y
            | None -> ()
         if isDrawable character then
            drawGlyph' character (float x) (float y)
