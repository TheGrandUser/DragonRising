using DraconicEngine.EntitySystem;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace DraconicEngine.RulesSystem
{
   // The Order:
   // ITargeter
   // ITargetingRegion & TargetPreview
   // TargetResult
   
   public static class Targeter
   {
      public static async Task<Option<ImmutableList<TargetResult>>> HandleChildTargetersAsync<T>(
         IEnumerable<T> targeters,
         Func<T, Task<Option<TargetResult>>> getResult)
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
                  results.Push(Tuple(targeter, r));
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

   public interface IFromLocationTargeter
   {
      Task<Option<TargetResult>> GetPlayerTargetingAsync(SceneView sceneView, Loc origin, ImmutableStack<Either<Loc, Vector>> path);
   }

   public interface IToLocationTargeter
   {
      ImmutableList<IFromLocationTargeter> Targeters { get; }
      ImmutableList<IFromLocationQuery> Queries { get; }
      ImmutableList<ILocationEffect> Effects { get; }
   }

   public interface IFromDirectionTargetter
   {
      Task<Option<TargetResult>> GetPlayerTargetingAsync(Vector direciton, Loc origin, ImmutableStack<Either<Loc, Vector>> path);
   }

   public interface IToDirectionTargeter
   {
      ImmutableList<IFromDirectionTargetter> Targeters { get; }
      ImmutableList<IAreaFromDirectionQuery> Queries { get; }
   }
   public interface IToEntityTargeter
   {
      ImmutableList<IFromLocationTargeter> Targeters { get; }
      ImmutableList<IFromLocationQuery> Queries { get; }
      ImmutableList<IEntityEffect> Effects { get; }
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
      public ImmutableList<TargetResult> ChildResults { get; }

      protected TargetResult(IEnumerable<TargetResult> childResults)
      {
         ChildResults = childResults.ToImmutableList();
      }

      public abstract IEnumerable<Fact> GetFacts(Entity initiatior, Scene context);
   }
   
   public class LocationTargetResult : TargetResult
   {
      private Loc location;
      IToLocationTargeter targeter;

      public LocationTargetResult(
         Loc location,
         IToLocationTargeter targeter,
         IEnumerable<TargetResult> childResults)
         : base(childResults)
      {
         this.targeter = targeter;
         this.location = location;
      }
      
      public override IEnumerable<Fact> GetFacts(Entity initiatior, Scene context)
      {
         var targeterFacts = ChildResults.SelectMany(r => r.GetFacts(initiatior, context));
         var queryFacts = targeter.Queries.SelectMany(q => q.GetFacts(initiatior, location, context));
         var effectFacts = targeter.Effects.SelectMany(e => e.GetFacts(initiatior, location, context));

         return targeterFacts.Concat(queryFacts).Concat(effectFacts);
      }
   }

   public class EntityTargetResult : TargetResult
   {
      private Entity entity;
      private IToEntityTargeter targeter;

      public EntityTargetResult(
         Entity entity,
         IToEntityTargeter targeter,
         IEnumerable<TargetResult> childResults)
         : base(childResults)
      {
         this.targeter = targeter;
         this.entity = entity;
      }
      
      public override IEnumerable<Fact> GetFacts(Entity initiatior, Scene context)
      {
         var targeterFacts = ChildResults.SelectMany(r => r.GetFacts(initiatior, context));
         var queryFacts = targeter.Queries.SelectMany(q => q.GetFacts(initiatior, entity.Location, context));
         var effectFacts = targeter.Effects.SelectMany(e => e.GetFacts(initiatior, entity, context));

         return targeterFacts.Concat(queryFacts).Concat(effectFacts);
      }
   }

   public class DirectionTargetResult : TargetResult
   {
      private Vector direction;
      private Loc location;
      private IToDirectionTargeter targeter;

      public DirectionTargetResult(Vector direction, Loc location, 
         IToDirectionTargeter targeter,
         IEnumerable<TargetResult> childResults)
         : base(childResults)
      {
         this.targeter = targeter;
         this.direction = direction;
         this.location = location;
      }
      
      public override IEnumerable<Fact> GetFacts(Entity initiatior, Scene context)
      {
         var targeterFacts = ChildResults.SelectMany(r => r.GetFacts(initiatior, context));
         var queryFacts = targeter.Queries.SelectMany(q => q.GetFacts(initiatior, direction, location, context));

         return targeterFacts.Concat(queryFacts);
      }
   }
}
