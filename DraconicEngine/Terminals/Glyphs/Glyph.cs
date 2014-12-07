using DraconicEngine.Media;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
    public class GlyphEntry
    {
        char character;
        Geometry glyphGeometry;

        public GlyphEntry(char character)
        {
            this.character = character;
        }

        public GlyphEntry(Geometry glyphGeometry)
        {
            this.glyphGeometry = glyphGeometry;
        }
    }

    public static class Glyphs
    {
        public static readonly GlyphEntry Space = new GlyphEntry(' ');
        public static readonly GlyphEntry ExclamationMark = new GlyphEntry('!');
        public static readonly GlyphEntry Quote = new GlyphEntry('"');
        public static readonly GlyphEntry Pound = new GlyphEntry('#');
        public static readonly GlyphEntry DollarSign = new GlyphEntry('$');
        public static readonly GlyphEntry Percent = new GlyphEntry('%');
        public static readonly GlyphEntry Ampersand = new GlyphEntry('&');
        public static readonly GlyphEntry Apostrophe = new GlyphEntry('\'');
        public static readonly GlyphEntry OpenParenthesis = new GlyphEntry('(');
        public static readonly GlyphEntry CloseParenthesis = new GlyphEntry(')');
        public static readonly GlyphEntry Asterisk = new GlyphEntry('*');
        public static readonly GlyphEntry Plus = new GlyphEntry('+');
        public static readonly GlyphEntry Comma = new GlyphEntry(',');
        public static readonly GlyphEntry Dash = new GlyphEntry('-');
        public static readonly GlyphEntry Period = new GlyphEntry('.');
        public static readonly GlyphEntry Slash = new GlyphEntry('/');
        public static readonly GlyphEntry Digit0 = new GlyphEntry('0');
        public static readonly GlyphEntry Digit1 = new GlyphEntry('1');
        public static readonly GlyphEntry Digit2 = new GlyphEntry('2');
        public static readonly GlyphEntry Digit3 = new GlyphEntry('3');
        public static readonly GlyphEntry Digit4 = new GlyphEntry('4');
        public static readonly GlyphEntry Digit5 = new GlyphEntry('5');
        public static readonly GlyphEntry Digit6 = new GlyphEntry('6');
        public static readonly GlyphEntry Digit7 = new GlyphEntry('7');
        public static readonly GlyphEntry Digit8 = new GlyphEntry('8');
        public static readonly GlyphEntry Digit9 = new GlyphEntry('9');
        public static readonly GlyphEntry Colon = new GlyphEntry(':');
        public static readonly GlyphEntry Semicolon = new GlyphEntry(';');
        public static readonly GlyphEntry LessThan = new GlyphEntry('<');
        public static readonly GlyphEntry EqualsSign = new GlyphEntry('=');
        public static readonly GlyphEntry GreaterThan = new GlyphEntry('>');
        public static readonly GlyphEntry QuestionMark = new GlyphEntry('?');
        public static readonly GlyphEntry At = new GlyphEntry('@');
        public static readonly GlyphEntry AUpper = new GlyphEntry('A');
        public static readonly GlyphEntry BUpper = new GlyphEntry('B');
        public static readonly GlyphEntry CUpper = new GlyphEntry('C');
        public static readonly GlyphEntry DUpper = new GlyphEntry('D');
        public static readonly GlyphEntry EUpper = new GlyphEntry('E');
        public static readonly GlyphEntry FUpper = new GlyphEntry('F');
        public static readonly GlyphEntry GUpper = new GlyphEntry('G');
        public static readonly GlyphEntry HUpper = new GlyphEntry('H');
        public static readonly GlyphEntry IUpper = new GlyphEntry('I');
        public static readonly GlyphEntry JUpper = new GlyphEntry('J');
        public static readonly GlyphEntry KUpper = new GlyphEntry('K');
        public static readonly GlyphEntry LUpper = new GlyphEntry('L');
        public static readonly GlyphEntry MUpper = new GlyphEntry('M');
        public static readonly GlyphEntry NUpper = new GlyphEntry('N');
        public static readonly GlyphEntry OUpper = new GlyphEntry('O');
        public static readonly GlyphEntry PUpper = new GlyphEntry('P');
        public static readonly GlyphEntry QUpper = new GlyphEntry('Q');
        public static readonly GlyphEntry RUpper = new GlyphEntry('R');
        public static readonly GlyphEntry SUpper = new GlyphEntry('S');
        public static readonly GlyphEntry TUpper = new GlyphEntry('T');
        public static readonly GlyphEntry UUpper = new GlyphEntry('U');
        public static readonly GlyphEntry VUpper = new GlyphEntry('V');
        public static readonly GlyphEntry WUpper = new GlyphEntry('W');
        public static readonly GlyphEntry XUpper = new GlyphEntry('X');
        public static readonly GlyphEntry YUpper = new GlyphEntry('Y');
        public static readonly GlyphEntry ZUpper = new GlyphEntry('Z');
        public static readonly GlyphEntry OpenBracket = new GlyphEntry('[');
        public static readonly GlyphEntry Backslash = new GlyphEntry('\\');
        public static readonly GlyphEntry CloseBracket = new GlyphEntry(']');
        public static readonly GlyphEntry Caret = new GlyphEntry('^');
        public static readonly GlyphEntry Underscore = new GlyphEntry('_');
        public static readonly GlyphEntry Accent = new GlyphEntry('`');
        public static readonly GlyphEntry ALower = new GlyphEntry('a');
        public static readonly GlyphEntry BLower = new GlyphEntry('b');
        public static readonly GlyphEntry CLower = new GlyphEntry('c');
        public static readonly GlyphEntry DLower = new GlyphEntry('d');
        public static readonly GlyphEntry ELower = new GlyphEntry('e');
        public static readonly GlyphEntry FLower = new GlyphEntry('f');
        public static readonly GlyphEntry GLower = new GlyphEntry('g');
        public static readonly GlyphEntry HLower = new GlyphEntry('h');
        public static readonly GlyphEntry ILower = new GlyphEntry('i');
        public static readonly GlyphEntry JLower = new GlyphEntry('j');
        public static readonly GlyphEntry KLower = new GlyphEntry('k');
        public static readonly GlyphEntry LLower = new GlyphEntry('l');
        public static readonly GlyphEntry MLower = new GlyphEntry('m');
        public static readonly GlyphEntry NLower = new GlyphEntry('n');
        public static readonly GlyphEntry OLower = new GlyphEntry('o');
        public static readonly GlyphEntry PLower = new GlyphEntry('p');
        public static readonly GlyphEntry QLower = new GlyphEntry('q');
        public static readonly GlyphEntry RLower = new GlyphEntry('r');
        public static readonly GlyphEntry SLower = new GlyphEntry('s');
        public static readonly GlyphEntry TLower = new GlyphEntry('t');
        public static readonly GlyphEntry ULower = new GlyphEntry('u');
        public static readonly GlyphEntry VLower = new GlyphEntry('v');
        public static readonly GlyphEntry WLower = new GlyphEntry('w');
        public static readonly GlyphEntry XLower = new GlyphEntry('x');
        public static readonly GlyphEntry YLower = new GlyphEntry('y');
        public static readonly GlyphEntry ZLower = new GlyphEntry('z');
        public static readonly GlyphEntry OpenBrace = new GlyphEntry('{');
        public static readonly GlyphEntry Pipe = new GlyphEntry('|');
        public static readonly GlyphEntry CloseBrace = new GlyphEntry('}');
        public static readonly GlyphEntry Tilde = new GlyphEntry('~');
        public static readonly GlyphEntry Bullet = new GlyphEntry('◦');
        public static readonly GlyphEntry BarUpDown = new GlyphEntry('│');
        public static readonly GlyphEntry BarUpDownLeft = new GlyphEntry('┤');
        public static readonly GlyphEntry BarUpDownDoubleLeft = new GlyphEntry('╡');
        public static readonly GlyphEntry BarDoubleUpDownSingleLeft = new GlyphEntry('╢');
        public static readonly GlyphEntry BarDoubleDownSingleLeft = new GlyphEntry('╖');
        public static readonly GlyphEntry BarDownDoubleLeft = new GlyphEntry('╕');
        public static readonly GlyphEntry BarDoubleUpDownLeft = new GlyphEntry('╣');
        public static readonly GlyphEntry BarDoubleUpDown = new GlyphEntry('║');
        public static readonly GlyphEntry BarDoubleDownLeft = new GlyphEntry('╗');
        public static readonly GlyphEntry BarDoubleUpLeft = new GlyphEntry('╝');
        public static readonly GlyphEntry BarDoubleUpSingleLeft = new GlyphEntry('╜');
        public static readonly GlyphEntry BarUpDoubleLeft = new GlyphEntry('╛');
        public static readonly GlyphEntry BarDownLeft = new GlyphEntry('┐');
        public static readonly GlyphEntry BarUpRight = new GlyphEntry('└');
        public static readonly GlyphEntry BarUpLeftRight = new GlyphEntry('┴');
        public static readonly GlyphEntry BarDownLeftRight = new GlyphEntry('┬');
        public static readonly GlyphEntry BarUpDownRight = new GlyphEntry('├');
        public static readonly GlyphEntry BarLeftRight = new GlyphEntry('─');
        public static readonly GlyphEntry BarUpDownLeftRight = new GlyphEntry('┼');
        public static readonly GlyphEntry BarUpDownDoubleRight = new GlyphEntry(' ');
        public static readonly GlyphEntry BarDoubleUpDownSingleRight = new GlyphEntry('╞');
        public static readonly GlyphEntry BarDoubleUpRight = new GlyphEntry('╚');
        public static readonly GlyphEntry BarDoubleDownRight = new GlyphEntry('╔');
        public static readonly GlyphEntry BarDoubleUpLeftRight = new GlyphEntry('╩');
        public static readonly GlyphEntry BarDoubleDownLeftRight = new GlyphEntry('╦');
        public static readonly GlyphEntry BarDoubleUpDownRight = new GlyphEntry('╠');
        public static readonly GlyphEntry BarDoubleLeftRight = new GlyphEntry('═');
        public static readonly GlyphEntry BarDoubleUpDownLeftRight = new GlyphEntry('╬');
        public static readonly GlyphEntry BarUpDoubleLeftRight = new GlyphEntry('╧');
        public static readonly GlyphEntry BarDoubleUpSingleLeftRight = new GlyphEntry('╨');
        public static readonly GlyphEntry BarDownDoubleLeftRight = new GlyphEntry('╤');
        public static readonly GlyphEntry BarDoubleDownSingleLeftRight = new GlyphEntry('╥');
        public static readonly GlyphEntry BarDoubleUpSingleRight = new GlyphEntry('╙');
        public static readonly GlyphEntry BarUpDoubleRight = new GlyphEntry('╘');
        public static readonly GlyphEntry BarDownDoubleRight = new GlyphEntry('╒');
        public static readonly GlyphEntry BarDoubleDownSingleRight = new GlyphEntry('╓');
        public static readonly GlyphEntry BarDoubleUpDownSingleLeftRight = new GlyphEntry('╫');
        public static readonly GlyphEntry BarUpDownDoubleLeftRight = new GlyphEntry('╪');
        public static readonly GlyphEntry BarUpLeft = new GlyphEntry('┘');
        public static readonly GlyphEntry BarDownRight = new GlyphEntry('┌');
        public static readonly GlyphEntry BarDown = new GlyphEntry(new RectangleGeometry(new Rect(new Point(0.233189657330513, 0.476005762815475), new Point(0.296551734209061, 0.998419523239136))));
        public static readonly GlyphEntry BarLeft = new GlyphEntry(new RectangleGeometry(new Rect(new Point(0, 0.476005762815475), new Point(0.296551734209061, 0.539367854595184))));
        public static readonly GlyphEntry BarRight = new GlyphEntry(new RectangleGeometry(new Rect(new Point(0.233189657330513, 0.476005762815475), new Point(0.529741406440735, 0.539367854595184))));
        public static readonly GlyphEntry BarUp = new GlyphEntry(new RectangleGeometry(new Rect(new Point(0.296551734209061, 0.539367854595184), new Point(0.233189657330513, 0))));
        public static readonly GlyphEntry BarDoubleDown = new GlyphEntry(new PathGeometry(new PathFigure(
           new Point(0.16422413289547, 0.407040238380432), true, new PolyLineSegment(true,
              new Point(0.365517228841782, 0.407040238380432),
              new Point(0.365517228841782, 0.998419523239136),
              new Point(0.302155166864395, 0.998419523239136),
              new Point(0.302155166864395, 0.470402330160141),
              new Point(0.227586209774017, 0.470402330160141),
              new Point(0.227586209774017, 0.998419523239136),
              new Point(0.16422413289547, 0.998419523239136),
              new Point(0.16422413289547, 0.407040238380432)))));
        public static readonly GlyphEntry BarDoubleLeft = new GlyphEntry(' ');
        public static readonly GlyphEntry BarDoubleRight = new GlyphEntry(' ');
        public static readonly GlyphEntry BarDoubleUp = new GlyphEntry(new PathGeometry(new PathFigure(
           new Point(0.16422413289547, 0.470402330160141), true, new PolyLineSegment(true,
              new Point(0.365517228841782, 0.470402330160141),
              new Point(0.365517228841782, 0),
              new Point(0.302155166864395, 0),
              new Point(0.302155166864395, 0.407040238380432),
              new Point(0.227586209774017, 0.407040238380432),
              new Point(0.227586209774017, 0),
              new Point(0.16422413289547, 0),
              new Point(0.16422413289547, 0.470402330160141)))));
        public static readonly GlyphEntry TriangleUp = new GlyphEntry('▴');
        public static readonly GlyphEntry TriangleDown = new GlyphEntry('▾');
        public static readonly GlyphEntry TriangleRight = new GlyphEntry('▸');
        public static readonly GlyphEntry TriangleLeft = new GlyphEntry('◂');
        public static readonly GlyphEntry ArrowUp = new GlyphEntry('↑');
        public static readonly GlyphEntry ArrowDown = new GlyphEntry('↓');
        public static readonly GlyphEntry ArrowRight = new GlyphEntry('→');
        public static readonly GlyphEntry ArrowLeft = new GlyphEntry('←');
        public static readonly GlyphEntry Solid = new GlyphEntry(' ');
        public static readonly GlyphEntry SolidFill = new GlyphEntry('█');
        public static readonly GlyphEntry Dark = new GlyphEntry(' ');
        public static readonly GlyphEntry DarkFill = new GlyphEntry('▓');
        public static readonly GlyphEntry Gray = new GlyphEntry(' ');
        public static readonly GlyphEntry GrayFill = new GlyphEntry('▒');
        public static readonly GlyphEntry Light = new GlyphEntry(' ');
        public static readonly GlyphEntry LightFill = new GlyphEntry('░');
        public static readonly GlyphEntry HorizontalBars = new GlyphEntry(' ');
        public static readonly GlyphEntry HorizontalBarsFill = new GlyphEntry(' ');
        public static readonly GlyphEntry VerticalBars = new GlyphEntry(' ');
        public static readonly GlyphEntry VerticalBarsFill = new GlyphEntry(' ');
        public static readonly GlyphEntry Face = new GlyphEntry('☺');
        public static readonly GlyphEntry Mountains = new GlyphEntry(' ');
        public static readonly GlyphEntry Grass = new GlyphEntry(' ');
        public static readonly GlyphEntry TreeConical = new GlyphEntry(' ');
        public static readonly GlyphEntry TreeRound = new GlyphEntry(' ');
        public static readonly GlyphEntry Tombstone = new GlyphEntry(' ');
        public static readonly GlyphEntry Hill = new GlyphEntry(' ');
        public static readonly GlyphEntry TreeDots = new GlyphEntry(' ');
        public static readonly GlyphEntry TwoDots = new GlyphEntry(' ');
        public static readonly GlyphEntry Dashes = new GlyphEntry(' ');
        public static readonly GlyphEntry Door = new GlyphEntry(' ');
        public static readonly GlyphEntry Box = new GlyphEntry(' ');

        public static ImmutableList<GlyphEntry> AllGlyphs { get { return allGlyphs; } }
        static readonly ImmutableList<GlyphEntry> allGlyphs = ImmutableList.Create(new[]
      {
         Space,
         ExclamationMark,
         Quote,
         Pound,
         DollarSign,
         Percent,
         Ampersand,
         Apostrophe,
         OpenParenthesis,
         CloseParenthesis,
         Asterisk,
         Plus,
         Comma,
         Dash,
         Period,
         Slash,
         Digit0,
         Digit1,
         Digit2,
         Digit3,
         Digit4,
         Digit5,
         Digit6,
         Digit7,
         Digit8,
         Digit9,
         Colon,
         Semicolon,
         LessThan,
         EqualsSign,
         GreaterThan,
         QuestionMark,
         At,
         AUpper,
         BUpper,
         CUpper,
         DUpper,
         EUpper,
         FUpper,
         GUpper,
         HUpper,
         IUpper,
         JUpper,
         KUpper,
         LUpper,
         MUpper,
         NUpper,
         OUpper,
         PUpper,
         QUpper,
         RUpper,
         SUpper,
         TUpper,
         UUpper,
         VUpper,
         WUpper,
         XUpper,
         YUpper,
         ZUpper,
         OpenBracket,
         Backslash,
         CloseBracket,
         Caret,
         Underscore,
         Accent,
         ALower,
         BLower,
         CLower,
         DLower,
         ELower,
         FLower,
         GLower,
         HLower,
         ILower,
         JLower,
         KLower,
         LLower,
         MLower,
         NLower,
         OLower,
         PLower,
         QLower,
         RLower,
         SLower,
         TLower,
         ULower,
         VLower,
         WLower,
         XLower,
         YLower,
         ZLower,
         OpenBrace,
         Pipe,
         CloseBrace,
         Tilde,
         Bullet,
         BarUpDown,
         BarUpDownLeft,
         BarUpDownDoubleLeft,
         BarDoubleUpDownSingleLeft,
         BarDoubleDownSingleLeft,
         BarDownDoubleLeft,
         BarDoubleUpDownLeft,
         BarDoubleUpDown,
         BarDoubleDownLeft,
         BarDoubleUpLeft,
         BarDoubleUpSingleLeft,
         BarUpDoubleLeft,
         BarDownLeft,
         BarUpRight,
         BarUpLeftRight,
         BarDownLeftRight,
         BarUpDownRight,
         BarLeftRight,
         BarUpDownLeftRight,
         BarUpDownDoubleRight,
         BarDoubleUpDownSingleRight,
         BarDoubleUpRight,
         BarDoubleDownRight,
         BarDoubleUpLeftRight,
         BarDoubleDownLeftRight,
         BarDoubleUpDownRight,
         BarDoubleLeftRight,
         BarDoubleUpDownLeftRight,
         BarUpDoubleLeftRight,
         BarDoubleUpSingleLeftRight,
         BarDownDoubleLeftRight,
         BarDoubleDownSingleLeftRight,
         BarDoubleUpSingleRight,
         BarUpDoubleRight,
         BarDownDoubleRight,
         BarDoubleDownSingleRight,
         BarDoubleUpDownSingleLeftRight,
         BarUpDownDoubleLeftRight,
         BarUpLeft,
         BarDownRight,
         BarDown,
         BarLeft,
         BarRight,
         BarUp,
         BarDoubleDown,
         BarDoubleLeft,
         BarDoubleRight,
         BarDoubleUp,
         TriangleUp,
         TriangleDown,
         TriangleRight,
         TriangleLeft,
         ArrowUp,
         ArrowDown,
         ArrowRight,
         ArrowLeft,
         Solid,
         SolidFill,
         Dark,
         DarkFill,
         Gray,
         GrayFill,
         Light,
         LightFill,
         HorizontalBars,
         HorizontalBarsFill,
         VerticalBars,
         VerticalBarsFill,
         Face,
         Mountains,
         Grass,
         TreeConical,
         TreeRound,
         Tombstone,
         Hill,
         TreeDots,
         TwoDots,
         Dashes,
         Door,
         Box,
      });
    }

    public enum Glyph
    {
        Space,
        ExclamationMark,
        Quote,
        Pound,
        DollarSign,
        Percent,
        Ampersand,
        Apostrophe,
        OpenParenthesis,
        CloseParenthesis,
        Asterisk,
        Plus,
        Comma,
        Dash,
        Period,
        Slash,
        Digit0,
        Digit1,
        Digit2,
        Digit3,
        Digit4,
        Digit5,
        Digit6,
        Digit7,
        Digit8,
        Digit9,
        Colon,
        Semicolon,
        LessThan,
        Equals,
        GreaterThan,
        QuestionMark,
        At,
        AUpper,
        BUpper,
        CUpper,
        DUpper,
        EUpper,
        FUpper,
        GUpper,
        HUpper,
        IUpper,
        JUpper,
        KUpper,
        LUpper,
        MUpper,
        NUpper,
        OUpper,
        PUpper,
        QUpper,
        RUpper,
        SUpper,
        TUpper,
        UUpper,
        VUpper,
        WUpper,
        XUpper,
        YUpper,
        ZUpper,
        OpenBracket,
        Backslash,
        CloseBracket,
        Caret,
        Underscore,
        Accent,
        ALower,
        BLower,
        CLower,
        DLower,
        ELower,
        FLower,
        GLower,
        HLower,
        ILower,
        JLower,
        KLower,
        LLower,
        MLower,
        NLower,
        OLower,
        PLower,
        QLower,
        RLower,
        SLower,
        TLower,
        ULower,
        VLower,
        WLower,
        XLower,
        YLower,
        ZLower,
        OpenBrace,
        Pipe,
        CloseBrace,
        Tilde,
        Bullet,
        BarUpDown,
        BarUpDownLeft,
        BarUpDownDoubleLeft,
        BarDoubleUpDownSingleLeft,
        BarDoubleDownSingleLeft,
        BarDownDoubleLeft,
        BarDoubleUpDownLeft,
        BarDoubleUpDown,
        BarDoubleDownLeft,
        BarDoubleUpLeft,
        BarDoubleUpSingleLeft,
        BarUpDoubleLeft,
        BarDownLeft,
        BarUpRight,
        BarUpLeftRight,
        BarDownLeftRight,
        BarUpDownRight,
        BarLeftRight,
        BarUpDownLeftRight,
        BarUpDownDoubleRight,
        BarDoubleUpDownSingleRight,
        BarDoubleUpRight,
        BarDoubleDownRight,
        BarDoubleUpLeftRight,
        BarDoubleDownLeftRight,
        BarDoubleUpDownRight,
        BarDoubleLeftRight,
        BarDoubleUpDownLeftRight,
        BarUpDoubleLeftRight,
        BarDoubleUpSingleLeftRight,
        BarDownDoubleLeftRight,
        BarDoubleDownSingleLeftRight,
        BarDoubleUpSingleRight,
        BarUpDoubleRight,
        BarDownDoubleRight,
        BarDoubleDownSingleRight,
        BarDoubleUpDownSingleLeftRight,
        BarUpDownDoubleLeftRight,
        BarUpLeft,
        BarDownRight,
        BarDown,
        BarLeft,
        BarRight,
        BarUp,
        BarDoubleDown,
        BarDoubleLeft,
        BarDoubleRight,
        BarDoubleUp,
        TriangleUp,
        TriangleDown,
        TriangleRight,
        TriangleLeft,
        ArrowUp,
        ArrowDown,
        ArrowRight,
        ArrowLeft,
        Solid,
        SolidFill,
        Dark,
        DarkFill,
        Gray,
        GrayFill,
        Light,
        LightFill,
        HorizontalBars,
        HorizontalBarsFill,
        VerticalBars,
        VerticalBarsFill,
        Face,
        Mountains,
        Grass,
        TreeConical,
        TreeRound,
        Tombstone,
        Hill,
        TreeDots,
        TwoDots,
        Dashes,
        Door,
        Box,
    }

    public static class GlyphExtensions
    {
        static readonly char[] glyphMap =
      {
         ' ', // Space
         '!', // ExclamationMark
         '"', // Quote
         '#', // Pound
         '$', // DollarSign
         '%', // Percent
         '&', // Ampersand
         '\'', // Apostrophe
         '(', // OpenParenthesis
         ')', // CloseParenthesis
         '*', // Asterisk
         '+', // Plus
         ',', // Comma
         '-', // Dash
         '.', // Period
         '/', // Slash
         '0', // Digit0
         '1', // Digit1
         '2', // Digit2
         '3', // Digit3
         '4', // Digit4
         '5', // Digit5
         '6', // Digit6
         '7', // Digit7
         '8', // Digit8
         '9', // Digit9
         ':', // Colon
         ';', // Semicolon
         '<', // LessThan
         '=', // Equals
         '>', // GreaterThan
         '?', // QuestionMark
         '@', // At
         'A', // AUpper
         'B', // BUpper
         'C', // CUpper
         'D', // DUpper
         'E', // EUpper
         'F', // FUpper
         'G', // GUpper
         'H', // HUpper
         'I', // IUpper
         'J', // JUpper
         'K', // KUpper
         'L', // LUpper
         'M', // MUpper
         'N', // NUpper
         'O', // OUpper
         'P', // PUpper
         'Q', // QUpper
         'R', // RUpper
         'S', // SUpper
         'T', // TUpper
         'U', // UUpper
         'V', // VUpper
         'W', // WUpper
         'X', // XUpper
         'Y', // YUpper
         'Z', // ZUpper
         '[', // OpenBracket
         '\\', // Backslash
         ']', // CloseBracket
         '^', // Caret
         '_', // Underscore
         '`', // Accent
         'a', // ALower
         'b', // BLower
         'c', // CLower
         'd', // DLower
         'e', // ELower
         'f', // FLower
         'g', // GLower
         'h', // HLower
         'i', // ILower
         'j', // JLower
         'k', // KLower
         'l', // LLower
         'm', // MLower
         'n', // NLower
         'o', // OLower
         'p', // PLower
         'q', // QLower
         'r', // RLower
         's', // SLower
         't', // TLower
         'u', // ULower
         'v', // VLower
         'w', // WLower
         'x', // XLower
         'y', // YLower
         'z', // ZLower
         '{', // OpenBrace
         '|', // Pipe
         '}', // CloseBrace
         '~', // Tilde
         '◦', // Bullet
         '│', // BarUpDown
         '┤', // BarUpDownLeft
         '╡', // BarUpDownDoubleLeft
         '╢', // BarDoubleUpDownSingleLeft
         '╖', // BarDoubleDownSingleLeft
         '╕', // BarDownDoubleLeft
         '╣', // BarDoubleUpDownLeft
         '║', // BarDoubleUpDown
         '╗', // BarDoubleDownLeft
         '╝', // BarDoubleUpLeft
         '╜', // BarDoubleUpSingleLeft
         '╛', // BarUpDoubleLeft
         '┐', // BarDownLeft
         '└', // BarUpRight
         '┴', // BarUpLeftRight
         '┬', // BarDownLeftRight
         '├', // BarUpDownRight
         '─', // BarLeftRight
         '┼', // BarUpDownLeftRight
         ' ', // BarUpDownDoubleRight
         '╞', // BarDoubleUpDownSingleRight
         '╚', // BarDoubleUpRight
         '╔', // BarDoubleDownRight
         '╩', // BarDoubleUpLeftRight
         '╦', // BarDoubleDownLeftRight
         '╠', // BarDoubleUpDownRight
         '═', // BarDoubleLeftRight
         '╬', // BarDoubleUpDownLeftRight
         '╧', // BarUpDoubleLeftRight
         '╨', // BarDoubleUpSingleLeftRight
         '╤', // BarDownDoubleLeftRight
         '╥', // BarDoubleDownSingleLeftRight
         '╙', // BarDoubleUpSingleRight
         '╘', // BarUpDoubleRight
         '╒', // BarDownDoubleRight
         '╓', // BarDoubleDownSingleRight
         '╫', // BarDoubleUpDownSingleLeftRight
         '╪', // BarUpDownDoubleLeftRight
         '┘', // BarUpLeft
         '┌', // BarDownRight
         ' ', // BarDown
         ' ', // BarLeft
         ' ', // BarRight
         ' ', // BarUp
         ' ', // BarDoubleDown
         ' ', // BarDoubleLeft
         ' ', // BarDoubleRight
         ' ', // BarDoubleUp
         '▴', // TriangleUp
         '▾', // TriangleDown
         '▸', // TriangleRight
         '◂', // TriangleLeft
         '↑', // ArrowUp
         '↓', // ArrowDown
         '→', // ArrowRight
         '←', // ArrowLeft
         ' ', // Solid
         '█', // SolidFill
         ' ', // Dark
         '▓', // DarkFill
         ' ', // Gray
         '▒', // GrayFill
         ' ', // Light
         '░', // LightFill
         ' ', // HorizontalBars
         ' ', // HorizontalBarsFill
         ' ', // VerticalBars
         ' ', // VerticalBarsFill
         '☺', // Face
         ' ', // Mountains
         ' ', // Grass
         ' ', // TreeConical
         ' ', // TreeRound
         ' ', // Tombstone
         ' ', // Hill
         ' ', // TreeDots
         ' ', // TwoDots
         ' ', // Dashes
         ' ', // Door
         ' ', // Box
      };

        public static char ToChar(this Glyph glyph)
        {
            return glyphMap[(int)glyph];
        }

        public static Glyph[] ToGlyphs(this string str)
        {
            return str.ToCharArray().Select(c => (c >= ' ' && c <= '~') ? (Glyph)(c - ' ') : Glyph.Space).ToArray();
        }
    }
}
