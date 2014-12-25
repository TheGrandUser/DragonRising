using DraconicEngine.GameWorld.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Actions.Requirements
{
   public class EntityRequirement : ActionRequirement
   {
      public IImmutableList<Type> ComponentTypes { get; }

      public int? MaxRange { get; }

      public EntityRequirement(int? maxRange, params Type[] componentTypes)
      {
         this.MaxRange = maxRange;
         this.ComponentTypes = ImmutableList.CreateRange(componentTypes);
      }

      public EntityRequirement(int? maxRange, IEnumerable<Type> componentTypes)
      {
         this.MaxRange = maxRange;
         this.ComponentTypes = ImmutableList.CreateRange(componentTypes);
      }

      public bool DoesEntityMatch(Entity entity)
      {
         return !this.ComponentTypes.Any(c => !entity.HasComponent(c));
      }

      public override bool MeetsRequirement(RequirementFulfillment fulfillment)
      {
         return fulfillment is EntityFulfillment;
      }
   }

   public class EntityFulfillment : RequirementFulfillment
   {
      public Entity Entity { get; }
      public EntityFulfillment(Entity entity)
      {
         this.Entity = entity;
      }
   }
}
