using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.EntitySystem
{
   public abstract class Node
   {
      Dictionary<string, Component> components = new Dictionary<string, Component>();

      public Entity Entity { get; set; }
   }
}
