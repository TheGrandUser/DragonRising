using DraconicEngine.RulesSystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using DraconicEngine;
using DragonRising.Views;
using static LanguageExt.Prelude;
using DragonRising.GameWorld;

namespace DragonRising.Plans.Targeters
{
   public class EntityInRangeTargeter : IFromLocationTargeter, IToEntityTargeter
   {
      public ImmutableList<IFromLocationTargeter> Targeters { get; }
      public ImmutableList<IFromLocationQuery> Queries { get; }
      public ImmutableList<IEntityEffect> Effects { get; }

      public EntityInRangeTargeter(
        SelectionRange range,
        string messagePattern, RogueColor messageColor,
        IEnumerable<IFromLocationTargeter> targeters,
        IEnumerable<IFromLocationQuery> queries,
        IEnumerable<IEntityEffect> effects)
      {
         Range = range;
         MessagePattern = messagePattern;
         MessageColor = messageColor;
         this.Targeters = targeters.ToImmutableList();
         this.Queries = queries.ToImmutableList();
         this.Effects = effects.ToImmutableList();
      }
      
      public SelectionRange Range { get; }
      public string MessagePattern { get; }
      public RogueColor MessageColor { get; }

      public async Task<Option<TargetResult>> GetPlayerTargetingAsync(SceneView sceneView, Loc origin, ImmutableStack<Either<Loc, Vector>> path)
      {
         var area = Area.Combine(this.Queries.SelectMany(q => q.GetArea().AsEnumerable()));

         var location = await PlayerController.SelectTargetEntity(origin, Range, c => c != World.Current.Player, sceneView, area);

         return await location.Match(Some: async creature =>
         {
            var newPath = path.Push(origin);

            var childResults = await Targeter.HandleChildTargetersAsync(
               this.Targeters,
               t => t.GetPlayerTargetingAsync(sceneView, creature.Location, newPath));

            var result = childResults.Map(rs => (TargetResult)new EntityTargetResult(creature, this, rs));
            
            return result;
         },
         None: () => Task.FromResult<Option<TargetResult>>(None));
      }
   }

}
