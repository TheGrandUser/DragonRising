using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.EntitySystem
{
   public abstract class GameSystem
   {
      internal int priority = 0;

      public virtual void AddToEngine(EntityEngine engine)
      {

      }

      public virtual void RemoveFromEngine(EntityEngine engine)
      {

      }

   }

   public abstract class GameSystemSync : GameSystem
   {
      public virtual void Update(double time)
      {

      }
   }

   //public abstract class GameSystemAsync : GameSystem
   //{
   //   public virtual Task Update(double time)
   //   {
   //      return Task.FromResult(0);
   //   }
   //}
}
