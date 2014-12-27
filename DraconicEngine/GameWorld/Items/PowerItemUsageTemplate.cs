﻿using DraconicEngine.Items;
using DraconicEngine.Powers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.Actions.Requirements;
using LanguageExt;
using LanguageExt.Prelude;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   public class PowerItemUsageComponent : IItemUsage
   {
      Power power;
      public Power Power { get { return power; } }

      public PowerItemUsageComponent(Power power)
      {
         this.power = power;
      }

      public bool Use(Entity user, Some<RequirementFulfillment> fulfillment)
      {
         if (DoFulfillmentsMatch(fulfillment))
         {
            this.Power.Do(user, fulfillment);
            return true;
         }
         return false;
      }

      bool DoFulfillmentsMatch(Some<RequirementFulfillment> fulfillment)
      {
         throw new NotImplementedException();
      }

      public Entity Owner { get; set; }

      public ActionRequirement Requirements
      {
         get
         {
            throw new NotImplementedException();
         }
      }
   }
}
