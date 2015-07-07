using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Services
{
   public interface ITurnEvents
   {
      void Add(Fact effect);

      void DoAllEvents();
   }

   public class TurnEvents
   {
      static ITurnEvents currentService;

      public static ITurnEvents Current { get { return currentService; } }

      public static void SetTurnEffects(ITurnEvents turnEffects)
      {
         currentService = turnEffects;
      }
   }
}
