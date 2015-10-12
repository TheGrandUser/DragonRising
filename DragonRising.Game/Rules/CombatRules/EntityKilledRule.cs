using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.EntitySystem;
using LanguageExt;
using static LanguageExt.Prelude;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Events;
using DragonRising.GameWorld;
using DragonRising.GameWorld.Components;
using DraconicEngine;
using DragonRising.Views;
using DraconicEngine.GameViews;

namespace DragonRising.Rules.CombatRules
{
   class EntityKilledRule : Rule<CreatureKilledEvent>
   {
      World world;
      MyPlayingScreen screen;

      public EntityKilledRule(World world, MyPlayingScreen screen)
      {
         this.world = world;
         this.screen = screen;
      }

      public int MonstersKilled { get; private set; }

      public override RuleResult Do(CreatureKilledEvent gameEvent, Scene scene)
      {
         gameEvent.CreatureKilled.As<DrawnComponent>(dc => dc.SeenCharacter = new Character(Glyph.Percent, RogueColors.DarkRed));
         gameEvent.CreatureKilled.Blocks = false;
         gameEvent.CreatureKilled.As<BehaviorComponent>(bc => bc.ClearBehaviors());
         gameEvent.CreatureKilled.As<CombatantComponent>(cc => cc.IsAlive = false);

         if (gameEvent.CreatureKilled == this.world.Player)
         {
            MessageService.Current.PostMessage("You have died!", RogueColors.Red);
         }

         if (gameEvent.KillingEntity.Match(killer => killer == world.Player, () => false))
         {
            var player = world.Player;

            MonstersKilled++;

            var monster = gameEvent.CreatureKilled;

            var monsterXP = monster.GetXP();
            var playerXP = player.GetXP();

            var targetCombat = monster.GetComponent<CombatantComponent>();
            var playerCombat = player.GetComponent<CombatantComponent>();
            playerXP.Value += monsterXP;

            var playerLevel = player.GetLevel();

            MessageService.Current.PostMessage($"{monster.Name} is dead! You gain {monsterXP} experience points", RogueColors.Orange);
            gameEvent.CreatureKilled.Name = "remains of " + gameEvent.CreatureKilled.Name;

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

         return RuleResult.Empty;
      }


      bool ChooseLevelUpBenefitStillApplies()
      {
         return World.Current.Player.GetComponentOrDefault<CombatantComponent>()?.IsAlive ?? false;
      }

      async Task ChooseLevelUpBenefit()
      {
         var selectBenefitScreen = new LevelUpScreen();

         var benefit = await RogueGame.Current.RunGameState(selectBenefitScreen);

         var combatantComponent = World.Current.Player.GetComponent<CombatantComponent>();

         switch (benefit)
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
   }
}
