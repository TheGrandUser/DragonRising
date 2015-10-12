using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tinkering
{
   abstract class Cost
   {
      sealed class NoCost : Cost
      {
      }

      public static Cost None { get; } = new Cost.NoCost();
   }

}
