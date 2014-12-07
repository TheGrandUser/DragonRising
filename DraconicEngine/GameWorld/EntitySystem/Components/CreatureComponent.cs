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
   }
}
