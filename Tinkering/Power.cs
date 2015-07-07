using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising;
using DragonRising.GameWorld;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Alligences;
using DraconicEngine;

namespace Tinkering
{
   class Power
   {
      public string Name { get; set; }
      public List<ParameterInfo> Parameters { get; set; }
      public Frequency Frequency { get; set; }
      public Cost Cost { get; set; }
      public IProcedure Procedure { get; set; }
   }
}
