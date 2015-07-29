﻿using DraconicEngine.Input;
using DraconicEngine.Terminals;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraconicEngine.GameViews
{
   public class ConfirmDialog : IGameView
   {
      string message;
      List<CharacterString> lines = new List<CharacterString>();
      ITerminal dialogPanel;

      public GameViewType Type { get { return GameViewType.Dialog; } }

      public ConfirmDialog(string message, ITerminal hostPanel)
      {

         int boarderSize = 4;
         var maxSize = hostPanel.Size.X - boarderSize;
         this.message = message + " Y / N";

         if (this.message.Length > maxSize)
         {
            var splitLines = new CharacterString(this.message).WordWrap(maxSize);

            this.lines.AddRange(splitLines);
         }
         else
         {
            this.lines.Add(new CharacterString(this.message));
         }

         var width = this.lines.Max(line => line.Count) + boarderSize;
         var height = this.lines.Count + boarderSize;

         Vector size = new Vector(width, height);
         Loc position = (hostPanel.LowerRight - size) / 2;

         this.dialogPanel = hostPanel[position, size][RogueColors.White, RogueColors.Black];
      }

      public bool Result { get; private set; }

      public async Task<TickResult> Tick()
      {
         while (true)
         {
            var key = await InputSystem.Current.GetKeyPressAsync();

            if (key.Key == RogueKey.Y)
            {
               this.Result = true;
               return TickResult.Finished;
            }
            else if (key.Key == RogueKey.N)
            {
               this.Result = false;
               return TickResult.Finished;
            }
         }
      }

      public Task Draw()
      {
         this.dialogPanel.Clear();
         this.dialogPanel.DrawBox(DrawBoxOptions.DoubleLines);
         
         var margine = 2;

         for (int i = 0; i < this.lines.Count; i++)
         {
            this.dialogPanel[margine, margine + i].Write(this.lines[i]);
         }

         return Task.FromResult(0);
      }
      
      public void OnStart() { }
   }
}