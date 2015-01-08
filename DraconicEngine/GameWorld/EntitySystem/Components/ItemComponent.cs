﻿using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Prelude;
using DraconicEngine.GameWorld.Actions.Requirements;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   public class ItemComponent : Component
   {
      public Option<Usable> Usable { get; set; }
      public Option<EquipableUse> EquipableUse { get; set; }
      public Option<WeaponUse> WeaponUse { get; set; }

      public ItemComponent() { }

      protected ItemComponent(ItemComponent original, bool fresh)
         : base(original, fresh)
      {
         this.Usable = original.Usable.Bind(cu => (Option<Usable>)cu.Clone(fresh));
         this.EquipableUse = original.EquipableUse.Bind(cu => (Option<EquipableUse>)cu.Clone(fresh));
         this.WeaponUse = original.WeaponUse.Bind(cu => (Option<WeaponUse>)cu.Clone(fresh));
      }

      protected override Component CloneCore(bool fresh)
      {
         return new ItemComponent(this, fresh);
      }
   }

   public class Usable
   {
      public IItemUsage Usage { get; set; }
      public int MaxCharges { get; set; }

      public int Charges { get; set; }
      public bool IsCharged { get; set; }

      public Usable Clone(bool fresh)
      {
         return new Usable()
         {
            Usage = Usage,
            MaxCharges = MaxCharges,
            IsCharged = IsCharged,
            Charges = fresh ? MaxCharges : Charges
         };
      }

      public ItemUseResult Use(Entity user, Some<RequirementFulfillment> itemsRequirements)
      {
         var usage = this.Usage;
         if (usage != null)
         {
            if (usage.Use(user, itemsRequirements))
            {
               if (this.IsCharged)
               {
                  this.Charges--;

                  if (this.Charges <= 0)
                  {
                     return ItemUseResult.Destroyed;
                  }
               }

               return ItemUseResult.Used;
            }
         }

         return ItemUseResult.NotUsed;
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
