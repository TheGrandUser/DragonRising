using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.EntitySystem.Nodes
{
   [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
   public class DoesNotHaveAttribute : Attribute
   {
      public Type ComponentType { get; }
      public DoesNotHaveAttribute(Type componentType)
      {
         ComponentType = componentType;
      }
   }
}
