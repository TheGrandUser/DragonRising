module InputSystem


open DraconicEngineF
open DraconicEngineF.InputTypes
open RogueToWPF
open ViewModels
open System
open System.Collections.Generic
open System.Diagnostics
open System.Linq
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Threading
open System.Threading.Tasks
open System.Windows
open System.Windows.Input
open FSharp.Control
open FSharp.Control.Reactive
open FSharp.Control.Reactive.Builders

type Vec = DraconicEngineF.CoreTypes.Vector

let getInputsFromTerminal (window: Window) (terminalControl: TerminalControl) =
   let mouseMoveRaw = new Subject<RogueMouseGesture * Loc>()

   let keyDown = new Subject<RogueKeyEvent>()
   let keyUp = new Subject<RogueKeyEvent>()
   let mouseMove = new Subject<RogueMouseGesture * Loc * Vec>()
   let mouseDown = new Subject<RogueMouseGesture * Loc>()
   let mouseWheel = new Subject<RogueMouseGesture * Loc * int>()

   let mouseToTerminal (args: MouseEventArgs) =
      let screenPoint = args.GetPosition terminalControl
      terminalControl.ScreenToTerminal screenPoint

   let subscriptions: IDisposable = 
      let keyDownSub = (window.KeyDown.Select toRogueKeyEvent).Subscribe keyDown
      let keyUpSub = (window.KeyUp.Select toRogueKeyEvent).Subscribe keyUp
      let mouseDownSub = (window.MouseDown.Select (fun args -> (RogueMouseAction.Movement, toRogueModifiers Keyboard.Modifiers), mouseToTerminal args)).Subscribe mouseDown
      let mouseMoveSub = (window.MouseMove.Select (fun args -> (RogueMouseAction.Movement, toRogueModifiers Keyboard.Modifiers), mouseToTerminal args)).Subscribe mouseMoveRaw
      let mouseWheelSub = (window.MouseWheel.Select (fun args -> (RogueMouseAction.WheelMove, toRogueModifiers Keyboard.Modifiers), mouseToTerminal args, args.Delta)).Subscribe mouseWheel

      let mouseRawToMoveSub =
         let emptyMove = ((RogueMouseAction.Movement, RogueModifierKeys.None), new Loc(-1, -1), Vec.Zero)
         (mouseMoveRaw |> Observable.scanInit emptyMove (fun (_, p, _) (g, l) -> g, l, l - p)).Skip(1)
         |> Observable.subscribe mouseMove.OnNext

      let subs = [| keyDownSub; keyUpSub; mouseDownSub; mouseMoveSub; mouseWheelSub; mouseRawToMoveSub |]
      new System.Reactive.Disposables.CompositeDisposable(subs) :> IDisposable

   let inputStreams = 
      {
       keyDown =  keyDown
       mouseMove = mouseMove
       mouseClick = mouseDown
       mouseWheel = mouseWheel }
   inputStreams, subscriptions

type WpfInputSystem (window: Window, terminalControl: TerminalControl) =
   let mouseMoveRaw = new Subject<RogueMouseGesture * Loc>()

   let keyDown = new Subject<RogueKeyEvent>()
   let keyUp = new Subject<RogueKeyEvent>()
   let mouseMove = new Subject<RogueMouseGesture * Loc * Vec>()
   let mouseDown = new Subject<RogueMouseGesture * Loc>()
   let mouseWheel = new Subject<RogueMouseGesture * Loc * int>()

   let mouseToTerminal (args: MouseEventArgs) =
      let screenPoint = args.GetPosition terminalControl
      terminalControl.ScreenToTerminal screenPoint

   let subscriptions = 
      let keyDownSub = (window.KeyDown.Select toRogueKeyEvent).Subscribe keyDown
      let keyUpSub = (window.KeyUp.Select toRogueKeyEvent).Subscribe keyUp
      let mouseDownSub = (window.MouseDown.Select (fun args -> (RogueMouseAction.Movement, toRogueModifiers Keyboard.Modifiers), mouseToTerminal args)).Subscribe mouseDown
      let mouseMoveSub = (window.MouseMove.Select (fun args -> (RogueMouseAction.Movement, toRogueModifiers Keyboard.Modifiers), mouseToTerminal args)).Subscribe mouseMoveRaw
      let mouseWheelSub = (window.MouseWheel.Select (fun args -> (RogueMouseAction.WheelMove, toRogueModifiers Keyboard.Modifiers), mouseToTerminal args, args.Delta)).Subscribe mouseWheel

      let mouseRawToMoveSub =
         let emptyMove = ((RogueMouseAction.Movement, RogueModifierKeys.None), new Loc(-1, -1), Vec.Zero)
         (mouseMoveRaw |> Observable.scanInit emptyMove (fun (_, p, _) (g, l) -> g, l, l - p)).Skip(1)
         |> Observable.subscribe mouseMove.OnNext

      let subs = [| keyDownSub; keyUpSub; mouseDownSub; mouseMoveSub; mouseWheelSub; mouseRawToMoveSub |]
      new System.Reactive.Disposables.CompositeDisposable(subs)

   let inputStreams = 
      {
       keyDown =  keyDown
       mouseMove = mouseMove
       mouseClick = mouseDown
       mouseWheel = mouseWheel }
   
   member this.KeyDownStream = keyDown
   member this.KeyUpStream = keyUp
   member this.MouseMove = mouseMove
   member this.MouseDown = mouseDown
   member this.MouseWheel = mouseWheel
   member this.InputStreams = inputStreams

   member this.IsKeyPressed key = Keyboard.GetKeyStates(toWpfKey key).HasFlag KeyStates.Down

   member this.GetKeyPresAsync() = 
      System.Reactive.Threading.Tasks.TaskObservableExtensions.ToTask(keyDown.FirstAsync()) |> Async.AwaitTask

   member this.GetCommandAsync gestures mGestures cancelToken =
      getCommandAsync this.InputStreams (gestures, mGestures, cancelToken)