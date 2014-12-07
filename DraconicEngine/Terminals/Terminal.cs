using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Terminals
{
    public class Terminal : TerminalBase
    {
        private readonly Character[] characters;

        private int width;
        private int height;

        public override Vector Size { get { return new Vector(width, height); } }
        public override Loc LowerRight { get { return new Loc(width, height); } }

        public Terminal(int width, int height)
           : base()
        {
            this.width = width;
            this.height = height;
            characters = new Character[width * height];

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i] = new Character(' ');
            }
        }

        protected override Character GetValueCore(Loc pos)
        {
            return characters[pos.Y * width + pos.X];
        }

        protected override bool SetValueCore(Loc pos, Character value)
        {
            var index = pos.Y * width + pos.X;
            // don't do anything if the value doesn't change
            if (characters[index].Equals(value)) return false;

            if (value.BackColor == null)
            {
                var current = characters[index];
                value = new Character(value.Glyph, value.ForeColor, current.BackColor);
            }

            characters[index] = value;
            return true;
        }

        internal override ITerminal CreateWindowCore(RogueColor foreColor, RogueColor? backColor, TerminalRect bounds)
        {
            return new WindowTerminal(this, foreColor, backColor, bounds);
        }
    }
}
