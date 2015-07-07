using DraconicEngine;
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

namespace DragonRising.Views
{
   class LoadGeneralDataScreen : IGameView
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

         this.messageTerminal = DragonRisingGame.Current.RootTerminal[x, y, width, height];
      }

      public GameViewType Type { get { return GameViewType.Screen; } }

      public Task Draw()
      {
         RogueGame.Current.RootTerminal.Clear();

         this.messageTerminal[3, 3][RogueColors.LightGray].Write(message);

         return Task.FromResult(0);
      }

      public Option<IGameView> Finish()
      {
         return None;
      }

      public void Start()
      {
      }

      public async Task<TickResult> Tick()
      {
         if (loaders.Count == 0)
         {
            return TickResult.Finished;
         }
         var loader = this.loaders.Dequeue();

         message = loader.Message;

         await loader.Load();

         return loaders.Count > 0 ? TickResult.Continue : TickResult.Finished;
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
         var powerLibrary = Library.Current.Powers;

         powerLibrary.Add(new CureMinorWoundsPower());
         powerLibrary.Add(new LightningPower());
         powerLibrary.Add(new FireballPower());
         powerLibrary.Add(new ConfuseNearestPower(8));

         var itemLibrary = Library.Current.Items;

         itemLibrary.Add(Make(HealingPotion, Glyph.ExclamationMark, RogueColors.Violet, RogueColors.Violet, powerLibrary.Get("Cure Minor Wounds")));
         itemLibrary.Add(Make(ScrollOfLightningBolt, Glyph.Pound, RogueColors.LightYellow, RogueColors.LightYellow, powerLibrary.Get("Lightning bolt")));
         itemLibrary.Add(Make(ScrollOfFireball, Glyph.Pound, RogueColors.Yellow, RogueColors.Yellow, powerLibrary.Get("Fireball")));
         itemLibrary.Add(Make(ScrollOfConfusion, Glyph.Pound, RogueColors.Violet, RogueColors.Violet, powerLibrary.Get("Confuse Nearest")));
         
         return Task.Delay(TimeSpan.FromSeconds(0.5));
      }

      static Entity Make(string name, Glyph glyph, RogueColor seen, RogueColor explored, Power power)
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
                  Usable = new Usable(power) { IsCharged = true, Charges = 1, MaxCharges = 1 }
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
