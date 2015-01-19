using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using Microsoft.Practices.Prism.PubSubEvents;
using DragonRising.GameWorld.Events;

namespace DragonRising
{
   class StatTracker
   {
      SubscriptionToken subscription;

      int monstersKilled = 0;
      public int MonstersKilled { get { return monstersKilled; } }

      public StatTracker(IEventAggregator eventAggregator)
      {
         var @event = eventAggregator.GetEvent<CreatureKilledEvent>();
         this.subscription = @event.Subscribe(OnCreatureKilled);
      }

      private void OnCreatureKilled(CreatureKilledEventArgs args)
      {
         if (args.Cause == "Entity" && args.KillingEntity == Scene.CurrentScene.FocusEntity)
         {
            monstersKilled++;
         }
      }
   }
}
