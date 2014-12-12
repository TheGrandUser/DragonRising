using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DraconicEngine;
using DraconicEngine.GameWorld.EntitySystem;
using DraconicEngine.GameStates;
using DraconicEngine.Input;
using DraconicEngine.Widgets;
using DraconicEngine.Terminals;
using DragonRising.Generators;
using DraconicEngine.Storage;
using DraconicEngine.GameWorld.EntitySystem.Components;
using DragonRising.Services;
using DraconicEngine.GameWorld.Alligences;
using LanguageExt;
using LanguageExt.Prelude;

namespace DragonRising.GameStates
{
   class MyPlayingState : PlayingState
   {
      static readonly int MapWidth = 160;
      static readonly int MapHeight = 80;

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

      public static MyPlayingState CreateNew()
      {
         Scene scene = new Scene(MapWidth, MapHeight);

         var generator = new DungeonGenerator(new GreenskinsGenerator(), new StandardItemGenerator());
         var startPoint = generator.MakeMap(scene);
         var playerAlligence = new Alligence() { Name = "Player" };

         var playerTemplate = new EntityTemplate("Player", Glyph.At, RogueColors.White,
            new CombatantComponentTemplate(hp: 30, defense: 2, power: 5),
            new CreatureComponentTemplate(playerAlligence) { VisionRadius = 6 },
            new DecisionComponentTemplate(),
            new InventoryComponentTemplate(26))
         {
            Blocks = true
         };

         Entity player = playerTemplate.Create();
         player.Location = startPoint;

         var inventory = player.GetComponent<InventoryComponent>();

         inventory.Items.Add(Library.Items.Get(TempConstants.ScrollOfLightningBolt).Create());
         inventory.Items.Add(Library.Items.Get(TempConstants.ScrollOfFireball).Create());
         inventory.Items.Add(Library.Items.Get(TempConstants.ScrollOfConfusion).Create());

         var playingState = new MyPlayingState(scene, player);

         return playingState;
      }

      public new PlayerController PlayerController
      {
         get { return (PlayerController)base.PlayerController; }
         set { base.PlayerController = value; }
      }

      public MyPlayingState(Scene scene, Entity player)
         : base(scene)
      {
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

         this.Widgets.Add(sceneWidget);
         this.Widgets.Add(this.hpBarWidget);
         this.Widgets.Add(this.messageWidget);
         this.Widgets.Add(this.highlightWidget);
         this.Widgets.Add(this.infoWidget);

         this.PlayerController = new PlayerController(sceneView) { PlayerCreature = player };
         this.lifeDeathMonitorService = new LifeDeathMonitorService(scene.EntityStore);

         this.Scene.EntityStore.Add(player);

         this.Scene.EntityStore.AllEntities.Single(e => e == player);
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

         this.Scene.FocusEntity = this.PlayerController.PlayerCreature;
         Scene.PushScene(this.Scene);

         this.messageService.PostMessage("Welcome stranger! Prepare to perish in the Tombs of the Ancient Kings.", RogueColors.Red);
      }

      protected override Option<IGameState> OnFinished()
      {
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

      public static MyPlayingState Load(string filePath)
      {
         throw new NotImplementedException();
      }

      protected override bool IsGameEndState()
      {
         return this.lifeDeathMonitorService.HasPlayerLost;
      }
   }
}
