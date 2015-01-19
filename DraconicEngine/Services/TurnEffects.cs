using DraconicEngine.GameWorld.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Services
{
   public interface ITurnEffects
   {
      void Add(IEffect effect);

      void DoAllEffects();
   }

   public class TurnEffects
   {
      static ITurnEffects currentService;

      public static ITurnEffects Current { get { return currentService; } }

      public static void SetTurnEffects(ITurnEffects turnEffects)
      {
         currentService = turnEffects;
      }
   }
}
