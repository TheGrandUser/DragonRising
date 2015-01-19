using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using Microsoft.Practices.Prism.PubSubEvents;
using DragonRising.GameWorld.Events;
using DragonRising.GameWorld;
using DragonRising.GameWorld.Components;
using DraconicEngine.GameWorld.EntitySystem;
using DragonRising.GameStates;
using DraconicEngine.GameWorld;
using DraconicEngine.GameStates;
using LanguageExt;
using LanguageExt.Prelude;

namespace DragonRising
{
   class StatTracker
   {
      SubscriptionToken subscription;

      int levelUpBase = 200;
      int levelUpFactor = 150;

      int monstersKilled = 0;
      public int MonstersKilled { get { return monstersKilled; } }

      public StatTracker(IEventAggregator eventAggregator)
      {
         var @event = eventAggregator.GetEvent<CreatureKilledEvent>();
         this.subscription = @event.Subscribe(OnCreatureKilled);
      }

      private void OnCreatureKilled(CreatureKilledEventArgs args)
      {
         if (args.Cause == "Entity" && args.KillingEntity == World.Current.Player)
         {
            monstersKilled++;

            var targetCombat = args.CreatureKilled.GetComponent<CombatantComponent>();
            var playerCombat = args.KillingEntity.GetComponent<CombatantComponent>();
            playerCombat.XP += targetCombat.XP;

            var playerLevel = args.KillingEntity.GetComponent<LevelComponent>();

            var levelUpXp = levelUpBase + levelUpFactor * playerLevel.Level;
            if (playerCombat.XP >= levelUpXp)
            {
               playerLevel.Level += 1;
               playerCombat.XP -= levelUpXp;

               MessageService.Current.PostMessage("Your battle skills grow stronger! You reached level " + playerLevel.Level, RogueColors.Yellow);

               MyPlayingState.Current.AddAsyncInterruption(
                  ChooseLevelUpBenefit,
                  StillApplies);
            }
         }
      }

      bool StillApplies()
      {
         return World.Current.Player.GetComponentOrDefault<CombatantComponent>()?.IsAlive ?? false;
      }

      async Task ChooseLevelUpBenefit()
      {
         var selectBenefitScreen = new SelectBenefitScreen();

         await RogueGame.Current.RunGameState(selectBenefitScreen);


      }
   }
}
