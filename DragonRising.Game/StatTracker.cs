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
using DraconicEngine.GameViews;
using LanguageExt;
using static LanguageExt.Prelude;

namespace DragonRising
{
   public sealed class StatTracker : IDisposable
   {
      SubscriptionToken subscription;
      
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

            var levelUpXp = LevelingPolicy.XpForNextLevel(playerLevel.Level);
            if (playerCombat.XP >= levelUpXp)
            {
               playerLevel.Level += 1;
               playerCombat.XP -= levelUpXp;
               

               MessageService.Current.PostMessage("Your battle skills grow stronger! You reached level " + playerLevel.Level, RogueColors.Yellow);

               MyPlayingScreen.Current.AddAsyncInterruption(
                  ChooseLevelUpBenefit,
                  ChooseLevelUpBenefitStillApplies);
            }
         }
      }

      bool ChooseLevelUpBenefitStillApplies()
      {
         return World.Current.Player.GetComponentOrDefault<CombatantComponent>()?.IsAlive ?? false;
      }

      async Task ChooseLevelUpBenefit()
      {
         var selectBenefitScreen = new LevelUpScreen();

         await RogueGame.Current.RunGameState(selectBenefitScreen);

         var combatantComponent = World.Current.Player.GetComponent<CombatantComponent>();

         switch (selectBenefitScreen.Benefit)
         {
            case Benefit.Constitution:
               combatantComponent.MaxHP += 20;
               break;
            case Benefit.Strength:
               combatantComponent.Power += 1;
               break;
            case Benefit.Agility:
               combatantComponent.Defense += 1;
               break;
         }

         combatantComponent.Heal(combatantComponent.MaxHP);
      }

      public void Dispose()
      {
         this.subscription.Dispose();
      }
   }
}
