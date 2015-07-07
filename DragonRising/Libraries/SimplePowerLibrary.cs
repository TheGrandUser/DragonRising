using DragonRising.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.GameWorld.Powers;

namespace DragonRising.Libraries
{
   class SimplePowerLibrary : IPowerLibrary
   {
      Dictionary<string, Power> usages = new Dictionary<string, Power>(StringComparer.InvariantCultureIgnoreCase);

      public void Add(Power power)
      {
         usages.Add(power.Name, power);
      }

      public bool Contains(string name)
      {
         return usages.ContainsKey(name);
      }

      public Power Get(string name)
      {
         return usages[name];
      }
   }
}
