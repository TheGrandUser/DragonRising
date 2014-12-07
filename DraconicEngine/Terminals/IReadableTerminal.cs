using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Terminals
{
    public interface IReadableTerminal
    {
        event EventHandler<CharacterEventArgs> CharacterChanged;

        Vector Size { get; }
        Loc LowerRight { get; }

        RogueColor ForeColor { get; }
        RogueColor? BackColor { get; }

        Character Get(Loc pos);
        Character Get(int x, int y);
    }
}
