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
   class Effect
   {
      class NoEffect : Effect { }

      public static Effect None { get; } = new NoEffect();
   }

   class HalfDamageEffect : Effect
   {
      public DamageEffect Effect { get; set; }
      public string Check { get; set; }
   }

   class ReducePoolEffect : Effect
   {

   }

   class DamageEffect : Effect
   {
      public Entity Attacker { get; internal set; }
      public int Damage { get; set; }
      public string Tag { get; set; }
      public Entity Target { get; internal set; }
   }
}
