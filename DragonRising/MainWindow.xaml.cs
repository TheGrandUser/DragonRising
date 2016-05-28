using DraconicEngine;
using DraconicEngine.Input;
using DraconicEngine.WPF;
using DragonRising.Rules;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System.Windows;

namespace DragonRising
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      IMessageService messageService = new SimpleMessageService(50);
      IInputSystem inputSystem;

      DragonRisingGame game;
      IUnityContainer unityContainer = new UnityContainer();

      public MainWindow()
      {
         InitializeComponent();

         ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(this.unityContainer));

         this.inputSystem = new WpfInputSystem(this, this.terminalView);
         InputSystem.SetCurrent(this.inputSystem);
         MessageService.SetMessageService(this.messageService);

         this.unityContainer.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());

         unityContainer.RegisterType<IRulesManager, RulesManager>(new ContainerControlledLifetimeManager());
         this.unityContainer.RegisterInstance(inputSystem);
         this.unityContainer.RegisterInstance(messageService);

         this.game = new DragonRisingGame(() => this.terminalView.InvalidateVisual());
         RogueGame.SetCurrentGame(game);

         this.terminalView.Terminal = this.game.RootTerminal;

         this.Loaded += MainWindow_Loaded;
      }

      async void MainWindow_Loaded(object sender, RoutedEventArgs e)
      {
         await this.game.Start();

         this.Close();
      }
   }
}
