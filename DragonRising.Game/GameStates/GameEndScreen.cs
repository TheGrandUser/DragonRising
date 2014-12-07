﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameStates;
using DraconicEngine.Input;
using DraconicEngine.Widgets;
using DraconicEngine.Terminals;
using LanguageExt;
using LanguageExt.Prelude;

namespace DragonRising.GameStates
{
   class GameEndScreen : IGameState
   {
      ITerminal messageTerminal;

      List<Widget> widgets = new List<Widget>();
      public List<Widget> Widgets { get { return widgets; } }

      string message = "Game Over (Press Enter to continue)";

      public GameEndScreen()
      {
         var lowerRight = RogueGame.Current.RootTerminal.LowerRight;
         var messageSize = new Vector(message.Length + 4, 5);

         var messagePosition = (lowerRight - messageSize) / 2;

         this.messageTerminal = RogueGame.Current.RootTerminal[RogueColors.White, RogueColors.Black][messagePosition, messageSize];
      }

      public async Task<TickResult> Tick()
      {
         var keyEvent = await InputSystem.Current.GetKeyPressAsync();

         if (keyEvent.Key == RogueKey.Enter)
         {
            return TickResult.Finished;
         }
         return TickResult.Continue;
      }

      public void Draw()
      {
         this.messageTerminal.Clear();

         messageTerminal.DrawBox(DrawBoxOptions.DoubleLines);
         messageTerminal[2, 2].Write(message);
      }

      public GameStateType Type { get { return GameStateType.Dialog; } }

      public Option<IGameState> Finish()
      {
         return None;
      }

      public void Start()
      {
      }
   }
}
