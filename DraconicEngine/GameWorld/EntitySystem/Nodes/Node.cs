using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem
{
   public abstract class Node
   {
      Dictionary<string, IComponent> components = new Dictionary<string, IComponent>();

      public Entity Entity { get; set; }

      public abstract void SetComponents(Entity entity);
      public abstract void ClearComponents();
   }
}
