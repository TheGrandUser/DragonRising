using DraconicEngine;
using DraconicEngine.EntitySystem;
using DragonRising.GameWorld;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Events;
using DragonRising.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.RulesSystem;

namespace DragonRising.Rules.GameFlowRules
{
   class GameEndsOnPlayerDeathRule : Rule<CreatureKilledEvent>
   {
      public override RuleResult Do(CreatureKilledEvent args)
      {
         MessageService.Current.PostMessage("You have died!", RogueColors.Red);

         var player = args.CreatureKilled;
         
         player.As<DrawnComponent>(dc => dc.SeenCharacter = new Character(Glyph.Percent, RogueColors.DarkRed));
         player.Blocks = false;
         player.As<BehaviorComponent>(bc => bc.ClearBehaviors());

         MyPlayingScreen.Current.GameHasEnded(GameEnd.Failure);

         return RuleResult.Empty;
      }
      
      protected override bool Filter(CreatureKilledEvent args)
      {
         var entity = args.CreatureKilled;
         return entity == World.Current.Player;
      }

      public override bool UseFilter => true;
      public override int Priority => 1;
   }

   public enum GameEnd
   {
      Victory,
      Failure,
   }
}
