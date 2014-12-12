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
      public CreatureComponentTemplate Template { get; set; }
      public int VisionRadius => Template.VisionRadius;
      public Some<Alligence> Alligence { get; set; } = AlligenceManager.Current.Neutral;

      public CreatureComponent(CreatureComponentTemplate template)
      {
         this.Template = template;
         this.Alligence = template.Alligence;
      }
   }

   public class CreatureComponentTemplate : ComponentTemplate
   {
      public int VisionRadius { get; set; }
      public Some<Alligence> Alligence { get; set; } = AlligenceManager.Current.Neutral;

      public CreatureComponentTemplate(Some<Alligence> alligence)
      {
         this.Alligence = alligence;
      }

      public override Type ComponentType=> typeof(CreatureComponent);

      public override Component CreateComponent() => new CreatureComponent(this);
   }
}
