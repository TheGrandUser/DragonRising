using DraconicEngine.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   public class CombatantComponent : Component
   {
      public int MaxHP { get; set; }
      public int HP { get; set; }
      public int Defense { get; set; }
      public int Power { get; set; }

      public CombatantComponent(int hp, int defense, int power)
      {
         this.MaxHP = this.HP = hp;
         this.Defense = defense;
         this.Power = power;
      }

      public void TakeDamage(int damage, Entity from)
      {
         if (damage > 0)
         {
            this.HP -= damage;

            if (this.HP <= 0)
            {
               Scene.CurrentScene.EntityStore.KillEntity(this.Owner);
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
   }
}
