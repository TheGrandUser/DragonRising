﻿using DraconicEngine;
using DraconicEngine.GameViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Terminals;
using DragonRising.GameWorld.Alligences;
using static DragonRising.TempConstants;
using LanguageExt;
using static LanguageExt.Prelude;
using DraconicEngine.EntitySystem;
using DragonRising.Storage;
using DragonRising.GameWorld.Components;
using DragonRising.GameWorld.Effects;
using DragonRising.GameWorld.Conditions;
using DraconicEngine.RulesSystem;
using DragonRising.GameWorld.Powers;
using DragonRising.Plans;
using DragonRising.GameWorld.Powers.Spells;

namespace DragonRising.Views
{
   class LoadGeneralDataScreen : IGameView<Unit>
   {
      private readonly ITerminal messageTerminal;

      string message = "Loading...";

      Queue<ILoader> loaders = new Queue<ILoader>();

      public LoadGeneralDataScreen()
      {
         loaders.Enqueue(new TileLoader());
         loaders.Enqueue(new PowersLoader());
         loaders.Enqueue(new ItemsLoader());
         //loaders.Enqueue(new AlligencesLoader());
         loaders.Enqueue(new CreaturesLoader());

         var height = 7;
         var width = loaders.Max(s => s.Message.Length) + 3 * 2;

         var x = (DragonRisingGame.ScreenWidth - width) / 2;
         var y = DragonRisingGame.ScreenHeight - height - 5;

         this.messageTerminal = DragonRisingGame.Current.RootTerminal[x, y, width, height][RogueColors.LightGray];
      }

      public GameViewType Type { get { return GameViewType.WholeScreen; } }

      public void Draw()
      {
         this.messageTerminal.Write(message);
      }
      
      public async Task<Unit> DoLogic()
      {
         while (loaders.Count > 0)
         {
            var loader = this.loaders.Dequeue();

            message = loader.Message;

            await loader.Load();
         }

         return unit;
      }
   }


   interface ILoader
   {
      string Message { get; }
      Task Load();
   }

   class TileLoader : ILoader
   {
      public string Message => "Loading Tiles...";
      public Task Load() => Task.Delay(TimeSpan.FromSeconds(0.05));
   }

   class PowersLoader : ILoader
   {
      public string Message => "Loading Powers...";
      public Task Load() => Task.Delay(TimeSpan.FromSeconds(0.15));
   }

   class ItemsLoader : ILoader
   {
      public string Message => "Loading Item...";
      public Task Load()
      {
         var spellLibrary = Library.Current.Spells;
         
         var itemLibrary = Library.Current.Items;

         itemLibrary.Add(Make(HealingPotion, Glyph.ExclamationMark, RogueColors.Violet, RogueColors.Violet, spellLibrary.Get("Cure Minor Wounds")));
         itemLibrary.Add(Make(ScrollOfLightningBolt, Glyph.Pound, RogueColors.LightYellow, RogueColors.LightYellow, spellLibrary.Get("Lightning Jolt")));
         itemLibrary.Add(Make(ScrollOfFireball, Glyph.Pound, RogueColors.Yellow, RogueColors.Yellow, spellLibrary.Get("Fireball")));
         itemLibrary.Add(Make(ScrollOfConfusion, Glyph.Pound, RogueColors.Violet, RogueColors.Violet, spellLibrary.Get("Confusion")));
         
         return Task.Delay(TimeSpan.FromSeconds(0.5));
      }

      static Entity Make(string name, Glyph glyph, RogueColor seen, RogueColor explored, Spell spell)
      {
         return new Entity(name,
            new ComponentSet(
               new DrawnComponent()
               {
                  SeenCharacter = new Character(glyph, seen),
                  ExploredCharacter = new Character(glyph, explored),
               },
               new ItemComponent()
               {
                  Usable = new SpellUsable(spell) { IsCharged = true, Charges = 1, MaxCharges = 1 }
               }));
      }
   }

   //class AlligencesLoader : ILoader
   //{
   //   public string Message => "Loading Alligences...";
   //   public Task Load()
   //   {
   //      var manager = AlligenceManager.Current;//new SimpleAlligenceManager();
   //      manager.Add(new Alligence() { Name = "Player" });
   //      //manager.Add(new Alligence() { Name = "Greenskins" });

   //      //AlligenceManager.SetAlligenceManager(manager);

   //      return Task.Delay(TimeSpan.FromSeconds(0.5));
   //   }
   //}

   class CreaturesLoader : ILoader
   {
      public string Message => "Loading Creatures...";
      public Task Load()
      {

         return Task.Delay(TimeSpan.FromSeconds(0.5));
      }
   }
}
