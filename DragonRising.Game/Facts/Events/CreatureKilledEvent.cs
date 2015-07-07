using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using LanguageExt;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Events
{
   public class CreatureKilledEvent : Fact
   {
      public Entity CreatureKilled { get; private set; }

      public string Cause { get; private set; }

      public Option<Entity> KillingEntity { get; private set; }

      public CreatureKilledEvent(Some<Entity> creatureKilled, Some<string> cause, Entity killingEntity = null)
      {
         this.CreatureKilled = creatureKilled;
         this.Cause = cause;
         this.KillingEntity = killingEntity;
      }
   }

   public class CreatureKilledPubSubEvent : PubSubEvent<CreatureKilledEvent>
   {
      public void Publish(Entity creatureKilled, string cause, Entity killingEntity = null)
      {
         this.Publish(new CreatureKilledEvent(creatureKilled, cause, killingEntity));
      }
   }
}
