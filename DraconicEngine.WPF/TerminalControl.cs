using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DraconicEngine.Terminals;

namespace DraconicEngine.WPF
{
   /// <summary>
   /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
   ///
   /// Step 1a) Using this custom control in a XAML file that exists in the current project.
   /// Add this XmlNamespace attribute to the root element of the markup file where it is 
   /// to be used:
   ///
   ///     xmlns:MyNamespace="clr-namespace:DraconicEngine.WPF"
   ///
   ///
   /// Step 1b) Using this custom control in a XAML file that exists in a different project.
   /// Add this XmlNamespace attribute to the root element of the markup file where it is 
   /// to be used:
   ///
   ///     xmlns:MyNamespace="clr-namespace:DraconicEngine.WPF;assembly=DraconicEngine.WPF"
   ///
   /// You will also need to add a project reference from the project where the XAML file lives
   /// to this project and Rebuild to avoid compilation errors:
   ///
   ///     Right click on the target project in the Solution Explorer and
   ///     "Add Reference"->"Projects"->[Select this project]
   ///
   ///
   /// Step 2)
   /// Go ahead and use your control in the XAML file.
   ///
   ///     <MyNamespace:CustomControl1/>
   ///
   /// </summary>
   public class TerminalControl : Control
   {
      static TerminalControl()
      {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(TerminalControl), new FrameworkPropertyMetadata(typeof(TerminalControl)));
      }

      public TerminalControl()
      {
         this.glyphSheet = new GlyphSheet(typeface, 14.0 * 96.0 / 72.0);
      }

      private const int TilePadding = 0;

      //Typeface typeface = new Typeface("Courier New");
      Typeface typeface = new Typeface("Consolas");
      private GlyphSheet glyphSheet;
      private ITerminal terminal;

      /// <summary>
      /// Gets and sets whether or not the cursor should be hidden when it hovers of the control.
      /// </summary>
      [Description("Whether or not the cursor should be hidden when over this control.")]
      public bool HideCursor { get; set; }

      [Browsable(false)]
      [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
      public ITerminal Terminal
      {
         get { return terminal; }
         set
         {
            if (terminal != value)
            {
               if (terminal != null)
               {
                  terminal.CharacterChanged -= Terminal_CharacterChanged;
               }

               terminal = value;

               if (terminal != null)
               {
                  terminal.CharacterChanged += Terminal_CharacterChanged;
               }
               this.InvalidateMeasure();
               this.InvalidateVisual();
            }
         }
      }

      protected override Size MeasureOverride(Size constraint)
      {
         if (terminal != null)
         {
            return new Size(
                   (glyphSheet.Width * terminal.Size.X) + (TilePadding * 2),
                   (glyphSheet.Height * terminal.Size.Y) + (TilePadding * 2));
         }

         return new Size(0, 0);
      }

      Brush blackBrush = new SolidColorBrush(Colors.Black);

      Rect? invalidRect = null;

      protected override void OnRender(DrawingContext drawingContext)
      {
         Rect workingRect = invalidRect ?? new Rect(this.RenderSize);

         drawingContext.DrawRectangle(blackBrush, null, workingRect);

         if (this.terminal != null)
         {
            // only refresh characters in the clip rect
            int left = (int)Math.Max(0, (workingRect.Left - TilePadding) / glyphSheet.Width);
            int top = (int)Math.Max(0, (workingRect.Top - TilePadding) / glyphSheet.Height);
            int right = (int)Math.Min(terminal.Size.X, (workingRect.Right - TilePadding) / glyphSheet.Width + 1);
            int bottom = (int)Math.Min(terminal.Size.Y, (workingRect.Bottom - TilePadding) / glyphSheet.Height + 1);

            //GuidelineSet guidelines = CreateGuidelines();

            //drawingContext.PushGuidelineSet(guidelines);

            for (int y = top; y < bottom; y++)
            {
               for (int x = left; x < right; x++)
               {
                  Character character = terminal.Get(x, y);

                  // fill the background if needed
                  if (character.BackColor.HasValue)
                  {
                     double fillLeft = (x * glyphSheet.Width) + TilePadding;
                     double fillTop = (y * glyphSheet.Height) + TilePadding;
                     double width = glyphSheet.Width;
                     double height = glyphSheet.Height;

                     // fill past the padding on the edges
                     if (x == 0)
                     {
                        fillLeft -= TilePadding;
                        width += TilePadding;
                     }
                     if (x == terminal.Size.X - 1)
                     {
                        width += TilePadding;
                     }
                     if (y == 0)
                     {
                        fillTop -= TilePadding;
                        height += TilePadding;
                     }
                     if (y == terminal.Size.Y - 1)
                     {
                        height += TilePadding;
                     }

                     drawingContext.DrawRectangle(new SolidColorBrush(character.BackColor.GetValueOrDefault().ToSystemColor()), null,
                        new Rect(fillLeft, fillTop, width, height));
                  }

                  // don't draw if it's a blank glyph
                  if (character.IsWhitespace)
                  {
                     continue;
                  }
                  var drawPoint = new Point((x * glyphSheet.Width) + TilePadding, (y * glyphSheet.Height) + TilePadding);

                  var glyphGeometry = glyphSheet.GetGeometry(character);

                  drawingContext.PushTransform(new TranslateTransform((x * glyphSheet.Width) + TilePadding, (y * glyphSheet.Height) + TilePadding));
                  drawingContext.DrawGeometry(new SolidColorBrush(character.ForeColor.ToSystemColor()), null, glyphGeometry);
                  drawingContext.Pop();
               }
            }

            //drawingContext.Pop();
         }

         invalidRect = null;

         base.OnRender(drawingContext);
      }

      private GuidelineSet CreateGuidelines()
      {
         var set = new GuidelineSet(
            Enumerable.Range(0, this.Terminal.Size.X + 1).Select(x => Math.Floor(x * this.glyphSheet.Width) + TilePadding + 0.5).ToArray(),
            Enumerable.Range(0, this.Terminal.Size.Y + 1).Select(y => Math.Floor(y * this.glyphSheet.Height) + TilePadding + 0.5).ToArray());

         return set;
      }

      private void InvalidateCharacter(Loc pos)
      {
         double width = glyphSheet.Width;
         double height = glyphSheet.Height;
         double left = (pos.X * width) + TilePadding;
         double top = (pos.Y * height) + TilePadding;

         // fill past the padding on the edges
         if (pos.X == 0)
         {
            left -= TilePadding;
            width += TilePadding;
         }
         if (pos.X == terminal.Size.X - 1)
         {
            width += TilePadding;
         }
         if (pos.Y == 0)
         {
            top -= TilePadding;
            height += TilePadding;
         }
         if (pos.Y == terminal.Size.Y - 1)
         {
            height += TilePadding;
         }

         // invalidate the rect under the character
         //this.invalidRect = new Rect(left, top, width, height);
         //this.InvalidateVisual();
      }

      private void Terminal_CharacterChanged(object sender, CharacterEventArgs e)
      {
         InvalidateCharacter(e.Position);
      }

      public Loc ScreenToTerminal(Point screenLocation)
      {
         int terminalX;
         if (screenLocation.X <= TilePadding)
         {
            terminalX = 0;
         }
         else if (screenLocation.X > (this.Terminal.Size.X - 1) * this.glyphSheet.Width + TilePadding)
         {
            terminalX = this.Terminal.Size.X - 1;
         }
         else
         {
            terminalX = (int)Math.Floor((screenLocation.X - TilePadding) / this.glyphSheet.Width) - 1;
         }

         int terminalY;
         if (screenLocation.Y <= TilePadding)
         {
            terminalY = 0;
         }
         else if (screenLocation.Y > (this.Terminal.Size.Y - 1) * this.glyphSheet.Height + TilePadding)
         {
            terminalY = this.Terminal.Size.X - 1;
         }
         else
         {
            terminalY = (int)Math.Floor((screenLocation.Y - TilePadding) / this.glyphSheet.Height) - 1;
         }

         return new Loc(terminalX, terminalY);
      }
   }
}
