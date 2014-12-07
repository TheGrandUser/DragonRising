using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
    public class RogueMessage
    {
        public string Message { get; private set; }
        public RogueColor Color { get; private set; }

        public RogueMessage(string message, RogueColor color)
        {
            this.Message = message;
            this.Color = color;
        }
    }
}
