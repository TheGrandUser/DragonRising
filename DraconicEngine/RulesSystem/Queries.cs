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
   public interface IQuery
   {
      IEnumerable<IQuery> Queries { get; }
      IEnumerable<IEffect> Effects { get; }
   }

   public interface IDirectionBasedAreaSelector : IQuery
   {
      Option<Area> GetArea();
      IEnumerable<Fact> GetFacts(Entity user, Direction dir, Loc loc);
   }

   public interface ILocationBasedQuery : IQuery
   {
      Option<Area> GetArea();
      IEnumerable<Fact> GetFacts(Entity user, Loc loc);
   }

   public interface IEntityFilter
   {
      bool Matches(Entity user, Entity target);
   }
}
