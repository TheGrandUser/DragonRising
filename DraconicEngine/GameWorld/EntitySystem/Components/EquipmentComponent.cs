﻿using LanguageExt;
using LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   public enum EquipmentSlot
   {
      Held1,
      Held2,
      Body,
      Head,
      Ring1,
      Ring2,
      Neck,
      Outer,
      Feet,
      Hands,
      Legs
   }

   public class EquipmentComponent : Component
   {
      Entity weapon1;
      Entity weapon2;

      Dictionary<EquipmentSlot, Entity> equipped = new Dictionary<EquipmentSlot, Entity>();

      public EquipmentComponent()
      {
      }

      protected EquipmentComponent(EquipmentComponent original, bool fresh)
         : base(original, fresh)
      {

      }

      protected override Component CloneCore(bool fresh)
      {
         return new EquipmentComponent(this, fresh);
      }

      public IEnumerable<Entity> Weild(EquipmentSlot slot, Entity item)
      {
         Debug.Assert(item.HasComponent<ItemComponent>(), "Equipping entity is not an item");

         var itemComponent = item.GetComponent<ItemComponent>();

         if (itemComponent.WeaponUse.IsSome)
         {
            return itemComponent.WeaponUse.Some(wu =>
            {
               if (wu.IsTwoHanded)
               {
                  var old = weapon1.cons(weapon2.cons(empty<Entity>()));

                  weapon1 = item;
                  weapon2 = item;
                  equipped[EquipmentSlot.Held1] = item;
                  equipped[EquipmentSlot.Held2] = item;

                  return old;
               }
               else if (slot == EquipmentSlot.Held1)
               {
                  var old = weapon1.cons(empty<Entity>());
                  weapon1 = item;
                  equipped[EquipmentSlot.Held1] = item;
                  return old;
               }
               else
               {
                  var old = weapon2.cons(empty<Entity>());
                  weapon2 = item;
                  equipped[EquipmentSlot.Held2] = item;
                  return old;
               }
            }).None(empty<Entity>()).Where(i => i != null);
         }
         else if (itemComponent.EquipableUse.IsSome)
         {
            return itemComponent.EquipableUse.Some(eq =>
            {
               var old = Remove(eq.Slot);

               equipped[eq.Slot] = item;

               return old;
            }).None(None).Match(Some: o => EnumerableEx.Return(o), None: () => Enumerable.Empty<Entity>());
         }

         return Enumerable.Empty<Entity>();
      }

      public Option<Entity> Remove(EquipmentSlot slot)
      {
         Entity item;
         if (equipped.TryGetValue(slot, out item))
         {
            equipped.Remove(slot);
            return item;
         }

         return None;
      }

      public Option<Entity> GetItemInSlot(EquipmentSlot slot)
      {
         Entity item;
         if (equipped.TryGetValue(slot, out item))
         {
            return item;
         }
         return None;
      }
   }
}