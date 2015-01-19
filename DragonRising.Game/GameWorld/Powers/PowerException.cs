using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Powers
{
   public class PowerException : Exception
   {
      public PowerException()
      {
      }

      public PowerException(string message)
         : base(message)
      {

      }
   }
}
