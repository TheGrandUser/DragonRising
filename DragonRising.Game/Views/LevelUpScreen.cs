using DraconicEngine.GameViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using DraconicEngine.Terminals;
using DragonRising.GameWorld;
using DragonRising.GameWorld.Components;
using DraconicEngine.EntitySystem;
using DragonRising.Widgets;
using DraconicEngine;

namespace DragonRising.Views
{
   public class LevelUpScreen : IGameView<Benefit>
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
      
      public GameViewType Type => GameViewType.WholeScreen;
      
      public void Draw()
      {
         this.menu.Draw();
      }
      
      public Task<Benefit> DoLogic()
      {
         return this.menu.Tick();
      }
   }

   public enum Benefit
   {
      Constitution,
      Strength,
      Agility,
   }
}
