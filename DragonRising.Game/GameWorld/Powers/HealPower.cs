using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.Commands.Requirements;
using LanguageExt;
using static LanguageExt.Prelude;
using DragonRising.GameWorld.Components;
using System.Collections.Immutable;
using DraconicEngine.RulesSystem;
using DragonRising.Facts.Events;
using System.Diagnostics;
using DragonRising.Plans.Effects;

namespace DragonRising.GameWorld.Powers
{
   class CureMinorWoundsPower : Power
   {
      IEffect effect;

      public CureMinorWoundsPower(int amount = 4)
         : base("Cure Minor Wounds")
      {
         effect = new HealEffect(amount);
      }

      public override IEnumerable<IEffect> Effects => EnumerableEx.Return(effect);
   }
}
