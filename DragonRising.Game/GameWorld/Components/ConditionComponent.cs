using DraconicEngine.EntitySystem;
using DragonRising.GameWorld.Effects;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.GameWorld.Conditions;

namespace DragonRising.GameWorld.Components
{
   public sealed class ConditionComponent : Component
   {
      Dictionary<Type, ICondition> conditions = new Dictionary<Type, ICondition>();

      public ConditionComponent()
      {
      }

      public ConditionComponent(ConditionComponent original, bool fresh)
      {

      }

      protected override Component CloneCore(bool fresh)
      {
         return new ConditionComponent(this, fresh);
      }

      public bool HasStatus<T>() where T : ICondition => conditions.ContainsKey(typeof(T));
      public Option<T> GetStatus<T>() where T : ICondition
      {
         ICondition condition;
         if (conditions.TryGetValue(typeof(T), out condition))
         {
            return Some((T)condition);
         }
         return None;
      }
      public bool AddStatus(ICondition condition)
      {
         if (!conditions.ContainsKey(condition.GetType()))
         {
            conditions.Add(condition.GetType(), condition);
            return true;
         }
         return false;
      }
      public bool AddStatus<T>()
         where T : ICondition, new()
      {
         if (!conditions.ContainsKey(typeof(T)))
         {
            conditions.Add(typeof(T), new T());
            return true;
         }
         return false;
      }
      public bool RemoveStatus<T>()
         where T : ICondition
      {
         return conditions.Remove(typeof(T));
      }

      public bool RemoveStatus(ICondition condition)
      {
         return conditions.Remove(condition.GetType());
      }
   }
}
