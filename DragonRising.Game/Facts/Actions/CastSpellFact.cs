using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Powers.Spells;

namespace DragonRising.Facts.Actions
{
   class CastSpellFact : Fact
   {
      public FinalizedPlan<Scene> Plan { get; }
      public Spell Spell { get; }
      public Entity User { get; }

      public CastSpellFact(Entity user, Spell spell, FinalizedPlan<Scene> plan)
      {
         this.User = user;
         this.Spell = spell;
         this.Plan = plan;
      }
   }
}
