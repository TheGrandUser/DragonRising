using DragonRising.Commands.Requirements;
using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.RulesSystem;
using DraconicEngine;
using LanguageExt;

namespace DragonRising.Plans
{
   public sealed class EffectPlan
   {
      EffectPlan(IEnumerable<IFromLocationTargeter> targeters, IEnumerable<IFromLocationQuery> queries, IEnumerable<IEntityEffect> effects)
      {
         Targeters = targeters;
         Queries = queries;
         Effects = effects;
      }
      
      public IEnumerable<IFromLocationTargeter> Targeters { get; } = Enumerable.Empty<IFromLocationTargeter>();
      public IEnumerable<IFromLocationQuery> Queries { get; } = Enumerable.Empty<IFromLocationQuery>();
      public IEnumerable<IEntityEffect> Effects { get; } = Enumerable.Empty<IEntityEffect>();

      public static IPowerBuilder CreatePower(string name) => new PowerBuilder();

      class PowerBuilder : IPowerBuilder
      {
         readonly ImmutableList<IFromLocationTargeter> targeters;
         readonly ImmutableList<IFromLocationQuery> queries;
         readonly ImmutableList<IEntityEffect> effects;

         public PowerBuilder()
         {
            targeters = ImmutableList<IFromLocationTargeter>.Empty;
            queries = ImmutableList<IFromLocationQuery>.Empty;
            effects = ImmutableList<IEntityEffect>.Empty;
         }

         PowerBuilder(PowerBuilder original, IFromLocationTargeter targeter)
         {
            targeters = original.targeters.Add(targeter);
            queries = original.queries;
            effects = original.effects;
         }

         PowerBuilder(PowerBuilder original, IFromLocationQuery query)
         {
            targeters = original.targeters;
            queries = original.queries.Add(query);
            effects = original.effects;
         }

         PowerBuilder(PowerBuilder original, IEntityEffect effect)
         {
            targeters = original.targeters;
            queries = original.queries;
            effects = original.effects.Add(effect);
         }

         public IPowerBuilder Add(IEntityEffect effect) => new PowerBuilder(this, effect);

         public IPowerBuilder Add(IFromLocationQuery query) => new PowerBuilder(this, query);

         public IPowerBuilder Add(IFromLocationTargeter targeter) => new PowerBuilder(this, targeter);

         public EffectPlan Finish() => new EffectPlan(targeters, queries, effects);
      }

      // A Power is an acyclical directed Graph
      // A Power has a one or more Root Nodes, zero or more Nodes, and one or more Effects
      // A Root Node can be an Input, Fetch, or Constant
      // A Node can be a Calculation or Enumeration
      // An Effect only applies to one Entity or one Location

      // Any Input node can have a dependency on another Node
      // (generally it will be a max distance limiter)



      // A Power has Zero or more Requirements
      // A Power has one or more Target Selectors(TS) (entities/locations)

      // Each TS has one Root Step and zero or more Steps
      // TS's can share Steps

      // Certain Kinds of Steps have one Requirement, others do not
      // Each Requirement is tied to a one or more Steps
      // A Requirement can depend on a zero or more prior Requirements
      // Each Step on a TS has zero or one Requirements
      // Each TS has one or more Effects

      // Nodes:

      // Constants:
      // Self
      // Number
      // String

      // Input:
      // SelectEntity, Requirement of One Entity of type
      // - Specific types: GetCreature, GetItem, GetObject
      // SelectLocation, Requirement of One Location
      // SelectDireciton, Requirement of One Direction
      // InputNumber, Requirement of One integer value
      // InputString, Requirement of One string value

      // Fetches:
      // Check Skill
      // Find by name

      // Enumerations:
      // AllWithin (of entity/location)


      // A Root Step can be either Self, GetEntity, GetLocation, or GetDirection
      // A Root Step without a Requirement targets Self or the Location of Self

      // A Step's Requirement can be one entity, one location, direction or a radius


      // Examples:
      // Regular Attack: One TS, Root Step (Get Creature), creature => step (CheckSKill), creature => Effect (Damage creature X amount)
      // Power Attack: One TS, Root Step (Get Creature), creature => GetValue, creature, value => Effect (Damage creature X+value amount)
      // Heal Self: One TS, Root Step (Self), Effect (Heal X amount)
      // Fireball: One TS, Root Step (Get Location), loc => Step (Of Entities, all within Radius of loc), entity => Effect (Damage entity X amount)
      // Chain Lightning: Five Ts's (for five targets), for each Root Step (Get Creature)
   }

   public interface IPowerBuilder
   {
      IPowerBuilder Add(IFromLocationTargeter targeter);
      IPowerBuilder Add(IFromLocationQuery query);
      IPowerBuilder Add(IEntityEffect effect);
      EffectPlan Finish();
   }
}