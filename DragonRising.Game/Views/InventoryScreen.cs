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
   class InventoryScreen : IGameView
   {
      InventoryComponent inventory;

      string header;
      ITerminal rootTerminal;
      ITerminal itemsTerminal;

      private  int? result;
      public int? Result { get { return this.result; } }

      public GameViewType Type { get { return GameViewType.WholeScreen; } }

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

      public async Task<TickResult> DoLogic()
      {
         while (true)
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
         }
      }

      public Task Draw()
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

         return Task.CompletedTask;
      }
   }
}
