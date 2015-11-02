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
using DragonRising.Plans;

namespace DragonRising.GameWorld.Powers
{
   public static class CureMinorWoundsPower
   {
      public static EffectPlan CreateHealEffect(int amount = 4)
      {
         return EffectPlan.CreatePower("Cure Minor Wounds").Add(new HealEffect(4)).Finish();
      }
   }
}
