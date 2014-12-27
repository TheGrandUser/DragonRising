using DraconicEngine.GameWorld.Effects;
using DraconicEngine.GameWorld.EntitySystem;
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
      void Add(int duration, IEffect effect, params IEffect[] additionalEffects);
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

      public void Add(int duration, IEffect effect, params IEffect[] additionalEffects)
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
               foreach (var effect in timer.Effects)
               {
                  TurnEffects.Current.Add(effect);
               }
            }
         }
      }
   }

   [Serializable]
   public sealed class TurnTimer
   {
      int duration;
      List<IEffect> effects;

      public TurnTimer(int duration, IEffect effect, params IEffect[] additionalEffects)
      {
         this.duration = duration;
         this.effects = additionalEffects.StartWith(effect).ToList();
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
      public IEnumerable<IEffect> Effects { get { return effects.AsEnumerable(); } }
   }
}
