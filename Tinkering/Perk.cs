using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tinkering
{
   class Perk
   {
      public string Name { get; set; }
   }

   class MobilityPerk
   {
      public string Type { get; set; }
      public int Speed { get; set; }
   }

   class PowerPerk : Perk
   {
      public Power Power { get; set; }
   }
}
