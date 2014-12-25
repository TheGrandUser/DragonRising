using DraconicEngine;
using DraconicEngine.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine.Terminals;
using DraconicEngine.Storage;
using DraconicEngine.Items;
using DragonRising.Entities.Items;
using DragonRising.Items;
using DraconicEngine.GameWorld.Alligences;
using DragonRising.TempConstants;
using LanguageExt;
using LanguageExt.Prelude;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;

namespace DragonRising.GameStates
{
   class LoadGeneralDataScreen : IGameState
   {
      private readonly ITerminal messageTerminal;

      string message = "Loading...";

      Queue<ILoader> loaders = new Queue<ILoader>();

      public LoadGeneralDataScreen()
      {
         loaders.Enqueue(new TileLoader());
         loaders.Enqueue(new PowersLoader());
         loaders.Enqueue(new ItemsLoader());
         loaders.Enqueue(new AlligencesLoader());
         loaders.Enqueue(new CreaturesLoader());

         var height = 7;
         var width = loaders.Max(s => s.Message.Length) + 3 * 2;

         var x = (DragonRisingGame.ScreenWidth - width) / 2;
         var y = DragonRisingGame.ScreenHeight - height - 5;

         this.messageTerminal = DragonRisingGame.Current.RootTerminal[x, y, width, height];
      }

      public GameStateType Type { get { return GameStateType.Screen; } }

      public Task Draw()
      {
         RogueGame.Current.RootTerminal.Clear();

         this.messageTerminal[3, 3][RogueColors.LightGray].Write(message);

         return Task.FromResult(0);
      }

      public Option<IGameState> Finish()
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
         var itemLibrary = Library.Items;

         itemLibrary.Add(Make(HealingPotion, Glyph.ExclamationMark, RogueColors.Violet, RogueColors.Violet, new HealingItem()));
         itemLibrary.Add(Make(ScrollOfLightningBolt, Glyph.Pound, RogueColors.LightYellow, RogueColors.LightYellow, new LightningScroll()));
         itemLibrary.Add(Make(ScrollOfFireball, Glyph.Pound, RogueColors.Yellow, RogueColors.Yellow, new FireballScroll()));
         itemLibrary.Add(Make(ScrollOfConfusion, Glyph.Pound, RogueColors.Violet, RogueColors.Violet, BehaviorReplacementItem.CreateConfusionItem()));

         //Library.SetItemLibrary(itemLibrary);

         return Task.Delay(TimeSpan.FromSeconds(0.5));
      }

      static Entity Make(string name, Glyph glyph, RogueColor seen, RogueColor explored, IItemUsage usage)
      {
         return new Entity(name,
            new DrawnComponent()
            {
               SeenCharacter = new Character(glyph, seen),
               ExploredCharacter = new Character(glyph, explored),
            },
            new LocationComponent(),
            new ItemComponent() { Usage = usage });
      }
   }

   class AlligencesLoader : ILoader
   {
      public string Message => "Loading Alligences...";
      public Task Load()
      {
         var manager = AlligenceManager.Current;//new SimpleAlligenceManager();
         manager.Add(new Alligence() { Name = "Player" });
         //manager.Add(new Alligence() { Name = "Greenskins" });

         //AlligenceManager.SetAlligenceManager(manager);

         return Task.Delay(TimeSpan.FromSeconds(0.5));
      }
   }

   class CreaturesLoader : ILoader
   {
      public string Message => "Loading Creatures...";
      public Task Load()
      {

         return Task.Delay(TimeSpan.FromSeconds(0.5));
      }
   }
}
