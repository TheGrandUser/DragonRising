using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.EntitySystem;
using DraconicEngine.GameViews;
using DraconicEngine.Input;
using DraconicEngine.Terminals;
using LanguageExt;
using static LanguageExt.Prelude;
using DragonRising.GameWorld.Components;

namespace DragonRising.Views
{
   class InventoryScreen : IGameView<int?>
   {
      InventoryComponent inventory;

      string header;
      ITerminal mainTerminal;
      ITerminal itemsTerminal;
      
      public GameViewType Type { get { return GameViewType.WholeScreen; } }

      public InventoryScreen(InventoryComponent inventory, Option<string> message, ITerminal hostPanel)
      {
         this.inventory = inventory;
         this.mainTerminal = hostPanel;

         this.header = message.Match(
            Some: m => inventory.Owner.Name + "'s inventory: " + m,
            None: () => inventory.Owner.Name + "'s inventory");

         var margin = new Vector(3, 4);
         var screenSize = hostPanel.Size;

         this.itemsTerminal = hostPanel[(Loc)margin, screenSize - margin * 2];
      }

      public async Task<int?> DoLogic()
      {
         while (true)
         {
            var keyEvent = await InputSystem.Current.GetKeyPressAsync();

            int index = keyEvent.Key - RogueKey.A;

            if (index >= 0 && index < this.inventory.Items.Count)
            {
               return index;
            }
            else if (keyEvent.Key == RogueKey.Escape)
            {
               return null;
            }
         }
      }

      public void Draw()
      {
         this.mainTerminal.DrawBox(DrawBoxOptions.DoubleLines);

         var headerPosition = (mainTerminal.Size.X - this.header.Length) / 2;
         this.mainTerminal[headerPosition, 1].Write(this.header);

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
               var text = "(" + option + ") " + item.Name;
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
   }
}
