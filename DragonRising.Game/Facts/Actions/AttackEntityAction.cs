using DraconicEngine;
using DraconicEngine.RulesSystem;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Components;
using DragonRising.Rules;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameWorld.Actions
{
   public class AttackEntityAction : ActionTaken
   {
      public Entity Attacker { get; set; }
      public Entity Target { get; set; }
      public Option<Entity> Weapon { get; set; }

      public AttackEntityAction(Entity attacker, Entity target, Option<Entity> weapon)
      {
         Contract.Requires(target != null);
         Contract.Requires(weapon.ForAll(w => w.HasComponent<ItemComponent>() && w.GetComponent<ItemComponent>().WeaponUse != null));
         Attacker = attacker;
         Target = target;
         Weapon = weapon;
      }
   }
}
