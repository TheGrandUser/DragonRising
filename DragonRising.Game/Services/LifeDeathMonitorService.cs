using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using System.Reactive.Linq;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld;
using Microsoft.Practices.Prism.PubSubEvents;
using DragonRising.GameWorld.Events;

namespace DragonRising.Services
{
   class LifeDeathMonitorService : IDisposable
   {
      IDisposable subscriptions;

      public LifeDeathMonitorService(IEventAggregator eventAggregator)
      {
         var creatureKilledEvent = eventAggregator.GetEvent<CreatureKilledEvent>();

         subscriptions = creatureKilledEvent.Subscribe(OnKilled);
      }

      private void OnKilled(CreatureKilledEventArgs args)
      {
         var entity = args.CreatureKilled;
         if (entity == World.Current.Player)
         {
            PlayerDeath(entity);
         }
         else
         {
            NpcDeath(entity);
         }
      }
      
      public Entity PlayerCreature { get; set; }

      private bool isPlayerDead;
      public bool HasPlayerLost { get { return isPlayerDead; } }

      public void PlayerDeath(Entity obj)
      {
         MessageService.Current.PostMessage("You have died!", RogueColors.Red);

         this.isPlayerDead = true;

         this.PlayerCreature.As<DrawnComponent>(dc => dc.SeenCharacter = new Character(Glyph.Percent, RogueColors.DarkRed));
         this.PlayerCreature.As<LocationComponent>(lc => lc.Blocks = false);
         this.PlayerCreature.As<BehaviorComponent>(bc => bc.ClearBehaviors());
      }
      public void NpcDeath(Entity npc)
      {
         var npcCombat = npc.GetComponent<CombatantComponent>();

         MessageService.Current.PostMessage($"{npc.Name} is dead! You gain {npcCombat.XP} experience points", RogueColors.Orange);
         
         npc.As<DrawnComponent>(dc => dc.SeenCharacter = new Character(Glyph.Percent, RogueColors.DarkRed));
         npc.As<LocationComponent>(lc => lc.Blocks = false);
         
         npc.Name = "remains of " + npc.Name;

         npc.RemoveComponent<BehaviorComponent>();
         npc.RemoveComponent<CreatureComponent>();


         World.Current.Scene.EntityStore.SendToBack(npc);
      }

      public void Dispose()
      {
         this.subscriptions.Dispose();
      }
   }
}
