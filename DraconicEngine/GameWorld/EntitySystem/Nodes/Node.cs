using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem
{
   public abstract class Node
   {
      Dictionary<string, Component> components = new Dictionary<string, Component>();

      public Entity Entity { get; set; }

      public abstract void SetComponents(Entity entity);
      public abstract void ClearComponents();
   }
}
