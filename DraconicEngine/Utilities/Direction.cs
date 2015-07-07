using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine
{
   public enum Direction
   {
      None,
      East,
      Southeast,
      South,
      Southwest,
      West,
      Northwest,
      North,
      Northeast,
      Up,
      Down,
   }

   [Flags]
   public enum Edges
   {
      East,
      South,
      West,
      North
   }
   
   public enum DirectionLimit
   {
      EightWay,
      Cardinal,
      FullVector,
   }
}
