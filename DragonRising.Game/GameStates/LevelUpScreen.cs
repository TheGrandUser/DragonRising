﻿using DraconicEngine.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using DraconicEngine.Terminals;
using DragonRising.GameWorld;
using DragonRising.GameWorld.Components;
using DraconicEngine.GameWorld.EntitySystem;
using DragonRising.Widgets;
using DraconicEngine;

namespace DragonRising.GameStates
{
   public class LevelUpScreen : GameState
   {
      MenuWidget<Benefit> menu;

      public LevelUpScreen()
      {
         var combatant = World.Current.Player.GetComponent<CombatantComponent>();
         
         this.menu = new MenuWidget<Benefit>(DragonRisingGame.Current.RootTerminal[10, 5], "'Level up! Choose a stat to raise:", false,
            new MenuItem<Benefit>($"Constitution (+20 hp from {combatant.MaxHP})", Benefit.Constitution),
            new MenuItem<Benefit>($"Strength (+1 attack from {combatant.Power})", Benefit.Strength),
            new MenuItem<Benefit>($"Agility (+1 defense from {combatant.Defense})", Benefit.Agility));
      }
      
      public override GameStateType Type => GameStateType.Screen;

      public Benefit Benefit { get; private set; }

      public override Task Draw()
      {
         RogueGame.Current.RootTerminal.Clear();

         this.menu.Draw();

         return Task.FromResult(0);
      }
      
      public override async Task<TickResult> Tick()
      {
         var result = await this.menu.Tick();

         return result.Match(
            Some: benefit =>
            {
               this.Benefit = benefit;
               return TickResult.Finished;
            },
            None: () => TickResult.Continue);
      }
   }

   public enum Benefit
   {
      Constitution,
      Strength,
      Agility,
   }
}