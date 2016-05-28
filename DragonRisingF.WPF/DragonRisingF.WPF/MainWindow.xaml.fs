namespace ViewModels

open System
open System.Windows
open FsXaml
open FsXaml.InjectXaml
open FsXaml.XamlTypeUtils
open FsXaml.Wpf
open FSharp.ViewModule
open FSharp.ViewModule.Validation
open DraconicEngineF
open DraconicEngineF.Game
open DraconicEngineF.Terminal
open DraconicEngineF.InputTypes
open DragonRisingF.GameRules
//open Microsoft.Practices.ServiceLocation
//open Microsoft.Practices.Unity
open System.Windows
open Akka.FSharp

type Vec = DraconicEngineF.CoreTypes.Vector

type MainViewBase = XAML<"MainWindow.xaml">

type MainView() as this =
   inherit MainViewBase()
   let terminalC = this.terminalControl :?> TerminalControl
   let inputStreams = InputSystem.getInputsFromTerminal this terminalC

   let rootTerminal = MainTerminal (Array.zeroCreate (80 * 60), new Vec(80, 60))

   //let unityContianer = new UnitContainer()
   //let messagesActorRef = ... replaces IMessageService, which has both read and write members
   //rules manager // registered RM with DI
   //register inputSystem and messenger with DI
   
   

   //let game = new DragonRisingGame(fun () -> terminalC.InvalidateVisual())
   //do RogueGame.SetCurrentGame(game)
   do terminalC.Terminal <- Some rootTerminal

   //let MainWindow_Loaded sender args = async {
   // do! game.Start()
   // this.Close() } |> Async.DoSomethingThatWillReturnUnit

   //this.Loaded += MainWindow_Loaded



type MainViewModel() as self = 
    inherit ViewModelBase()

    let name = self.Factory.Backing(<@self.Name@>, "")

    member x.Name with get() = name.Value and set value = name.Value <- value