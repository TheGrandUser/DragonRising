using DraconicEngine.GameWorld.EntitySystem;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Events
{
    public class CreatureKilledEventArgs
    {
        public Entity CreatureKilled { get; private set; }

        public string Cause { get; private set; }

        public Entity KillingEntity { get; private set; }

        public CreatureKilledEventArgs(Entity creatureKilled, string cause, Entity killingEntity = null)
        {
            this.CreatureKilled = creatureKilled;
            this.Cause = cause;
            this.KillingEntity = killingEntity;
        }
    }

    public class CreatureKilledEvent : PubSubEvent<CreatureKilledEventArgs>
    {
        public void Publish(Entity creatureKilled, string cause, Entity killingEntity = null)
        {
            this.Publish(new CreatureKilledEventArgs(creatureKilled, cause, killingEntity));
        }
    }
}
