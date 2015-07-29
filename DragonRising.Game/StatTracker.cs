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
using DraconicEngine.EntitySystem;
using DragonRising.Views;
using DraconicEngine.GameViews;
using LanguageExt;
using static LanguageExt.Prelude;

namespace DragonRising
{
   public sealed class StatTracker : IDisposable
   {
      SubscriptionToken subscription;
      MyPlayingScreen screen;

      int monstersKilled = 0;
      public int MonstersKilled { get { return monstersKilled; } }

      public StatTracker(MyPlayingScreen screen, IEventAggregator eventAggregator)
      {
         this.screen = screen;
         var @event = eventAggregator.GetEvent<CreatureKilledPubSubEvent>();
         this.subscription = @event.Subscribe(OnCreatureKilled);
      }

      private void OnCreatureKilled(CreatureKilledEvent args)
      {
         args.KillingEntity.IfSome(player =>
         {
            if (player == World.Current.Player)
            {
               monstersKilled++;

               var monster = args.CreatureKilled;

               var monsterXP = monster.GetXP();
               var playerXP = player.GetXP();

               var targetCombat = monster.GetComponent<CombatantComponent>();
               var playerCombat = player.GetComponent<CombatantComponent>();
               playerXP.Value += monsterXP;

               var playerLevel = player.GetLevel();

               var levelUpXp = LevelingPolicy.XpForNextLevel(playerLevel.Value);
               if (playerXP >= levelUpXp)
               {
                  playerLevel.Value += 1;
                  playerXP.Value -= levelUpXp;


                  MessageService.Current.PostMessage("Your battle skills grow stronger! You reached level " + playerLevel, RogueColors.Yellow);

                  screen.AddAsyncInterruption(
                     ChooseLevelUpBenefit,
                     ChooseLevelUpBenefitStillApplies);
               }
            }
         });
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
