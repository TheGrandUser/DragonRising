using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using DragonRising.Commands.Requirements;
using DraconicEngine.EntitySystem;
using DraconicEngine;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Powers;
using System.Collections.Immutable;
using DragonRising.Plans;
using DragonRising.GameWorld.Powers.Spells;
using DragonRising.Facts.Actions;

namespace DragonRising.GameWorld.Components
{
   public class ItemComponent : Component
   {
      public Usable Usable { get; set; }
      public EquipableUse EquipableUse { get; set; }
      public WeaponUse WeaponUse { get; set; }

      public ItemComponent() { }

      protected ItemComponent(ItemComponent original, bool fresh)
         : base(original, fresh)
      {
         this.Usable = original.Usable?.Clone(fresh);
         this.EquipableUse = original.EquipableUse?.Clone(fresh);
         this.WeaponUse = original.WeaponUse?.Clone(fresh);
      }

      protected override Component CloneCore(bool fresh)
      {
         return new ItemComponent(this, fresh);
      }

      protected override void OnOwnerChanged(Entity oldOwner, Entity newOwner)
      {
      }
   }

   public abstract class Usable
   {
      public Usable(EffectPlan plan)
      {
         this.Plan = plan;
      }

      protected Usable(Usable original, bool fresh)
      {
         this.Plan = original.Plan;
         this.MaxCharges = original.MaxCharges;
         this.IsCharged = original.IsCharged;
         this.Charges = fresh ? MaxCharges : original.Charges;
      }

      public EffectPlan Plan { get; }
      public int MaxCharges { get; set; }

      public int Charges { get; set; }
      public bool IsCharged { get; set; }

      public Usable Clone(bool fresh)
      {
         return CloneCore(fresh);
      }

      protected abstract Usable CloneCore(bool fresh);

      public abstract Fact GetFact(Entity user, FinalizedPlan<Scene> plan);
   }

   public class SpellUsable : Usable
   {
      Spell spell;

      public SpellUsable(Spell spell)
         : base(spell.Plan)
      {
         this.spell = spell;
      }

      protected SpellUsable(SpellUsable original, bool fresh)
         : base(original, fresh)
      {
         this.spell = original.spell;
      }

      public override Fact GetFact(Entity user, FinalizedPlan<Scene> plan)
      {
         return new CastSpellFact(user, spell, plan);
      }

      protected override Usable CloneCore(bool fresh)
      {
         return new SpellUsable(this, fresh);
      }
   }

   public class EquipableUse
   {
      public EquipmentSlot Slot { get; set; }

      // Stuff

      public EquipableUse Clone(bool fresh)
      {
         return new EquipableUse()
         {
            Slot = Slot
         };
      }
   }

   public class WeaponUse
   {
      public bool IsTwoHanded { get; set; }

      public int Power { get; set; }
      public int Range { get; set; }

      // takes ammo?

      public string DamageType { get; set; } = "Normal";

      public WeaponUse Clone(bool fresh)
      {
         return new WeaponUse()
         {
            IsTwoHanded = IsTwoHanded,
            Power = Power,
            Range = Range
         };
      }
   }

   public enum ItemUseResult
   {
      Used,
      Destroyed,
      NotUsed,
   }
}
