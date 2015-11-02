using DraconicEngine;
using DraconicEngine.GameViews;
using DragonRising.GameWorld.Alligences;
using DraconicEngine.EntitySystem;
using DraconicEngine.Input;
using DraconicEngine.Terminals;
using DraconicEngine.Widgets;
using DragonRising.GameWorld.Components;
using DragonRising.Generators;
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
   class NewGameScreen : IGameView<Option<World>>
   {
      ISceneGenerator sceneGenerator;
      
      int tick = 0;
      bool showCursor = false;

      string nameInProgress;

      ITerminal namePromptTerminal;

      public NewGameScreen(ISceneGenerator sceneGenerator)
      {
         this.sceneGenerator = sceneGenerator;
         this.namePromptTerminal = RogueGame.Current.RootTerminal[7, 10][RogueColors.White];
      }

      public GameViewType Type => GameViewType.WholeScreen;
      public void Draw()
      {
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
      }
      
      public async Task<Option<World>> DoLogic()
      {
         while (true)
         {
            var keyPress = await InputSystem.Current.GetKeyPressAsync();

            if (keyPress.Key == RogueKey.Enter)
            {
               if (nameInProgress != string.Empty)
               {
                  var world = CreateNew(nameInProgress);

                  return world;
               }
            }
            else if (keyPress.Key == RogueKey.Escape)
            {
               return None;
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
         }
      }
      
      World CreateNew(string name)
      {
         var player = new Entity(name,
            new ComponentSet(
               new DrawnComponent() { SeenCharacter = new Character(Glyph.DUpper, RogueColors.White) },
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

         sceneGenerator.GenerateNewScene(world);

         return world;
      }
   }
}