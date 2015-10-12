using DraconicEngine.EntitySystem;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.RulesSystem
{
   public interface IAreaFromDirectionQuery<TContext>
   {
      Option<Area> GetArea();
      IEnumerable<Fact> GetFacts(Entity user, Vector dir, Loc loc, TContext context);
   }

   public interface IFromLocationQuery<TContext>
   {
      Option<Area> GetArea();
      IEnumerable<Fact> GetFacts(Entity user, Loc loc, TContext context);
   }

   public interface IEntityFilter
   {
      bool Matches(Entity user, Entity target);
   }
}
