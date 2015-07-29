using DraconicEngine;
using DraconicEngine.GameViews;
using DragonRising.GameWorld.Alligences;
using DraconicEngine.EntitySystem;
using DraconicEngine.Input;
using DraconicEngine.Terminals;
using DraconicEngine.Widgets;
using DragonRising.GameWorld.Components;
using DragonRising.Generators;
using DragonRising.Services;
using DragonRising.Storage;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.GameWorld;

namespace DragonRising.Views
{
   class NewGameScreen : IGameView
   {
      public GameViewType Type => GameViewType.Screen;

      string nameInProgress;

      ITerminal namePromptTerminal;

      public NewGameScreen()
      {
         this.namePromptTerminal = RogueGame.Current.RootTerminal[7, 10][RogueColors.White];
      }

      int tick = 0;
      bool showCursor = false;

      public Task Draw()
      {
         RogueGame.Current.RootTerminal.Clear();

         if (tick % 15 == 0)
         {
            showCursor = !showCursor;
         }

         if (showCursor)
         {
            this.namePromptTerminal.Write("Enter your name: " + nameInProgress + "|");
         }
         else
         {
            this.namePromptTerminal.Write("Enter your name: " + nameInProgress);
         }

         return Task.FromResult(0);
      }
      
      public void OnStart()
      {

      }

      public async Task<TickResult> Tick()
      {
         var keyPress = await InputSystem.Current.GetKeyPressAsync();

         if (keyPress.Key == RogueKey.Enter)
         {
            if (nameInProgress != string.Empty)
            {
               CreateNew(nameInProgress);

               return TickResult.Finished;
            }
            return TickResult.Continue;
         }
         else if (keyPress.Key == RogueKey.Escape)
         {
            return TickResult.Finished;
         }
         else if (keyPress.Key == RogueKey.Backspace || keyPress.Key == RogueKey.Delete)
         {
            if (this.nameInProgress != string.Empty)
            {
               this.nameInProgress = this.nameInProgress.Substring(0, this.nameInProgress.Length - 1);
            }
         }
         else if (keyPress.Char.HasValue && Character.IsGlyph(keyPress.Char.Value))
         {
            this.nameInProgress += keyPress.Char.Value;
         }

         return TickResult.Continue;
      }

      public World World { get; set; }
      public string GameName { get; set; }


      void CreateNew(string name)
      {
         var player = new Entity(name,
            new ComponentSet(
               new DrawnComponent() { SeenCharacter = new Character(Glyph.At, RogueColors.White) },
               new CombatantComponent(hp: 30, defense: 2, power: 5),
               new CreatureComponent(6),
               new InventoryComponent() { Capacity = 26 },
               new BehaviorComponent()),
            new StatSet(new CharacterStat<int>("Level", 1)))
         { Blocks = true };

         var inventory = player.GetComponent<InventoryComponent>();

         inventory.Items.Add(Library.Current.Items.Get(TempConstants.ScrollOfLightningBolt).Clone());
         inventory.Items.Add(Library.Current.Items.Get(TempConstants.ScrollOfFireball).Clone());
         inventory.Items.Add(Library.Current.Items.Get(TempConstants.ScrollOfConfusion).Clone());

         var world = new World(player);

         this.World = world;
         this.GameName = name;
      }
   }
}