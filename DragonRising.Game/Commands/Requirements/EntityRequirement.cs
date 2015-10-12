using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using DragonRising.GameWorld.Components;
using DragonRising.Commands.Requirements;
using DragonRising.GameWorld.Alligences;
using DragonRising.GameWorld.Actions;
using DragonRising.GameWorld;
using DraconicEngine.RulesSystem;

namespace DragonRising.Commands.Requirements
{
   public class EntityRequirement : PlanRequirement
   {
      public IImmutableList<Type> ComponentTypes { get; }

      public SelectionRange Range { get; }
      public bool ExcludeSelf { get; }
      public Relationship? Relationship2 { get; }

      public static PlanRequirement Any { get; } = new EntityRequirement(null, false, null);
      public static PlanRequirement AnyCreature { get; } = new EntityRequirement(null, false, null, typeof(CreatureComponent));
      public static PlanRequirement AnyCombatant { get; } = new EntityRequirement(null, false, null, typeof(CombatantComponent));

      public EntityRequirement(SelectionRange range, bool excludeSelf, Relationship? relationship, params Type[] componentTypes)
      {
         this.Range = range;
         this.ExcludeSelf = excludeSelf;
         this.Relationship2 = relationship;
         this.ComponentTypes = componentTypes.ToImmutableList();
      }
      
      public bool DoesEntityMatch(Entity entity)
      {
         return !this.ComponentTypes.Any(c => !entity.HasComponent(c));
      }

      public override bool MeetsRequirement(RequirementFulfillment fulfillment)
      {
         return fulfillment is EntityFulfillment;
      }


      public static PlanRequirement AnyCreatureWithinRange(SelectionRange range)
      {
         return new EntityRequirement(range, false, null, typeof(CreatureComponent));
      }
      public static PlanRequirement AnyEnemyWithinRange(SelectionRange range)
      {
         return new EntityRequirement(range, true, Relationship.Enemy, typeof(CreatureComponent));
      }
      public static PlanRequirement AnyAllyWithinRange(SelectionRange range)
      {
         return new EntityRequirement(range, false, Relationship.Ally, typeof(CreatureComponent));
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
