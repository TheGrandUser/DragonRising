﻿using DraconicEngine.RulesSystem;
using DraconicEngine.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using DraconicEngine.EntitySystem;
using DraconicEngine;
using DragonRising.GameWorld.Behaviors;

namespace DragonRising.GameWorld.Components
{
   public sealed class BehaviorComponent : Component
   {
      static readonly JustPassBehavior defaultBehavior = new JustPassBehavior();
      LinkedList<Behavior> behaviors = new LinkedList<Behavior>();

      public BehaviorComponent()
      {
      }

      public BehaviorComponent(Behavior initialBehavior)
      {
         this.behaviors.AddLast(initialBehavior);
      }

      BehaviorComponent(BehaviorComponent original, bool fresh)
         : base(original, fresh)
      {
         this.behaviors = new LinkedList<Behavior>(original.behaviors.Select(b => b.Clone()));
      }

      protected override Component CloneCore(bool fresh)
      {
         return new BehaviorComponent(this, fresh);
      }

      [JsonIgnore]
      public Behavior CurrentBehavior => behaviors.Count > 0 ? behaviors.Last.Value : defaultBehavior;

      public void PushBehavior(Behavior behavior)
      {
         this.behaviors.AddLast(behavior);
      }

      public Behavior PopBehavior()
      {
         if (this.behaviors.Count > 0)
         {
            var current = CurrentBehavior;
            this.behaviors.RemoveLast();
            return current;
         }
         return null;
      }

      public IEnumerable<Behavior> Behaviors => this.behaviors.AsEnumerable();

      public void ClearBehaviors()
      {
         this.behaviors.Clear();
      }

      public void RemoveBehavior(Behavior behavior)
      {
         this.behaviors.Remove(behavior);
      }

      public void RemoveBehavior(string name)
      {
         var behavior = behaviors.FirstOrDefault(b => b.Name == name);
         if (behavior != null)
         {
            behaviors.Remove(behavior);
         }
      }
   }
}
