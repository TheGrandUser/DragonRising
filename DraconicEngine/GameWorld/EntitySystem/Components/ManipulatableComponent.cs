using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Items;

namespace DraconicEngine.GameWorld.EntitySystem.Components
{
   class ManipulatableComponent : Component
   {
      public bool RequiresItem { get; set; }

      public bool Use(Item itemToUse)
      {
         return false;
      }
   }
}