using DragonRising.GameWorld.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld;
using DraconicEngine;

namespace DragonRising.GameWorld.Components
{
   public class CombatantComponent : Component
   {
      public int HP { get; set; }
      public int MaxHP { get; set; }
      public int Power { get; set; }
      public int Defense { get; set; }
      public int XP { get; set; }
      public bool IsAlive { get; set; } = true;

      public CombatantComponent()
      {
      }
      

      protected CombatantComponent(CombatantComponent original, bool fresh) : base(original, fresh)
      {
         
         this.MaxHP = original.MaxHP;
         this.Power = original.Power;
         this.Defense = original.Defense;
         this.XP = original.XP;

         this.HP = fresh ? original.MaxHP : original.HP;
         this.IsAlive = fresh | original.IsAlive;
      }

      public CombatantComponent(int hp, int xp, int defense, int power)
      {
         MaxHP = hp;
         HP = hp;
         XP = xp;
         Defense = defense;
         Power = power;
      }

      public void TakeDamage(int damage, Entity from)
      {
         if (damage > 0)
         {
            this.HP -= damage;

            if (this.HP <= 0)
            {
               this.IsAlive = false;
               this.Owner.SetBlocks(false);
               World.Current.Scene.EntityStore.KillEntity(this.Owner);
               var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
               var creatureKilledEvent = eventAggregator.GetEvent<CreatureKilledEvent>();
               creatureKilledEvent.Publish(this.Owner, "Entity", from);
            }
         }
      }

      public void Heal(int amount)
      {
         this.HP = Math.Min(this.HP + amount, this.MaxHP);
      }

      protected override Component CloneCore(bool fresh)
      {
         return new CombatantComponent(this, fresh);
      }
   }
}
