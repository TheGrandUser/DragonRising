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
      public static async Task<Option<ImmutableList<TargetResult<TContext>>>> HandleChildTargetersAsync<T, TContext>(
         IEnumerable<T> targeters,
         Func<T, Task<Option<TargetResult<TContext>>>> getResult)
      {
         Stack<T> targetStack = new Stack<T>(targeters.Reverse());
         Stack<Tuple<T, TargetResult<TContext>>> results = new Stack<Tuple<T, TargetResult<TContext>>>(targetStack.Count);

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

   public interface IFromLocationTargeter<TContext>
   {
      Task<Option<TargetResult<TContext>>> GetPlayerTargetingAsync(SceneView sceneView, Loc origin, ImmutableStack<Either<Loc, Vector>> path);
   }

   public interface IToLocationTargeter<TContext>
   {
      ImmutableList<IFromLocationTargeter<TContext>> Targeters { get; }
      ImmutableList<IFromLocationQuery<TContext>> Queries { get; }
      ImmutableList<ILocationEffect<TContext>> Effects { get; }
   }

   public interface IFromDirectionTargetter<TContext>
   {
      Task<Option<TargetResult<TContext>>> GetPlayerTargetingAsync(Vector direciton, Loc origin, ImmutableStack<Either<Loc, Vector>> path);
   }

   public interface IToDirectionTargeter<TContext>
   {
      ImmutableList<IFromDirectionTargetter<TContext>> Targeters { get; }
      ImmutableList<IAreaFromDirectionQuery<TContext>> Queries { get; }
   }
   public interface IToEntityTargeter<TContext>
   {
      ImmutableList<IFromLocationTargeter<TContext>> Targeters { get; }
      ImmutableList<IFromLocationQuery<TContext>> Queries { get; }
      ImmutableList<IEntityEffect<TContext>> Effects { get; }
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

   public abstract class TargetResult<TContext>
   {
      public ImmutableList<TargetResult<TContext>> ChildResults { get; }

      protected TargetResult(IEnumerable<TargetResult<TContext>> childResults)
      {
         ChildResults = childResults.ToImmutableList();
      }

      public abstract IEnumerable<Fact> GetFacts(Entity initiatior, TContext context);
   }
   
   public class LocationTargetResult<TContext> : TargetResult<TContext>
   {
      private Loc location;
      IToLocationTargeter<TContext> targeter;

      public LocationTargetResult(
         Loc location,
         IToLocationTargeter<TContext> targeter,
         IEnumerable<TargetResult<TContext>> childResults)
         : base(childResults)
      {
         this.targeter = targeter;
         this.location = location;
      }
      
      public override IEnumerable<Fact> GetFacts(Entity initiatior, TContext context)
      {
         var targeterFacts = ChildResults.SelectMany(r => r.GetFacts(initiatior, context));
         var queryFacts = targeter.Queries.SelectMany(q => q.GetFacts(initiatior, location, context));
         var effectFacts = targeter.Effects.SelectMany(e => e.GetFacts(initiatior, location, context));

         return targeterFacts.Concat(queryFacts).Concat(effectFacts);
      }
   }

   public class EntityTargetResult<TContext> : TargetResult<TContext>
   {
      private Entity entity;
      private IToEntityTargeter<TContext> targeter;

      public EntityTargetResult(
         Entity entity,
         IToEntityTargeter<TContext> targeter,
         IEnumerable<TargetResult<TContext>> childResults)
         : base(childResults)
      {
         this.targeter = targeter;
         this.entity = entity;
      }
      
      public override IEnumerable<Fact> GetFacts(Entity initiatior, TContext context)
      {
         var targeterFacts = ChildResults.SelectMany(r => r.GetFacts(initiatior, context));
         var queryFacts = targeter.Queries.SelectMany(q => q.GetFacts(initiatior, entity.Location, context));
         var effectFacts = targeter.Effects.SelectMany(e => e.GetFacts(initiatior, entity, context));

         return targeterFacts.Concat(queryFacts).Concat(effectFacts);
      }
   }

   public class DirectionTargetResult<TContext> : TargetResult<TContext>
   {
      private Vector direction;
      private Loc location;
      private IToDirectionTargeter<TContext> targeter;

      public DirectionTargetResult(Vector direction, Loc location, 
         IToDirectionTargeter<TContext> targeter,
         IEnumerable<TargetResult<TContext>> childResults)
         : base(childResults)
      {
         this.targeter = targeter;
         this.direction = direction;
         this.location = location;
      }
      
      public override IEnumerable<Fact> GetFacts(Entity initiatior, TContext context)
      {
         var targeterFacts = ChildResults.SelectMany(r => r.GetFacts(initiatior, context));
         var queryFacts = targeter.Queries.SelectMany(q => q.GetFacts(initiatior, direction, location, context));

         return targeterFacts.Concat(queryFacts);
      }
   }
}
