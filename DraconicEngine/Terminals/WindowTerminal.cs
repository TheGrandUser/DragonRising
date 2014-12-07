using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Terminals
{
    public class WindowTerminal : TerminalBase
    {
        public WindowTerminal(TerminalBase parent, RogueColor foreColor, RogueColor? backColor, TerminalRect bounds)
           : base(foreColor, backColor)
        {
            this.parent = parent;
            this.bounds = bounds;
        }

        public override Vector Size { get { return bounds.Size; } }
        public override Loc LowerRight { get { return bounds.BottomRight; } }

        protected override Character GetValueCore(Loc pos)
        {
            return this.parent.Get(pos + this.bounds.Position);
        }

        protected override bool SetValueCore(Loc pos, Character value)
        {
            if (!this.bounds.Size.Contains(pos)) return false;

            return this.parent.SetInternal(pos + this.bounds.Position, value);
        }

        internal override ITerminal CreateWindowCore(RogueColor foreColor, RogueColor? backColor, TerminalRect childBounds)
        {
            // transform by this window's bounds and then defer to the parent.
            // this flattens out a chain of windows at creation time so that
            // drawing commands don't have to burn cycles walking up the chain
            // each time. this shaves a measurable chunk of time off drawing.
            childBounds += this.bounds.Position;
            childBounds = childBounds.Intersection(this.bounds);

            return this.parent.CreateWindowCore(foreColor, backColor, childBounds);
        }

        private TerminalBase parent;
        public TerminalBase Parent { get { return parent; } }

        private TerminalRect bounds;
        public TerminalRect Bounds { get { return bounds; } }
    }
}
