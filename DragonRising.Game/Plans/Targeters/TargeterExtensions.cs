using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILocationBasedTargeter = DraconicEngine.RulesSystem.IFromLocationTargeter<DragonRising.Scene>;
using ILocationBasedQuery = DraconicEngine.RulesSystem.IFromLocationQuery<DragonRising.Scene>;
using TargetResult = DraconicEngine.RulesSystem.TargetResult<DragonRising.Scene>;
using LocationTargetResult = DraconicEngine.RulesSystem.LocationTargetResult<DragonRising.Scene>;
using EntityTargetResult = DraconicEngine.RulesSystem.EntityTargetResult<DragonRising.Scene>;
using DirectionTargetResult = DraconicEngine.RulesSystem.DirectionTargetResult<DragonRising.Scene>;

namespace DragonRising.Plans.Targeters
{
   //static class TargeterExtensions
   //{
   //   public static void Match(this ITargeter self,
   //      Action<LocationInRangeTargeter> point,
   //      Action<EntityInRangeTargeter> entity,
   //      Action<DirectionTargeter> direction)
   //   {
   //      if (self is LocationInRangeTargeter)
   //      {
   //         point((LocationInRangeTargeter)self);
   //      }
   //      else if (self is EntityInRangeTargeter)
   //      {
   //         entity((EntityInRangeTargeter)self);
   //      }
   //      else if (self is DirectionTargeter)
   //      {
   //         direction((DirectionTargeter)self);
   //      }
   //      else { throw new ArgumentException("Unknown targeter"); }
   //   }

   //   public static T Match<T>(this ITargeter self,
   //      Func<LocationInRangeTargeter, T> point,
   //      Func<EntityInRangeTargeter, T> entity,
   //      Func<DirectionTargeter, T> direction)
   //   {
   //      if (self is LocationInRangeTargeter)
   //      {
   //         return point((LocationInRangeTargeter)self);
   //      }
   //      else if (self is EntityInRangeTargeter)
   //      {
   //         return entity((EntityInRangeTargeter)self);
   //      }
   //      else if (self is DirectionTargeter)
   //      {
   //         return direction((DirectionTargeter)self);
   //      }
   //      else { throw new ArgumentException("Unknown targeter"); }
   //   }

   //   public static void Match(this TargetResult self,
   //      Action<LocationTargetResult> loc,
   //      Action<EntityTargetResult> entity,
   //      Action<DirectionTargetResult> dir)
   //   {
   //      if (self is LocationTargetResult)
   //      {
   //         loc((LocationTargetResult)self);
   //      }
   //      else if (self is EntityTargetResult)
   //      {
   //         entity((EntityTargetResult)self);
   //      }
   //      else if (self is DirectionTargetResult)
   //      {
   //         dir((DirectionTargetResult)self);
   //      }
   //      else
   //      {
   //         throw new ArgumentException("Unknown results type");
   //      }
   //   }
   //   public static T Match<T>(this TargetResult self,
   //      Func<LocationTargetResult, T> loc,
   //      Func<EntityTargetResult, T> entity,
   //      Func<DirectionTargetResult, T> dir)
   //   {
   //      if (self is LocationTargetResult)
   //      {
   //         return loc((LocationTargetResult)self);
   //      }
   //      else if (self is EntityTargetResult)
   //      {
   //         return entity((EntityTargetResult)self);
   //      }
   //      else if (self is DirectionTargetResult)
   //      {
   //         return dir((DirectionTargetResult)self);
   //      }
   //      else
   //      {
   //         throw new ArgumentException("Unknown results type");
   //      }
   //   }


   //   public static void Match(this TargetResult self,
   //      Action<LocatableTargetResult> loc,
   //      Action<DirectionTargetResult> dir)
   //   {
   //      if (self is LocatableTargetResult)
   //      {
   //         loc((LocatableTargetResult)self);
   //      }
   //      else
   //      {
   //         Debug.Assert(self is DirectionTargetResult);
   //         dir((DirectionTargetResult)self);
   //      }
   //   }
   //   public static T Match<T>(this TargetResult self,
   //      Func<LocatableTargetResult, T> loc,
   //      Func<DirectionTargetResult, T> dir)
   //   {
   //      if (self is LocatableTargetResult)
   //      {
   //         return loc((LocatableTargetResult)self);
   //      }
   //      else if (self is DirectionTargetResult)
   //      {
   //         return dir((DirectionTargetResult)self);
   //      }
   //      else
   //      {
   //         throw new ArgumentException("Unknown results type");
   //      }
   //   }
   //}
}
