using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.GameWorld.Actions;

namespace DragonRising.Services
{
   public class RulesManager : IRulesManager
   {
      public void PerformAction(RogueAction action)
      {
         throw new NotImplementedException();
      }
   }

   public interface IRulesManager
   {
      void PerformAction(RogueAction action);
   }
}
