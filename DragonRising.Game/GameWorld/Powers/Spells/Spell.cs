using DragonRising.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Powers.Spells
{
   public class Spell
   {
      public Spell(string name, EffectPlan plan)
      {
         Name = name;
         Plan = plan;
      }

      public string Name { get; }
      public EffectPlan Plan { get; }
   }
}
