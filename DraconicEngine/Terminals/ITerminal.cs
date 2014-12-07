using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Terminals
{
    public enum DrawBoxOptions
    {
        None = 0x0,
        DoubleLines = 0x1,
        ContinueLines = 0x2,

        Default = ContinueLines
    }

    public interface ITerminal : IReadableTerminal
    {
        void Write(char ascii);
        void Write(Glyph glyph);
        void Write(Character character);
        void Write(string text);
        void Write(CharacterString text);

        void Scroll(Vector offset, Func<Loc, Character> scrollOnCallback);
        void Scroll(int x, int y, Func<Loc, Character> scrollOnCallback);

        void Clear();

        void Fill(Glyph glyph);

        void DrawBox();
        void DrawBox(DrawBoxOptions options);

        ITerminal this[RogueColor foreColor] { get; }
        ITerminal this[RogueColor foreColor, RogueColor? backColor] { get; }
        ITerminal this[ColorPair color] { get; }

        ITerminal this[Loc pos] { get; }
        ITerminal this[int x, int y] { get; }
        ITerminal this[TerminalRect rect] { get; }
        ITerminal this[Loc pos, Vector size] { get; }
        ITerminal this[int x, int y, int width, int height] { get; }

        void Set(Loc pos, Character value);
        void Set(int x, int y, Character value);
    }
}
