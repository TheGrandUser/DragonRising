using DraconicEngine;
using DraconicEngine.GameStates;
using DraconicEngine.GameWorld.Alligences;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DraconicEngine.GameWorld.EntitySystem.Systems;
using DraconicEngine.Input;
using DraconicEngine.Storage;
using DraconicEngine.Terminals;
using DraconicEngine.Widgets;
using DragonRising.Services;
using LanguageExt;
using LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonRising.GameStates
{
   class MyPlayingState : PlayingState
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
      MessagesWidget messageWidget;
      HighlightWidget highlightWidget;
      MessagesWidget infoWidget;
      SceneWidget sceneWidget;
      FocusEntitySceneView sceneView;
      LifeDeathMonitorService lifeDeathMonitorService;

      public ITerminal ScenePanel { get { return sceneWidget.Panel; } }

      IMessageService messageService;
      List<RogueMessage> infoMessages = new List<RogueMessage>();
      private Terminal rootTerminal;

      string gameName;

      public PlayerController PlayerController { get; set; }

      public MyPlayingState(Scene scene, string gameName)
         : base(scene)
      {
         var player = scene.FocusEntity;
         this.gameName = gameName;

         this.messageService = MessageService.Current;
         this.rootTerminal = DragonRisingGame.Current.RootTerminal;

         var scenePanel = rootTerminal[1, 1, DragonRisingGame.ScreenWidth - 2, SceneHeight];
         this.sceneView = new FocusEntitySceneView(this.Scene, scenePanel, player);

         this.statsPanel = rootTerminal[0, PanelY, DragonRisingGame.ScreenWidth, PanelHeight];
         this.sceneWidget = new SceneWidget(scene, this.sceneView, scenePanel);

         this.hpBarWidget = new BarWidget(this.statsPanel[1, 1, BarWidth, 1], "HP",
            () => player.GetComponent<CombatantComponent>().HP,
            () => player.GetComponent<CombatantComponent>().MaxHP,
            RogueColors.Red, RogueColors.DarkRed);

         this.infoWidget = new MessagesWidget(this.statsPanel[1, 2, BarWidth, this.statsPanel.Size.Y - 3], this.infoMessages, MessagePriority.ShowOldest);

         this.messageWidget = new MessagesWidget(this.statsPanel[MessageX, 1, MessageWidth, MessageHeight], this.messageService.Messages, MessagePriority.ShowNewest);

         this.highlightWidget = new HighlightWidget(this.sceneWidget.Panel[RogueColors.Black, RogueColors.LightCyan]);

         var gameManagerEntity = new Entity()
         {
            Name = "Game Manager"
         };
         gameManagerEntity.AddComponent(new GameActionsComponent());

         this.Engine.AddEntity(gameManagerEntity);

         this.Engine.AddSystem(new AIDecisionSystem(), 1, SystemTrack.Game);
         this.Engine.AddSystem(new CreatureActionSystem(), 2, SystemTrack.Game);

         this.Engine.AddSystem(new RenderSystem(scenePanel, sceneView, scene), 4, SystemTrack.Render);
         this.Engine.AddSystem(new ItemRenderSystem(scenePanel, sceneView, scene), 5, SystemTrack.Render);
         this.Engine.AddSystem(new CreatureRenderSystem(scenePanel, sceneView, scene), 6, SystemTrack.Render);

         this.Widgets.Add(sceneWidget);
         this.Widgets.Add(hpBarWidget);
         this.Widgets.Add(messageWidget);
         this.Widgets.Add(highlightWidget);
         this.Widgets.Add(infoWidget);

         this.PlayerController = new PlayerController(sceneView) { PlayerCreature = player };
         this.lifeDeathMonitorService = new LifeDeathMonitorService(scene.EntityStore);
      }

      protected override void PreSceneDraw()
      {
         this.rootTerminal.DrawBox(DrawBoxOptions.DoubleLines);

         this.statsPanel.DrawBox(DrawBoxOptions.DoubleLines);
      }

      protected override Some<IGameState> CreateEndScreen()
      {
         return new GameEndScreen();
      }

      public override void Start()
      {
         base.Start();
         MyPlayingState.Current = this;

         Scene.PushScene(this.Scene);

         this.messageService.PostMessage("Welcome stranger! Prepare to perish in the Tombs of the Ancient Kings.", RogueColors.Red);
      }

      protected override Option<IGameState> OnFinished()
      {
         SaveManager.Current.SaveGame(this.gameName, this.Scene);

         Scene.PopScene();
         MyPlayingState.Current = null;

         return None;
      }

      public static new MyPlayingState Current { get; private set; }

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

      protected override Task<PlayerTurnResult> GetPlayerTurn(TimeSpan timeout)
      {
         return this.PlayerController.GetInputAsync(timeout);
      }
   }
}
