namespace ViewModels

open DraconicEngineF
open DraconicEngineF.DisplayCore
open DraconicEngineF.Terminal
open Glyphs
open System
open System.ComponentModel
open System.Windows
open System.Windows.Controls
open System.Windows.Media


type TerminalControl() =
   inherit Control()
   //static do Control.DefaultStyleKeyProperty.OverrideMetadata(typedefof<TerminalControl>, new FrameworkPropertyMetadata(typedefof<TerminalControl>));

   let typeface = new Typeface "Consolas"
   let glyphSheet = new GlyphSheet(typeface, 14.0 * 96.0 / 72.0)
   let pen: Pen = null
   let mutable terminal: Terminal option = None
   let mutable hideCursor = false

   member this.HideCursor with get() = hideCursor and set value = hideCursor <- value

   [<Browsable(false)>]
   [<DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>]
   member this.Terminal 
      with get() = terminal 
      and set (value) = 
         if value <> terminal then
            terminal <- value
            this.InvalidateMeasure()
            this.InvalidateVisual()
   member this.ScreenToTerminal (screenLocation: Point) =
      let terminalX = 
         if screenLocation.X <= padding then 0
         elif screenLocation.X > (float terminal.Value.Size.X - 1.0) * glyphSheet.Width + padding then terminal.Value.Size.X - 1
         else floor((screenLocation.X - padding) / glyphSheet.Width) - 1.0 |> int
      let terminalY = 
         if screenLocation.Y <= padding then 0
         elif screenLocation.Y > (float terminal.Value.Size.Y - 1.0) * glyphSheet.Height + padding then terminal.Value.Size.Y - 1
         else floor((screenLocation.Y - padding) / glyphSheet.Height) - 1.0 |> int
      new Loc(terminalX, terminalY)
   override this.MeasureOverride _ =
      match terminal with
      | Some t -> 
         let padding = padding * 2.0
         new Size(glyphSheet.Width * (float t.Size.X) + padding, glyphSheet.Height * (float t.Size.Y) + padding)
      | None -> Size.Empty

   override this.OnRender (drawingContext) =
      let workingRect = new Rect(this.RenderSize)
      match terminal with
      | Some t -> drawTerminal drawingContext t glyphSheet workingRect
      | None -> drawBackground drawingContext workingRect

