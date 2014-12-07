using DraconicEngine.Items;
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
   public class PowerItemUsageTemplate : IItemUsageTemplate
   {
      Power power;
      public Power Power { get { return power; } }
      public string Name { get; set; }

      IResultSelector resultSelector;
      public IResultSelector ResultSelector { get { return resultSelector; } }

      public PowerItemUsageTemplate(Power power, IResultSelector resultSelector)
      {
         this.power = power;
         this.resultSelector = resultSelector;
      }

      public Type UsageType { get { return typeof(PowerItemUsageComponent); } }

      public IItemUsage CreateUsage()
      {
         return new PowerItemUsageComponent(this);
      }
   }

   public class PowerItemUsageComponent : IItemUsage
   {
      List<Fulfilment> fulfilments;
      PowerItemUsageTemplate template;


      public PowerItemUsageComponent(PowerItemUsageTemplate template)
      {
         this.template = template;
      }

      public async Task<ItemUseResult> PrepUseAsync(Entity user)
      {
         if (this.template.Power.Requirements.Count > 0)
         {
            this.fulfilments = new List<Fulfilment>();
            foreach (var requirement in this.template.Power.Requirements)
            {
               Fulfilment fulfilment = await Fulfil(requirement);

               if (fulfilment == null)
               {
                  this.fulfilments.Clear();
                  this.fulfilments = null;
                  return ItemUseResult.NotUsed;
               }

               this.fulfilments.Add(fulfilment);
            }
         }
         return this.template.ResultSelector.Select(user, this.fulfilments);
      }

      private Task<Fulfilment> Fulfil(Requirement requirement)
      {
         throw new NotImplementedException();
      }

      public void Use(Entity user)
      {
         this.template.Power.Do(user, ImmutableList.CreateRange(this.fulfilments));

         this.fulfilments.Clear();
         this.fulfilments = null;
      }

      public ItemUseResult PrepUse(Entity user, Some<RequirementFulfillment> fulfillment)
      {
         throw new NotImplementedException();
      }

      public Entity Owner { get; set; }

      public IItemUsageTemplate Template { get { return this.template; } }

      public ActionRequirement Requirements
      {
         get
         {
            throw new NotImplementedException();
         }
      }
   }
}
