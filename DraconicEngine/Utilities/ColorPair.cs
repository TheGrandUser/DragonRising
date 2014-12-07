using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
    public class ColorPair
    {
        public RogueColor Fore;
        public RogueColor? Back;

        public ColorPair(RogueColor fore, RogueColor? back)
        {
            Fore = fore;
            Back = back;
        }
    }
}
