using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Powers
{
   public class Fulfilment
   {
      public Requirement Requirement { get; protected set; }
   }

   public class FulfilmentEntity : Fulfilment
   {
      public new RequirementEntity Requirement { get { return (RequirementEntity)base.Requirement; } set { base.Requirement = value; } }
      // Value

      public Entity Value { get; private set; }
   }

   public class FulfilmentCreature : Fulfilment
   {
      public new RequirementCreature Requirement { get { return (RequirementCreature)base.Requirement; } set { base.Requirement = value; } }

      public Entity Value { get; private set; }
   }

   public class FulfilmentItem : Fulfilment
   {
      public new RequirementItem Requirement { get { return (RequirementItem)base.Requirement; } set { base.Requirement = value; } }
      // Value
      public Entity Value { get; private set; }
   }

   public class FulfilmentLocation : Fulfilment
   {
      public new RequirementLocation Requirement { get { return (RequirementLocation)base.Requirement; } set { base.Requirement = value; } }
      // Value
      public Loc Value { get; private set; }
   }
}
