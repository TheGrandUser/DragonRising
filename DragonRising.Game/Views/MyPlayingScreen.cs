using DraconicEngine;
using DraconicEngine.GameViews;
using DragonRising.GameWorld.Alligences;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.Input;
using DraconicEngine.Terminals;
using DraconicEngine.Widgets;
using DragonRising.GameWorld.Systems;
using DragonRising.Services;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonRising.GameWorld.Components;
using DragonRising.Widgets;
using DragonRising.Storage;
using DragonRising.GameWorld;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.PubSubEvents;

namespace DragonRising.GameStates
{
   class MyPlayingScreen : PlayingScreen
   {
      public static readonly int BarWidth = 20;
      public static readonly int PanelHeight = 9;
      public static readonly int PanelY = DragonRisingGame.ScreenHeight - PanelHeight;
      public static readonly int SceneHeight = DragonRisingGame.ScreenHeight - PanelHeight - 1;

      public static readonly int MessageX = BarWidth + 2;
      public static readonly int MessageWidth = DragonRisingGame.ScreenWidth - BarWidth - 3;
      public static readonly int MessageHeight = PanelHeight - 2;

      ITerminal statsPanel;

      BarWidget hpBarWidget;
      BarWidget xpBarWidget;
      MessagesWidget messageWidget;
      HighlightWidget highlightWidget;
      MessagesWidget infoWidget;
      SceneWidget sceneWidget;
      FocusEntitySceneView sceneView;
      LifeDeathMonitorService lifeDeathMonitorService;

      StatTracker statTracker;

      Queue<IAsyncInterruption> interruptions = new Queue<IAsyncInterruption>();

      public World World { get; set; }

      public ITerminal ScenePanel { get { return sceneWidget.Panel; } }

      IMessageService messageService;
      List<RogueMessage> infoMessages = new List<RogueMessage>();
      private Terminal rootTerminal;
      
      string gameName;

      public PlayerController PlayerController { get; set; }

      public MyPlayingScreen(World world, string gameName)
      {
         this.World = world;

         var player = world.Player;
         this.gameName = gameName;

         this.messageService = MessageService.Current;
         this.rootTerminal = DragonRisingGame.Current.RootTerminal;

         var scenePanel = rootTerminal[1, 1, DragonRisingGame.ScreenWidth - 2, SceneHeight];
         this.sceneView = new FocusEntitySceneView(this.World, scenePanel, player);

         this.statsPanel = rootTerminal[0, PanelY, DragonRisingGame.ScreenWidth, PanelHeight];
         this.sceneWidget = new SceneWidget(world, this.sceneView, scenePanel);

         this.hpBarWidget = new BarWidget(this.statsPanel[1, 1, BarWidth, 1], "HP",
            () => player.GetComponent<CombatantComponent>().HP,
            () => player.GetComponent<CombatantComponent>().MaxHP,
            RogueColors.Red, RogueColors.DarkRed);

         this.xpBarWidget = new BarWidget(this.statsPanel[1, 3, BarWidth, 1], "XP",
            () => player.GetComponent<CombatantComponent>().XP,
            () => LevelingPolicy.XpForNextLevel(player.GetComponent<LevelComponent>().Level),
            RogueColors.Purple, RogueColors.DarkPurple);

         this.infoWidget = new MessagesWidget(this.statsPanel[1, 2, BarWidth, this.statsPanel.Size.Y - 3], this.infoMessages, MessagePriority.ShowOldest);

         this.messageWidget = new MessagesWidget(this.statsPanel[MessageX, 1, MessageWidth, MessageHeight], this.messageService.Messages, MessagePriority.ShowNewest);

         this.highlightWidget = new HighlightWidget(this.sceneWidget.Panel[RogueColors.Black, RogueColors.LightCyan]);
         
         this.Engine.AddSystem(new ActionSystem(ServiceLocator.Current.GetInstance<IRulesManager>()), 1, SystemTrack.Game);
         this.Engine.AddSystem(new RenderSystem(scenePanel, sceneView, world), 4, SystemTrack.Render);
         this.Engine.AddSystem(new ItemRenderSystem(scenePanel, sceneView, world), 5, SystemTrack.Render);
         this.Engine.AddSystem(new CreatureRenderSystem(scenePanel, sceneView, world), 6, SystemTrack.Render);

         this.Widgets.Add(sceneWidget);
         this.Widgets.Add(hpBarWidget);
         this.Widgets.Add(xpBarWidget);
         this.Widgets.Add(messageWidget);
         this.Widgets.Add(highlightWidget);
         this.Widgets.Add(infoWidget);

         this.PlayerController = new PlayerController(sceneView) { PlayerCreature = player };
         this.lifeDeathMonitorService = new LifeDeathMonitorService(ServiceLocator.Current.GetInstance<IEventAggregator>());
         this.statTracker = new StatTracker(ServiceLocator.Current.GetInstance<IEventAggregator>());
      }

      protected override void PreSceneDraw()
      {
         this.rootTerminal.DrawBox(DrawBoxOptions.DoubleLines);

         this.statsPanel.DrawBox(DrawBoxOptions.DoubleLines);
      }

      protected override Some<IGameView> CreateEndScreen()
      {
         return new GameEndScreen();
      }

      public override void Start()
      {
         base.Start();
         MyPlayingScreen.Current = this;
         World.Current = World;

         this.messageService.PostMessage("Welcome stranger! Prepare to perish in the Tombs of the Ancient Kings.", RogueColors.Red);
      }

      protected override Option<IGameView> OnFinished()
      {
         this.statTracker.Dispose();
         this.lifeDeathMonitorService.Dispose();
         
         SaveManager.Current.SaveGame(this.gameName, this.World);

         MyPlayingScreen.Current = null;
         World.Current = null;

         this.World.Dispose();

         return None;
      }

      public static new MyPlayingScreen Current { get; private set; }

      protected override EntityEngine Engine => this.World.EntityEngine;

      public void ClearInfoMessages()
      {
         this.infoMessages.Clear();
      }

      public void AddInfoMessage(RogueMessage message)
      {
         this.infoMessages.Add(message);
      }

      public void ClearHighlight()
      {
         this.highlightWidget.Enabled = false;
      }
      public void SetHighlight(Loc scenePoint)
      {

         this.highlightWidget.Enabled = true;
         this.highlightWidget.Location = scenePoint;
      }

      protected override bool IsGameEndState()
      {
         return this.lifeDeathMonitorService.HasPlayerLost;
      }

      protected override bool IsUnderPlayerControl()
      {
         return this.PlayerController != null;
      }

      protected override async Task<PlayerTurnResult> GetPlayerTurn(TimeSpan timeout)
      {
         while(interruptions.Count > 0)
         {
            var interruption = interruptions.Dequeue();
            if (interruption.StillApplies())
            {
               await interruption.Run();
            }
         }
         return await this.PlayerController.GetInputAsync(timeout);
      }

      public override void AddAsyncInterruption(IAsyncInterruption interruption)
      {
         this.interruptions.Enqueue(interruption);
      }
   }
}
