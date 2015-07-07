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
   abstract class ParameterInfo
   {
      public string Name { get; set; }
      public abstract Type Type { get; }
      public int? DistanceLimit { get; set; }
   }

   class ParameterInfo<T> : ParameterInfo
   {
      public override Type Type => typeof(T);
   }
}
