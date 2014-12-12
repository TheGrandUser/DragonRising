using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Items;
using DraconicEngine.GameWorld.Actions;
using DraconicEngine.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.Behaviors;
using System.Diagnostics;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   public sealed class BehaviorComponent : Component
   {
      static readonly JustPassBehavior defaultBehavior = new JustPassBehavior();

      public BehaviorComponent()
      {
      }

      public BehaviorComponent(IBehavior initialBehavior)
      {
         this.behaviors.AddLast(initialBehavior);
      }

      public bool IsDirectlyControlled { get; set; }

      LinkedList<IBehavior> behaviors = new LinkedList<IBehavior>();

      public IBehavior CurrentBehavior => behaviors.Count > 0 ? behaviors.Last.Value : defaultBehavior;

      public void PushBehavior(IBehavior behavior)
      {
         this.behaviors.AddLast(behavior);
      }

      public IBehavior PopBehavior()
      {
         if (this.behaviors.Count > 0)
         {
            var current = CurrentBehavior;
            this.behaviors.RemoveLast();
            return current;
         }
         return null;
      }

      public IEnumerable<IBehavior> Behaviors => this.behaviors.AsEnumerable();

      public void ClearBehaviors()
      {
         this.behaviors.Clear();
      }

      public void RemoveBehavior(IBehavior behavior)
      {
         this.behaviors.Remove(behavior);
      }
   }

   public class BehaviorComponentTemplate : ComponentTemplate
   {
      Type behaviorType;

      public BehaviorComponentTemplate(Type behaviorType)
      {
         this.behaviorType = behaviorType;
      }

      public override Type ComponentType => typeof(BehaviorComponent);

      public override Component CreateComponent()
      {
         var behavior = (IBehavior)behaviorType.Assembly.CreateInstance(behaviorType.FullName);
         Debug.Assert(behavior != null, "No behavior!");
         return new BehaviorComponent(behavior);
      }
   }
}
