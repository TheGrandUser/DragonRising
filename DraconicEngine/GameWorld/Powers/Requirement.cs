using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Powers
{
   public enum RequirementType
   {
      Entity,
      Creature,
      Object,
      Item,
      Location,
   }

   public class Requirement
   {
      // Entity
      // Location
      // Direction
      // Number
      // String

      // Preview style/shape (just the entity/cell, or area around cell)

      public string Description { get; set; }
      public RequirementType Type { get; set; }
      public int? RangeLimit { get; set; }

      public ImmutableList<IRestriction> Restrictions { get; set; }
   }

   // Single entity/locations(w/direction)
   // - Distance
   // Sequence of entities/locations(w/direction)
   // - Not greater than X from origin and/or Y from last
   // Wall of locations w/direction

   public interface IRestriction
   {
      // Kinds of restrictions
      // Origin cell + radius/length/shape/area + direction
      // - Origin can be based on prior selections, such as chain
      //   lightning not striking targets X away from the first
      // - Unless otherwise specified, origin is the caster's cell

      // LoS/LoE or penetrating

      Loc GetOrigin();

      bool Qualifies(Entity entity);
      bool Qualifies(Loc location);
   }

   public class RequirementEntity : Requirement
   {
      // Restrictions
      // Entity Type
      // - Enemies or allies, same creature type, or specific type (such as animals)

      public FulfilmentEntity Fulfil(Entity value, ImmutableList<Fulfilment> dependantFulfilments)
      {
         throw new NotImplementedException();
      }
   }

   public class RequirementCreature : Requirement
   {
      // Restrictions
      // Entity Type
      // - Enemies or allies, same creature type, or specific type (such as animals)

      public FulfilmentCreature Fulfil(Entity value, ImmutableList<Fulfilment> dependantFulfilments)
      {
         throw new NotImplementedException();
      }
   }

   public class RequirementItem : Requirement
   {
      // Restrictions
      // Entity Type
      // - Enemies or allies, same creature type, or specific type (such as animals)

      public FulfilmentItem Fulfil(Entity value, ImmutableList<Fulfilment> dependantFulfilments)
      {
         throw new NotImplementedException();
      }
   }

   public class RequirementLocation : Requirement
   {
      public FulfilmentLocation Fulfil(Loc value, ImmutableList<Fulfilment> dependantFulfilments)
      {
         throw new NotImplementedException();
      }
   }
}