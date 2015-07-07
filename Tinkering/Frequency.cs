using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tinkering
{
   abstract class Frequency
   {
      sealed class AtWillFrequency : Frequency
      {
      }


      public static Frequency AtWill { get; } = new AtWillFrequency();
   }

   class RechargeFrequency : Frequency
   {
      public int Target { get; set; }
      public int Range { get; set; }
   }
}
