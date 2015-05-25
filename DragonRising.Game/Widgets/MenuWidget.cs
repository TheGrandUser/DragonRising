using DraconicEngine.GameViews;
using DraconicEngine.Terminals;
using DraconicEngine.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.Input;
using static DraconicEngine.Input.CommandGestureFactory;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading;
using DraconicEngine.Terminals.Input;

// message
// .
// item 1
// item 2
// item 3
// .
// page



// .
// message
// .
// item 1
// .
// item 2
// .
// item 3
// .
// page
// .

namespace DragonRising.Widgets
{
   class MenuItem<TValue>
   {
      //private Func<Task<TickResult>> command;

      private Func<bool> canExecute;

      public bool CanExecute() => canExecute?.Invoke() ?? true;
      public TValue Value { get; }
      //public Task<TickResult> Execute() => command();

      public MenuItem(string name, TValue value, Func<bool> canExecute = null)
      {
         this.Name = name;
         this.Value = value;
         this.canExecute = canExecute;
      }

      public string Name { get; private set; }
   }

   class MenuWidget<TValue> : Widget
   {
      string message;
      int currentMenuItem = 0;
      int currentMenuPage = 0;
      int spacingPerItem;

      int itemsPerPage;
      int pageCount;
      ITerminal messagePanel;
      ITerminal itemsPanel;
      ITerminal pagePanel;

      List<MenuItem<TValue>> menuItems;

      enum MenuNavigation
      {
         Up,
         Down,
         Select,
         MouseSelect,
         MousePoint,
      }

      public MenuWidget(ITerminal terminal, string message, bool compact, params MenuItem<TValue>[] items)
         : base(terminal)
      {
         if (terminal.Size.Y < (compact ? 7 : 11)) // needs room for at least three items
         {
            throw new ArgumentException("The given terminal is too small, must be at least 11 spaces high");
         }
         if (!items.Any(i => i.CanExecute()))
         {
            throw new ArgumentException("No menu items are valid");
         }
         this.message = message;

         this.menuItems = items.ToList();

         int messageY = compact ? 0 : 1;
         int firstItemY = compact ? 1 : 3;
         int pageYFromBottom = compact ? 0 : 1;

         int messageArea = messageY + (compact ? 1 : 2);
         int pageArea = pageYFromBottom + 1;

         int buffer = compact ? 4 : 5;
         spacingPerItem = compact ? 1 : 2;

         var availableItemHeight = terminal.Size.Y - messageArea;

         this.itemsPerPage = availableItemHeight / spacingPerItem;
         if(items.Length > itemsPerPage)
         {
            availableItemHeight -= pageArea;
            this.itemsPerPage = availableItemHeight / spacingPerItem;
         }
         
         this.pageCount = (this.menuItems.Count / itemsPerPage) + (this.menuItems.Count % itemsPerPage == 0 ? 0 : 1);

         this.messagePanel = terminal[3, messageY];
         this.itemsPanel = terminal[3, firstItemY];
         this.pagePanel = terminal[3, terminal.Size.Y - pageYFromBottom];

         this.currentMenuItem = menuItems.Select((mi, i) => new { mi, i }).First(mi => mi.mi.CanExecute()).i;
      }

      public async Task<Option<TValue>> Tick()
      {
         var result = await InputSystem.Current.GetCommandAsync(this.Gestures, CancellationToken.None);
         var command = result.Command as ValueCommand<MenuNavigation>;

         if (command.Value == MenuNavigation.Select)
         {
            if (currentMenuItem >= 0 && currentMenuItem < menuItems.Count)
            {
               if (menuItems[currentMenuItem].CanExecute())
               {
                  return menuItems[currentMenuItem].Value;
               }
            }
         }
         else if (command.Value == MenuNavigation.Up)
         {
            var validItems = menuItems.Select((mi, i) => new { mi, i })
               .Where(mi => mi.mi.CanExecute()).Select(me => me.i)
               .ToList();

            var currentValidIndex = validItems.IndexOf(currentMenuItem);

            --currentValidIndex;
            if (currentValidIndex < 0)
            {
               currentValidIndex = 0;
            }

            currentMenuItem = validItems[currentValidIndex];
            return None;
         }
         else if (command.Value == MenuNavigation.Down)
         {
            var validItems = menuItems.Select((mi, i) => new { mi, i })
               .Where(mi => mi.mi.CanExecute()).Select(me => me.i)
               .ToList();

            var currentValidIndex = validItems.IndexOf(currentMenuItem);

            ++currentValidIndex;
            if (currentValidIndex >= validItems.Count)
            {
               currentValidIndex = validItems.Count - 1;
            }

            currentMenuItem = validItems[currentValidIndex];
            return None;
         }
         else if (command.Value == MenuNavigation.MouseSelect)
         {
            return None;
         }
         else if (command.Value == MenuNavigation.MousePoint)
         {
            return None;
         }

         return None;
      }

      public override void Draw()
      {
         this.messagePanel.Write(this.message);

         for (int i = 0; i < this.menuItems.Count; i++)
         {
            var menuItem = this.menuItems[i];
            var color = menuItem.CanExecute() ? (currentMenuItem == i ? RogueColors.White : RogueColors.LightGray) : RogueColors.Gray;
            this.itemsPanel[0, i * spacingPerItem][color].Write(menuItem.Name);
         }

         if (pageCount > 1)
         {
            this.pagePanel.Write($"Page {this.currentMenuPage}/{this.pageCount}");
         }
      }


      CommandGesture upGesture = CreateGesture(MenuNavigation.Up, GestureSet.Create(RogueKey.Up, RogueKey.NumPad8));
      CommandGesture downGesture = CreateGesture(MenuNavigation.Down, GestureSet.Create(RogueKey.Down, RogueKey.NumPad2));
      CommandGesture selectGesture = CreateGesture(MenuNavigation.Select, GestureSet.Create(RogueKey.Enter, RogueKey.Space));

      CommandGesture mouseSelectGesture = CreateGesture(MenuNavigation.MouseSelect, GestureSet.Create(RogueMouseAction.LeftClick));
      CommandGesture mousePointGesture = CreateGesture(MenuNavigation.MousePoint, GestureSet.Create(RogueMouseAction.Movement));

      IEnumerable<CommandGesture> Gestures
      {
         get
         {
            yield return upGesture;
            yield return downGesture;
            yield return selectGesture;
            //yield return mouseSelectGesture;
            //yield return mousePointGesture;
         }
      }
   }
}












