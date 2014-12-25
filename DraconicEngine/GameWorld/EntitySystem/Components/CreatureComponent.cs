using DraconicEngine.GameWorld.Alligences;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   public sealed class CreatureComponent : Component
   {
      public int VisionRadius { get; set; }
      public Some<Alligence> Alligence { get; set; } = AlligenceManager.Current.Neutral;

      public CreatureComponent()
      {
      }

      public CreatureComponent(Some<Alligence> alligence, int visionRadius)
      {
         this.Alligence = alligence;
         this.VisionRadius = visionRadius;
      }

      protected CreatureComponent(CreatureComponent original, bool fresh)
         : base(original, fresh)
      {
         this.Alligence = original.Alligence;
         this.VisionRadius = original.VisionRadius;
      }

      protected override Component CloneCore(bool fresh)
      {
         return new CreatureComponent(this, fresh);
      }
   }
}
