using DraconicEngine.GameWorld.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.Storage
{
   public interface IBehaviorLibrary
   {
      Behavior Get(string name);
      bool Contains(string name);
      void Add(Behavior entity);

      Dictionary<string, Behavior> Templates { get; }
   }
}
