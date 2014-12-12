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
      public CombatantComponentTemplate Template { get; set; }
      public int HP { get; set; }
      public int MaxHP => Template.MaxHP;
      public int Power => Template.Power;
      public int Defense => Template.Defense;

      public bool IsAlive { get; set; } = true;

      public CombatantComponent(CombatantComponentTemplate template)
      {
         this.Template = template;
         this.HP = template.MaxHP;
      }

      public void TakeDamage(int damage, Entity from)
      {
         if (damage > 0)
         {
            this.HP -= damage;

            if (this.HP <= 0)
            {
               this.IsAlive = false;
               this.Owner.Blocks = false;
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

   public class CombatantComponentTemplate : ComponentTemplate
   {
      public CombatantComponentTemplate() { }
      public CombatantComponentTemplate(int hp, int defense, int power)
      {
         this.MaxHP = hp;
         this.Defense = defense;
         this.Power = power;
      }
      public int MaxHP { get; set; }
      public int Defense { get; set; }
      public int Power { get; set; }
      public override Type ComponentType => typeof(CombatantComponent);

      public override Component CreateComponent()
      {
         return new CombatantComponent(this);
      }
   }
}
