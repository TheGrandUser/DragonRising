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
using IFromLocationTargeter = DraconicEngine.RulesSystem.IFromLocationTargeter<DragonRising.Scene>;
using IFromLocationQuery = DraconicEngine.RulesSystem.IFromLocationQuery<DragonRising.Scene>;
using TargetResult = DraconicEngine.RulesSystem.TargetResult<DragonRising.Scene>;
using LocationTargetResult = DraconicEngine.RulesSystem.LocationTargetResult<DragonRising.Scene>;
using ILocationEffect = DraconicEngine.RulesSystem.ILocationEffect<DragonRising.Scene>;
using IEntityEffect = DraconicEngine.RulesSystem.IEntityEffect<DragonRising.Scene>;

namespace DragonRising.Plans.Targeters
{
   public class EntityInRangeTargeter : IFromLocationTargeter<Scene>, IToEntityTargeter<Scene>
   {
      public ImmutableList<IFromLocationTargeter> Targeters { get; }
      public ImmutableList<IFromLocationQuery> Queries { get; }
      public ImmutableList<IEntityEffect> Effects { get; }

      public EntityInRangeTargeter(
        SelectionRange range,
        string messagePattern, RogueColor messageColor,
        IEnumerable<IFromLocationTargeter<Scene>> targeters,
        IEnumerable<IFromLocationQuery<Scene>> queries,
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

      public async Task<Option<TargetResult<Scene>>> GetPlayerTargetingAsync(SceneView sceneView, Loc origin, ImmutableStack<Either<Loc, Vector>> path)
      {
         var area = Area.Combine(this.Queries.SelectMany(q => q.GetArea().AsEnumerable()));

         var location = await PlayerController.SelectTargetEntity(origin, Range, c => c != World.Current.Player, sceneView, area);

         return await location.Match(Some: async creature =>
         {
            var newPath = path.Push(origin);

            var childResults = await Targeter.HandleChildTargetersAsync(
               this.Targeters,
               t => t.GetPlayerTargetingAsync(sceneView, creature.Location, newPath));

            var result = childResults.Map(rs => (TargetResult<Scene>)new EntityTargetResult<Scene>(creature, this, rs));
            
            return result;
         },
         None: () => Task.FromResult<Option<TargetResult<Scene>>>(None));
      }
   }

}
