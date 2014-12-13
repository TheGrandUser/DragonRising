using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameWorld.Actions
{
   public interface IActionResolver
   {
      Type ActionType { get; }
      Type ResultType { get; }
   }

   public class DefaultResolverAttribute : Attribute
   {
      public Type ActionType { get; }
      public DefaultResolverAttribute(Type actionType)
      {
         this.ActionType = actionType;
      }
   }
}
