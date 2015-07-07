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

   public class Usable
   {
      public Usable(Power power)
      {
         this.Power = power;
      }

      public Usable(Usable original, bool fresh)
      {
         this.Power = original.Power;
         this.MaxCharges = original.MaxCharges;
         this.IsCharged = original.IsCharged;
         this.Charges = fresh ? MaxCharges : original.Charges;
      }

      public Power Power { get; }
      public int MaxCharges { get; set; }

      public int Charges { get; set; }
      public bool IsCharged { get; set; }

      public Usable Clone(bool fresh)
      {
         return new Usable(this, fresh);
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
