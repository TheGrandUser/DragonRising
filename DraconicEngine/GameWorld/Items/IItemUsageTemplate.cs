using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Items
{
   public interface IItemUsageTemplate
   {
      string Name { get; set; }
      Type UsageType { get; }

      IItemUsage CreateUsage();
   }
}
