﻿using DraconicEngine.Input;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static LanguageExt.Prelude;

namespace DraconicEngine.WPF
{
   public class WpfInputSystem : IInputSystem
   {
      Window window;
      TerminalControl terminalControl;

      //Subject<KeyEventArgs> keyDownRaw = new Subject<KeyEventArgs>();
      //Subject<KeyEventArgs> keyUpRaw = new Subject<KeyEventArgs>();
      //Subject<MouseButtonEventArgs> mouseDownRaw = new Subject<MouseButtonEventArgs>();
      Subject<Tuple<RogueMouseGesture, Loc>> mouseMoveRaw = new Subject<System.Tuple<RogueMouseGesture, Loc>>();
      //Subject<MouseWheelEventArgs> mouseWheelRaw = new Subject<MouseWheelEventArgs>();



      Subject<RogueKeyEvent> keyDown = new Subject<RogueKeyEvent>();
      Subject<RogueKeyEvent> keyUp = new Subject<RogueKeyEvent>();
      Subject<Tuple<RogueMouseGesture, Loc, Vector>> mouseMove = new Subject<System.Tuple<RogueMouseGesture, Loc, Vector>>();
      Subject<Tuple<RogueMouseGesture, Loc>> mouseDown = new Subject<System.Tuple<RogueMouseGesture, Loc>>();
      Subject<Tuple<RogueMouseGesture, Loc, int>> mouseWheel = new Subject<System.Tuple<RogueMouseGesture, Loc, int>>();

      public IObservable<RogueKeyEvent> KeyDownStream => keyDown;
      public IObservable<RogueKeyEvent> KeyUpStream => keyUp;
      public IObservable<Tuple<RogueMouseGesture, Loc, Vector>> MouseMove => mouseMove;
      public IObservable<Tuple<RogueMouseGesture, Loc>> MouseDown => mouseDown;
      public IObservable<Tuple<RogueMouseGesture, Loc, int>> MouseWheel => mouseWheel;
      
      public WpfInputSystem(Window window, TerminalControl terminalControl)
      {
         this.window = window;
         this.terminalControl = terminalControl;

         window.KeyDown += Window_KeyDown;
         window.KeyUp += Window_KeyUp;
         window.MouseDown += Window_MouseDown;
         window.MouseMove += Window_MouseMove;
         window.MouseWheel += Window_MouseWheel;
         
         mouseMoveRaw.Scan(
            Tuple(new RogueMouseGesture(RogueMouseAction.Movement, RogueModifierKeys.None), new Loc(-1, -1), Vector.Zero),
            (last, next) => Tuple(next.Item1, next.Item2, next.Item2 - last.Item2))
            .Skip(1).Subscribe(mouseMove);
      }

      private void Window_KeyDown(object sender, KeyEventArgs e) => keyDown.OnNext(e.ToRogueKeyEvent());
      private void Window_KeyUp(object sender, KeyEventArgs e) => keyUp.OnNext(e.ToRogueKeyEvent());
      private void Window_MouseDown(object sender, MouseButtonEventArgs args)
      {
         var sceenPoint = args.GetPosition(this.terminalControl);
         var terminalPoint = this.terminalControl.ScreenToTerminal(sceenPoint);
         mouseDown.OnNext(Tuple(
            new RogueMouseGesture(RogueMouseAction.Movement, (RogueModifierKeys)Keyboard.Modifiers),
            terminalPoint));
      }
      private void Window_MouseMove(object sender, MouseEventArgs args)
      {
         var sceenPoint = args.GetPosition(this.terminalControl);
         var terminalPoint = this.terminalControl.ScreenToTerminal(sceenPoint);
         mouseMoveRaw.OnNext(Tuple(
            new RogueMouseGesture(RogueMouseAction.Movement, (RogueModifierKeys)Keyboard.Modifiers),
            terminalPoint));
      }
      private void Window_MouseWheel(object sender, MouseWheelEventArgs args)
      {
         var sceenPoint = args.GetPosition(this.terminalControl);
         var terminalPoint = this.terminalControl.ScreenToTerminal(sceenPoint);
         mouseWheel.OnNext(Tuple(new RogueMouseGesture(RogueMouseAction.WheelMove, (RogueModifierKeys)Keyboard.Modifiers), terminalPoint, args.Delta));
      }

      public bool IsKeyPressed(RogueKey key) => Keyboard.GetKeyStates(key.ToWpfKey()).HasFlag(KeyStates.Down);

      public async Task<RogueKeyEvent> GetKeyPressAsync() => await keyDown.FirstAsync();

      public async Task<InputResult> GetCommandAsync(IEnumerable<CommandGesture> commandGestures, CancellationToken cancelToken)
      {
         var gestures = commandGestures.ToList();

#if DEBUG
         var doubledUpGestures =
            (from g1 in gestures
             from g2 in gestures.TakeWhile(g => g != g1)
             where g1.GestureSet.MouseGesture != null && g2.GestureSet.MouseGesture != null
             where g1.GestureSet.MouseGesture.Matches(g2.GestureSet.MouseGesture)
             select new { g1, g2 }).ToArray();

         foreach (var gesturePair in doubledUpGestures)
         {
            Debug.WriteLine(gesturePair);
         }
#endif


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