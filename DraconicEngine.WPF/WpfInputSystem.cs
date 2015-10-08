using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DraconicEngine.Input;
using System.Threading;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace DraconicEngine.WPF
{
   public class WpfInputSystem : IInputSystem
   {
      Window window;
      TerminalControl terminalControl;

      IObservable<RogueKeyEvent> keyDown;
      Subject<Tuple<RogueMouseGesture, Loc, Vector>> mouseMove;
      IObservable<Tuple<RogueMouseGesture, Loc>> mouseDown;
      IObservable<Tuple<RogueMouseGesture, Loc, int>> mouseWheel;

      class testClass
      {
         public void M() { }
      }

      public WpfInputSystem(Window window, TerminalControl terminalControl)
      {
         this.window = window;
         this.terminalControl = terminalControl;

         this.keyDown = Observable.FromEvent<KeyEventHandler, KeyEventArgs>(h => this.window.KeyDown += h, h => this.window.KeyDown -= h).Select(args => args.ToRogueKeyEvent());
         this.mouseDown =
            from args in Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(h => this.window.MouseDown += h, h => this.window.MouseDown -= h)
            let sceenPoint = args.GetPosition(this.terminalControl)
            let terminalPoint = this.terminalControl.ScreenToTerminal(sceenPoint)
            select Tuple.Create(new RogueMouseGesture((RogueMouseAction)args.ChangedButton, (RogueModifierKeys)Keyboard.Modifiers), terminalPoint);
         
         var mouseMove =
            from args in Observable.FromEvent<MouseEventHandler, MouseEventArgs>(h => this.window.MouseMove += h, h => this.window.MouseMove -= h)
            let sceenPoint = args.GetPosition(this.terminalControl)
            let terminalPoint = this.terminalControl.ScreenToTerminal(sceenPoint)
            select Tuple.Create(new RogueMouseGesture(RogueMouseAction.Movement, (RogueModifierKeys)Keyboard.Modifiers), terminalPoint);

         this.mouseMove = new Subject<Tuple<RogueMouseGesture, Loc, Vector>>();
         mouseMove.Scan(
            Tuple.Create(new RogueMouseGesture(RogueMouseAction.Movement, RogueModifierKeys.None), new Loc(-1, -1), Vector.Zero),
            (last, next) => Tuple.Create(next.Item1, next.Item2, next.Item2 - last.Item2))
            .Skip(1).Subscribe(this.mouseMove);

         this.mouseWheel =
            from args in Observable.FromEvent<MouseWheelEventHandler, MouseWheelEventArgs>(h => this.window.MouseWheel += h, h => this.window.MouseWheel -= h)
            let sceenPoint = args.GetPosition(this.terminalControl)
            let terminalPoint = this.terminalControl.ScreenToTerminal(sceenPoint)
            select Tuple.Create(new RogueMouseGesture(RogueMouseAction.WheelMove, (RogueModifierKeys)Keyboard.Modifiers), terminalPoint, args.Delta);
      }

      public bool IsKeyPressed(RogueKey key) => Keyboard.GetKeyStates(key.ToWpfKey()).HasFlag(KeyStates.Down);
      
      public async Task<RogueKeyEvent> GetKeyPressAsync() => await keyDown.FirstAsync();

      public async Task<InputResult> GetCommandAsync(IEnumerable<CommandGesture> commandGestures, CancellationToken cancelToken)
      {
         var gestures = commandGestures.ToList();

         var keyInputResults = 
            from args in keyDown
            from gesture in gestures
            let matchingGesture = gesture.GestureSet.KeyGestures.FirstOrDefault(kg => kg.Matches(args))
            where matchingGesture != null
            select GetKeyGestureReady(gesture, matchingGesture);

         var mouseDownInputResults =
            from args in mouseDown
            let gesture = gestures.OfType<CommandGestureSingle>().SingleOrDefault(d => args.Item1.Matches(d.GestureSet.MouseGesture))
            where gesture != null
            select new InputResult(gesture.Command);

         var mouseMoveInputResults =
            from args in mouseMove
            let gesture = gestures.OfType<CommandGesture2D>().SingleOrDefault(d => args.Item1.Matches(d.GestureSet.MouseGesture))
            where gesture != null
            select new InputResult2D(gesture.GetValue(args.Item2, args.Item3), args.Item2, args.Item3);

         var mouseWheelInputResults =
            from args in mouseWheel
            let gesture = gestures.OfType<CommandGesture1D>().SingleOrDefault(d => args.Item1.Matches(d.GestureSet.MouseGesture))
            where gesture != null
            select new InputResult1D(gesture.GetValue(args.Item3), args.Item3);

         var obs = Observable.Merge(keyInputResults, mouseDownInputResults, mouseMoveInputResults, mouseWheelInputResults);
         var firstObs = obs.FirstOrDefaultAsync();
         //var commandTask = firstObs.ToTask(cancelToken);

         return await firstObs;
      }

      static InputResult GetKeyGestureReady(CommandGesture toDo, RogueKeyGesture keyGesture)
      {
         if (toDo is CommandGestureSingle)
         {
            var value = ((CommandGestureSingle)toDo).Command;
            return new InputResult(value);
         }
         else if (toDo is CommandGesture1D)
         {
            var toDo1D = ((CommandGesture1D)toDo);
            var delta = toDo1D.KeyToDelta(keyGesture);
            var command = toDo1D.GetValue(delta);
            return new InputResult1D(command, delta);
         }
         else if (toDo is CommandGesture2D)
         {
            var toDo2D = ((CommandGesture2D)toDo);
            var delta = toDo2D.KeyToDelta(keyGesture);
            var value = toDo2D.GetValue(null, delta);
            return new InputResult2D(value, null, delta);
         }
         throw new ArgumentException();
      }
   }
}