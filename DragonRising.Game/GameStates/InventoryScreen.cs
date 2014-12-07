using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameStates;
using DraconicEngine.Input;
using DraconicEngine.Terminals;
using DraconicEngine.GameWorld.EntitySystem.Components;
using LanguageExt;
using LanguageExt.Prelude;

namespace DragonRising.GameStates
{
   class InventoryScreen : IGameState<int?>
   {
      InventoryComponent inventory;

      string header;
      ITerminal rootTerminal;
      ITerminal itemsTerminal;

      private  int? result;
      public int? Result { get { return this.result; } }

      public GameStateType Type { get { return GameStateType.Screen; } }

      public InventoryScreen(InventoryComponent inventory, Option<string> message)
      {
         this.inventory = inventory;

         this.rootTerminal = RogueGame.Current.RootTerminal;

         this.header = message.Match(
            Some: m => inventory.Owner.Name + "'s inventory: " + m,
            None: () => inventory.Owner.Name + "'s inventory");

         var margin = new Vector(3, 4);
         var screenSize = rootTerminal.Size;

         this.itemsTerminal = rootTerminal[(Loc)margin, screenSize - margin * 2];
      }

      public async Task<TickResult> Tick()
      {
         var keyEvent = await InputSystem.Current.GetKeyPressAsync();

         int index = keyEvent.Key - RogueKey.A;

         if (index >= 0 && index < this.inventory.Items.Count)
         {
            this.result = index;
            return TickResult.Finished;
         }
         else if (keyEvent.Key == RogueKey.Escape)
         {
            this.result = null;
            return TickResult.Finished;
         }

         return TickResult.Continue;
      }

      public void Draw()
      {
         this.rootTerminal.Clear();
         this.rootTerminal.DrawBox(DrawBoxOptions.DoubleLines);

         var headerPosition = (rootTerminal.Size.X - this.header.Length) / 2;
         this.rootTerminal[headerPosition, 1].Write(this.header);

         if (this.inventory.Items.Count == 0)
         {
            this.itemsTerminal.Write("Inventory is empty.");
         }
         else
         {
            bool skipLines = this.inventory.Items.Count < this.itemsTerminal.Size.Y / 2;

            char option = 'a';
            int y = 0;

            foreach (var item in this.inventory.Items)
            {
               var text = "(" + option + ") " + item.Template.Name;
               this.itemsTerminal[0, y].Write(text);
               y++;
               option++;
               if (skipLines)
               {
                  y++;
               }
            }
         }
      }

      public Option<IGameState> Finish()
      {
         return None;
      }

      public void Start()
      {
      }
   }
}
