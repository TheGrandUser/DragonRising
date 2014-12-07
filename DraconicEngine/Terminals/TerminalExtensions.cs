using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Terminals
{
    public static class TerminalExtensions
    {
        public static void DrawBar(this ITerminal panel, int x, int y, int totalWidth, string name, int value, int maximum, RogueColor barColor, RogueColor backColor)
        {
            var barWidth = (int)(totalWidth * (double)value / maximum);

            var barTerminal = panel[x, y, totalWidth, 1];

            barTerminal[RogueColors.Black, backColor].Fill(Glyph.Space);
            if (barWidth > 0)
            {
                barTerminal[RogueColors.Black, barColor][0, 0, barWidth, 1].Fill(Glyph.Space);
            }

            string message = string.Format("{0}: {1}/{2}", name, value, maximum);
            var margin = (totalWidth - message.Length) / 2;
            barTerminal[margin, 0][RogueColors.White, null].Write(message);
        }

        public static Loc? RootVecToLocalVec(this ITerminal terminal, Loc rootPosition)
        {
            if (terminal is Terminal)
            {
                if (rootPosition.X < 0 || rootPosition.X >= terminal.Size.X ||
                   rootPosition.Y < 0 || rootPosition.Y >= terminal.Size.Y)
                {
                    return null;
                }
                return rootPosition;
            }

            Stack<TerminalRect> childBounds = new Stack<TerminalRect>();
            ITerminal current = terminal;
            do
            {
                if (current is WindowTerminal)
                {
                    var windowTerminal = (WindowTerminal)terminal;

                    childBounds.Push(windowTerminal.Bounds);
                    current = windowTerminal.Parent;
                }
                else
                {
                    childBounds.Push(new TerminalRect(current.Size));
                    current = null;
                }
            } while (current != null);

            Loc currentPosition = rootPosition;

            while (childBounds.Count > 0)
            {
                var bounds = childBounds.Pop();
                Vector offset = (Vector)bounds.TopLeft;
                currentPosition -= offset;

                if (currentPosition.X < 0 || currentPosition.X >= bounds.Width ||
                   currentPosition.Y < 0 || currentPosition.Y >= bounds.Height)
                {
                    return null;
                }
            }

            return currentPosition;
        }

        public static void SetHighlight(this ITerminal terminal, Loc location)
        {
            var currentCharacter = terminal.Get(location);

            terminal.Set(location, new Character(currentCharacter.Glyph, RogueColors.Black, RogueColors.LightCyan));
        }

        public static void SetHighlight(this ITerminal terminal, Loc location, RogueColor foreColor, RogueColor backColor)
        {
            var currentCharacter = terminal.Get(location);

            terminal.Set(location, new Character(currentCharacter.Glyph, foreColor, backColor));
        }
    }
}
