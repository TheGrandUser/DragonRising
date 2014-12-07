using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Items;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   class ManipulatableComponent : IComponent
   {
      public Entity Owner { get; set; }
      public bool RequiresItem { get; set; }

      public bool Use(Item itemToUse)
      {
         throw new NotImplementedException();
      }
   }
}