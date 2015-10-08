using DraconicEngine.EntitySystem;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Effects;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.GameWorld.Conditions;

namespace DragonRising.GameWorld.Events
{
   class AddConditionEvent : Fact
   {
      public AddConditionEvent(Some<Entity> entity, ICondition condition, int? duration = null)
      {
         Entity = entity;
         Condition = condition;
         Duration = duration;
      }

      public Entity Entity { get; }
      public ICondition Condition { get; }
      public int? Duration { get; }
   }

   class ConditionExpiredEvent : Fact
   {
      public ConditionExpiredEvent(Some<Entity> entity, ICondition condition)
      {
         Entity = entity;
         Condition = condition;
      }

      public Entity Entity { get; }
      public ICondition Condition { get; }
   }

   class ConditionRemovedEvent : Fact
   {
      public ConditionRemovedEvent(Some<Entity> entity, ICondition condition)
      {
         Entity = entity;
         Condition = condition;
      }

      public Entity Entity { get; }
      public ICondition Condition { get; }
   }
}
