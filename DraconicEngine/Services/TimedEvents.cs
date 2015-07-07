using DraconicEngine.RulesSystem;
using DraconicEngine.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.Services
{
   public interface ITimedEvents
   {
      void Tick();
      void AddTimer(TurnTimer timer);
      void AddFromNow(int duration, Fact gameEvent, params Fact[] additionalEvents);
   }

   public static class TimedEvents
   {
      static ITimedEvents currentService;

      public static ITimedEvents Current { get { return currentService; } }

      public static void SetTimedEvents(ITimedEvents timedEvents)
      {
         currentService = timedEvents;
      }
   }

   public class BasicTimedEvents : ITimedEvents
   {
      List<TurnTimer> timers = new List<TurnTimer>();

      public void AddFromNow(int duration, Fact effect, params Fact[] additionalEffects)
      {
         AddTimer(new TurnTimer(duration, effect, additionalEffects));
      }

      public void AddTimer(TurnTimer timer)
      {
         this.timers.Add(timer);
      }

      public void Tick()
      {
         foreach(var timer in timers)
         {
            if (timer.Tick())
            {
               foreach (var @event in timer.Events)
               {
                  TurnEvents.Current.Add(@event);
               }
            }
         }
      }
   }

   [Serializable]
   public sealed class TurnTimer
   {
      int duration;
      List<Fact> gameEvents;

      public TurnTimer(int duration, Fact gameEvent, params Fact[] additionalEvents)
      {
         this.duration = duration;
         this.gameEvents = additionalEvents.StartWith(gameEvent).ToList();
      }

      public bool Tick()
      {
         --duration;
         if (duration == 0)
         {
            return true;
         }
         return false;
      }
      public int Duration { get { return duration; } }
      public IEnumerable<Fact> Events { get { return gameEvents.AsEnumerable(); } }
   }
}
