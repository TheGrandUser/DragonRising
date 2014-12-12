using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using System.Reactive.Linq;

namespace DragonRising.Services
{
   class LifeDeathMonitorService
   {
      IDisposable subscriptions;

      public LifeDeathMonitorService(IEntityStore store)
      {
         subscriptions = store.Killed.Subscribe(OnKilled);
      }

      private void OnKilled(Entity entity)
      {
         if (entity == Scene.CurrentScene.FocusEntity)
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

         this.PlayerCreature.Character = new Character(Glyph.Percent, RogueColors.DarkRed);
      }
      public void NpcDeath(Entity npc)
      {
         MessageService.Current.PostMessage(npc.Name + " is dead!", RogueColors.Orange);
         npc.Character = new Character(Glyph.Percent, RogueColors.DarkRed);
         npc.Blocks = false;
         //npc.RemoveComponent<CombatantComponent>();
         //npc.RemoveComponent<CreatureComponent>();
         npc.GetComponent<BehaviorComponent>().ClearBehaviors();
         npc.Name = "remains of " + npc.Name;

         Scene.CurrentScene.EntityStore.SendToBack(npc);
      }
   }
}
