using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Facts.Events
{
   class CreatureHealedEvent : Fact
   {
      public CreatureHealedEvent(Entity sender, Entity target, int amount)
      {
         Sender = sender;
         Target = target;
         Amount = amount;
      }

      public Entity Sender { get; }
      public Entity Target { get; }
      public int Amount { get; }
   }
}
