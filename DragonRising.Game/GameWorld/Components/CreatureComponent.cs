using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using DraconicEngine;
using DragonRising.GameWorld.Alligences;

namespace DragonRising.GameWorld.Components
{
   public sealed class CreatureComponent : Component
   {
      public int VisionRadius { get; set; }
      public Alligence Alligence { get; set; } = Alligence.Neutral;

      public CreatureComponent()
      {
      }

      public CreatureComponent(int visionRadius)
      {
         this.VisionRadius = visionRadius;
      }

      public CreatureComponent(Alligence alligence, int visionRadius)
      {
         this.Alligence = alligence;
         this.VisionRadius = visionRadius;
      }

      CreatureComponent(CreatureComponent original, bool fresh)
         : base(original, fresh)
      {
         this.Alligence = original.Alligence;
         this.VisionRadius = original.VisionRadius;
      }

      protected override Component CloneCore(bool fresh)
      {
         return new CreatureComponent(this, fresh);
      }

      protected override void OnOwnerChanged(Entity oldOwner, Entity newOwner)
      {
         // Get Stats
      }
   }
}
