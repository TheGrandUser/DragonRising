﻿using DraconicEngine;
using DraconicEngine.GameViews;
using DragonRising.GameWorld.Alligences;
using DraconicEngine.EntitySystem;
using DraconicEngine.Input;
using DraconicEngine.Terminals;
using DraconicEngine.Widgets;
using DragonRising.GameWorld.Systems;
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
using DragonRising.Rules.GameFlowRules;
using DragonRising.Rules;
using DragonRising.Rules.CombatRules;
using DragonRising.Rules.ModificationRules;
using DragonRising.Rules.InventoryRules;
using DragonRising.Rules.ExplorationRules;
using DragonRising.Rules.MagicRules;
using DragonRising.Generators;

namespace DragonRising.Views
{
   public class MyPlayingScreen : PlayingScreen
   {
      public static readonly int BarWidth = 20;
      public static readonly int PanelHeight = 9;
      public static readonly int PanelY = DragonRisingGame.ScreenHeight - PanelHeight;
      
      public static readonly int SceneHeight = DragonRisingGame.ScreenHeight - PanelHeight - 1;

      public static readonly int MessageX = BarWidth + 2;
      public static readonly int MessageWidth = DragonRisingGame.ScreenWidth - BarWidth - 3;
      public static readonly int MessageHeight = PanelHeight - 2;

      ITerminal statsPanel;
      ISaveManager saveManager;

      BarWidget hpBarWidget;
      BarWidget xpBarWidget;
      MessagesWidget messageWidget;
      HighlightWidget highlightWidget;
      MessagesWidget infoWidget;
      SceneWidget sceneWidget;
      SceneView sceneView;

      FieldOfView fieldOfView;
      
      Queue<IAsyncInterruption> interruptions = new Queue<IAsyncInterruption>();

      public World World { get; set; }

      public ITerminal ScenePanel => sceneWidget.Panel;
      public SceneView SceneView => sceneView;

      IMessageService messageService;
      private Terminal rootTerminal;
      
      string gameName;

      public PlayerController PlayerController { get; set; }

      public MyPlayingScreen(World world, ISaveManager saveManager)
      {
         this.World = world;
         this.saveManager = saveManager;

         var player = world.Player;
         this.gameName = player.Name;

         var rulesManager = ServiceLocator.Current.GetInstance<IRulesManager>();

         this.messageService = MessageService.Current;
         this.rootTerminal = DragonRisingGame.Current.RootTerminal;

         var scenePanel = rootTerminal[1, 1, DragonRisingGame.ScreenWidth - 2, SceneHeight];
         this.sceneView = new SceneView(scenePanel);

         this.fieldOfView = new FieldOfView(world);

         this.PlayerController = new PlayerController(this, messageService, fieldOfView, new SceneGenerator(), sceneView) { PlayerCreature = player };

         this.statsPanel = rootTerminal[0, PanelY, DragonRisingGame.ScreenWidth, PanelHeight];
         this.sceneWidget = new SceneWidget(world, this.sceneView, fieldOfView, scenePanel);

         this.hpBarWidget = new BarWidget(this.statsPanel[1, 1, BarWidth, 1], "HP",
            () => player.GetComponent<CombatantComponent>().HP,
            () => player.GetComponent<CombatantComponent>().MaxHP,
            RogueColors.Red, RogueColors.DarkRed);

         this.xpBarWidget = new BarWidget(this.statsPanel[1, 3, BarWidth, 1], "XP",
            () => player.GetXP(),
            () => LevelingPolicy.XpForNextLevel(player.GetLevel()?.Value ?? 0),
            RogueColors.Purple, RogueColors.DarkPurple);

         this.infoWidget = new MessagesWidget(this.statsPanel[1, 2, BarWidth, this.statsPanel.Size.Y - 3], messageService.InfoMessages, MessagePriority.ShowOldest);

         this.messageWidget = new MessagesWidget(this.statsPanel[MessageX, 1, MessageWidth, MessageHeight], messageService.EventMessages, MessagePriority.ShowNewest);

         this.highlightWidget = new HighlightWidget(this.sceneWidget.Panel[RogueColors.Black, RogueColors.LightCyan]);
         
         this.Engine.AddSystem(new ActionSystem(rulesManager), 1, SystemTrack.Game);
         this.Engine.AddSystem(new RenderSystem(scenePanel, sceneView, world), 4, SystemTrack.Render);
         this.Engine.AddSystem(new ItemRenderSystem(scenePanel, sceneView, world), 5, SystemTrack.Render);
         this.Engine.AddSystem(new CreatureRenderSystem(scenePanel, sceneView, world), 6, SystemTrack.Render);

         this.Widgets.Add(sceneWidget);
         this.Widgets.Add(hpBarWidget);
         this.Widgets.Add(xpBarWidget);
         this.Widgets.Add(messageWidget);
         this.Widgets.Add(highlightWidget);
         this.Widgets.Add(infoWidget);
         
         
         SetupRules(rulesManager);

         World.Current = World;
         this.messageService.PostMessage("Welcome stranger! Prepare to perish in the Tombs of the Ancient Kings.", RogueColors.Red);
      }

      private void SetupRules(IRulesManager rulesManager)
      {
         rulesManager.AddRule(new AddConditionRule());
         rulesManager.AddRule(new AttackRule());
         rulesManager.AddRule(new UseASpellRule());
         rulesManager.AddRule(new ConditionExpiredRule());
         rulesManager.AddRule(new DamageRule());
         rulesManager.AddRule(new DropItemRule());
         rulesManager.AddRule(new GameEndsOnPlayerDeathRule());
         rulesManager.AddRule(new ManipulateEntityRule());
         rulesManager.AddRule(new MoveToRule());
         rulesManager.AddRule(new OnConfusedStatusAddedRule());
         rulesManager.AddRule(new OnConfusedStatusRemovedRule());
         rulesManager.AddRule(new PickupItemRule());
         rulesManager.AddRule(new ReportDamageRule());
         rulesManager.AddRule(new ReportStatusAddedRule());
         rulesManager.AddRule(new ReportStatusRemovedRule());
         rulesManager.AddRule(new UseItemRule());
         rulesManager.AddRule(new UsePortalRule());
         rulesManager.AddRule(new EntityKilledRule(this.World, this));
      }

      protected override void PreSceneDraw()
      {
         this.rootTerminal.DrawBox(DrawBoxOptions.DoubleLines);

         this.statsPanel.DrawBox(DrawBoxOptions.DoubleLines);
      }

      protected override IGameView<Unit> CreateEndScreen()
      {
         return new GameEndScreen();
      }

      protected override void Save()
      {
         saveManager.SaveGame(this.gameName, this.World);
      }

      protected override void OnFinished()
      {
         World.Current = null;

         this.World.Dispose();
      }
      
      protected override EntityEngine Engine => this.World.EntityEngine;

      public void ClearHighlight() { this.highlightWidget.Enabled = false; }
      public void SetHighlight(Loc scenePoint)
      {

         this.highlightWidget.Enabled = true;
         this.highlightWidget.Location = scenePoint;
      }

      protected override bool IsGameEndState()
      {
         return !this.World.Player.GetComponent<CombatantComponent>().IsAlive;
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
