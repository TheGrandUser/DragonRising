using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DraconicEngine.WPF
{
   public class GlyphSheet
   {
      double fontSize;
      double glyphWidth;
      double glyphHeight;

      private Geometry[] geometryCache;
      private Typeface typeface;
      private GlyphTypeface glyphTypeface;
      private Point offsetPoint;

      public GlyphSheet(Typeface typeface, double fontSize)
      {
         this.fontSize = fontSize;
         this.typeface = typeface;
         
         if (!this.typeface.TryGetGlyphTypeface(out glyphTypeface))
         {
            throw new ArgumentException("typeface does not have a GlyphTypeface");
         }

         var testIndex = glyphTypeface.CharacterToGlyphMap['╬'];
         var testCrossGlyph = glyphTypeface.GetGlyphOutline(testIndex, this.fontSize, this.fontSize);

         //var testCrossGlyph = new FormattedText(
         //      textToFormat: "╬",
         //      culture: CultureInfo.InvariantCulture,
         //      flowDirection: FlowDirection.LeftToRight,
         //      typeface: this.typeface,
         //      emSize: this.fontSize,
         //      foreground: Brushes.Black).BuildGeometry(new Point());

         this.offsetPoint = new Point(-testCrossGlyph.Bounds.TopLeft.X, -testCrossGlyph.Bounds.TopLeft.Y);
         this.glyphWidth = testCrossGlyph.Bounds.Width;
         this.glyphHeight = testCrossGlyph.Bounds.Height;

         var scale = (1.5 * glyphWidth) / glyphHeight;
         glyphHeight = glyphHeight * scale;

         var allGlyphs = (Glyph[])Enum.GetValues(typeof(Glyph));
         geometryCache = new Geometry[allGlyphs.Length];
            
         for (int i = 1; i < 136; i++)
         {
            var glyph = allGlyphs[i];
            var c = glyph.ToChar();
            var index = glyphTypeface.CharacterToGlyphMap[c];
            var geometry = glyphTypeface.GetGlyphOutline(index, this.fontSize, this.fontSize);

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new TranslateTransform(offsetPoint.X, offsetPoint.Y));
            transformGroup.Children.Add(new ScaleTransform(1, scale));

            geometry.Transform = transformGroup;

            //FormattedText text = new FormattedText(
            //   textToFormat: c.ToString(),
            //   culture: CultureInfo.InvariantCulture,
            //   flowDirection: FlowDirection.LeftToRight,
            //   typeface: this.typeface,
            //   emSize: this.fontSize,
            //   foreground: Brushes.Black);

            //var geometry = text.BuildGeometry(offsetPoint);
            if (geometry.CanFreeze)
            {
               geometry.Freeze();
            }
            geometryCache[i] = geometry;
         }
      }

      public Geometry GetGeometry(Character character)
      {
         if (character.IsWhitespace)
         {
            return null;
         }

         Geometry glyphGeometry = this.geometryCache[(int)character.Glyph];
         if (glyphGeometry != null)
         {
            return glyphGeometry;
         }

         char c = character.Glyph.ToChar();

         if (c != ' ')
         {
            var index = glyphTypeface.CharacterToGlyphMap[c];
            glyphGeometry = glyphTypeface.GetGlyphOutline(index, this.fontSize, this.fontSize);
            glyphGeometry.Transform = new TranslateTransform(offsetPoint.X, offsetPoint.Y);

            //var adjustedSize = this.fontSize / typeface.FontFamily.LineSpacing;

            //FormattedText text = new FormattedText(
            //   textToFormat: c.ToString(),
            //   culture: CultureInfo.InvariantCulture,
            //   flowDirection: FlowDirection.LeftToRight,
            //   typeface: this.typeface,
            //   emSize: adjustedSize,
            //   foreground: Brushes.Black);
            //glyphGeometry = text.BuildGeometry(new Point());
         }
         else
         {
         }

         if (glyphGeometry.CanFreeze)
         {
            glyphGeometry.Freeze();
         }

         this.geometryCache[(int)character.Glyph] = glyphGeometry;

         return glyphGeometry;
      }

      public double Width { get { return Math.Floor(glyphWidth); } }
      public double Height { get { return Math.Floor(glyphHeight); } }
   }
}