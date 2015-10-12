using DragonRising.GameWorld.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DraconicEngine;

namespace DragonRising.GameWorld.Components
{
   public class CombatantComponent : Component
   {
      CharacterStat<int> hp;
      CharacterStat<int> maxHp;
      CharacterStat<int> power;
      CharacterStat<int> defense;
      CharacterStat<bool> isAlive;

      public int HP { get; set; }
      public int MaxHP { get; set; }
      public int Power { get; set; }
      public int Defense { get; set; }
      public bool IsAlive { get; set; } = true;

      public CombatantComponent()
      {
      }
      
      protected CombatantComponent(CombatantComponent original, bool fresh) : base(original, fresh)
      {
         this.MaxHP = original.MaxHP;
         this.Power = original.Power;
         this.Defense = original.Defense;

         this.HP = fresh ? original.MaxHP : original.HP;
         this.IsAlive = fresh | original.IsAlive;
      }

      public CombatantComponent(int hp, int defense, int power)
      {
         MaxHP = hp;
         HP = hp;
         Defense = defense;
         Power = power;
      }
      
      public void Heal(int amount)
      {
         this.HP = Math.Min(this.HP + amount, this.MaxHP);
      }

      protected override Component CloneCore(bool fresh)
      {
         return new CombatantComponent(this, fresh);
      }

      protected override void OnOwnerChanged(Entity oldOwner, Entity newOwner)
      {
         // Get stats
         hp = newOwner.GetStat<int>("HP");
         maxHp = newOwner.GetStat<int>("MaxHP");
         power = newOwner.GetStat<int>("Power");
         defense = newOwner.GetStat<int>("Defense");
         isAlive = newOwner.GetStat<bool>("IsAlive");
      }
   }
}
