using DraconicEngine.EntitySystem;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;

namespace DraconicEngine.RulesSystem
{
   // The Order:
   // ITargeter
   // ITargetingRegion & TargetPreview
   // TargetResult

   public interface ITargeter
   {
      IEnumerable<ITargeter> Targeters { get; }
      IEnumerable<IQuery> Queries { get; }
      IEnumerable<IEffect> Effects { get; }

   }

   public static class Targeter
   {
      public static async Task<Option<ImmutableList<TargetResult>>> HandleChildTargetersAsync<T>(
         IEnumerable<T> targeters,
         Func<T, Task<Option<TargetResult>>> getResult)
         where T : ITargeter
      {
         Stack<T> targetStack = new Stack<T>(targeters.Reverse());
         Stack<Tuple<T, TargetResult>> results = new Stack<Tuple<T, TargetResult>>(targetStack.Count);

         while (targetStack.Count > 0)
         {
            var targeter = targetStack.Pop();

            var result = await getResult(targeter);

            if (!result.Match(
               Some: r =>
               {
                  results.Push(tuple(targeter, r));
                  return true;
               },
               None: () =>
               {
                  if (results.Count > 0)
                  {
                     var prior = results.Pop();
                     targetStack.Push(targeter);
                     targetStack.Push(prior.Item1);
                     return true;
                  }
                  else
                  {
                     return false;
                  }
               }))
            {
               return None;
            }
         }

         return results.Select(r => r.Item2).ToImmutableList();
      }
   }
   
   public interface ILocationBasedTargeter : ITargeter
   {
      Task<Option<TargetResult>> GetPlayerTargetingAsync(Loc origin, ImmutableStack<Either<Loc, Vector>> path);
   }

   public interface IDirectionBasedTargetter : ITargeter
   {
      Task<Option<TargetResult>> GetPlayerTargetingAsync(Vector direciton, Loc origin, ImmutableStack<Either<Loc, Vector>> path);
   }

   /// <summary>
   /// Describes the valid targets for the current targeter, either locations in an area, list of entities, or directions
   /// Would display as the range of choices where a fireball could be placed
   /// Origin is based on the prior TargetResult
   /// </summary>
   public interface ITargetingOptions
   {
   }

   public class AreaTargetingOptions
   {
      public Area Area { get; set; }
   }

   public class EntitiesTargetingOptions
   {
      public List<Entity> Entities { get; set; }
   }

   /// <summary>
   /// Summarizes all the selectors for a particular targeter
   /// Would display as the area of a fireball
   /// Origin is always 0, 0 local space
   /// </summary>
   public class TargetPreview
   {
      public Area EntitiesOnlyArea { get; set; }
      public Area Area { get; set; }
   }

   public class SelectionRange
   {
      public SelectionRange(int? range, RangeLimits limits = RangeLimits.None)
      {
         Range = range;
         Limits = limits;
      }

      public int? Range { get; set; }
      public RangeLimits Limits { get; set; }
   }

   [Flags]
   public enum RangeLimits
   {
      None,
      LineOfEffect,
      LineOfSight
   }

   public abstract class TargetResult
   {
      public ITargeter Targeter { get; }
      public ImmutableList<TargetResult> ChildResults { get; }

      protected TargetResult(ITargeter targeter, IEnumerable<TargetResult> childResults)
      {
         Targeter = targeter;
         ChildResults = childResults.ToImmutableList();
      }
   }

   public abstract class LocatableTargetResult : TargetResult
   {
      public LocatableTargetResult(ITargeter targeter, IEnumerable<TargetResult> childResults)
         : base(targeter, childResults)
      {

      }
      public abstract Loc Location { get; }
   }

   public class LocationTargetResult : LocatableTargetResult
   {
      public LocationTargetResult(Loc location, ITargeter targeter, IEnumerable<TargetResult> childResults)
         : base(targeter, childResults)
      {
         Location = location;
      }

      public override Loc Location { get; }
   }

   public class EntityTargetResult : LocatableTargetResult
   {
      public EntityTargetResult(Entity entity, ITargeter targeter, IEnumerable<TargetResult> childResults)
         : base(targeter, childResults)
      {
         Entity = entity;
      }

      public Entity Entity { get; }

      public override Loc Location => Entity.Location;
   }

   public class DirectionTargetResult : TargetResult
   {
      public DirectionTargetResult(Vector direction, Loc location, ITargeter targeter, IEnumerable<TargetResult> childResults)
         : base(targeter, childResults)
      {
         Direction = direction;
         Location = location;
      }

      public Loc Location { get; }
      public Vector Direction { get; }
   }


}
