using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DraconicEngine.Input;
using KeyGesture = DraconicEngine.Input.RogueKeyGesture;
using System.Collections.Immutable;

namespace DraconicEngine.WPF
{
   public class WpfInputSystem : IInputSystem
   {
      abstract class Request { }

      class KeyRequest : Request
      {
         public TaskCompletionSource<RogueKeyEvent> Tcs { get; private set; }

         public KeyRequest(TaskCompletionSource<RogueKeyEvent> tcs)
         {
            this.Tcs = tcs;
         }
      }

      class GestureRequest : Request
      {
         private TaskCompletionSource<InputResult> currentTcs;
         public IReadOnlyList<CommandGesture> CommandGestures { get; private set; }

         public GestureRequest(IReadOnlyList<CommandGesture> commandGestures, TaskCompletionSource<InputResult> tcs)
         {
            this.CommandGestures = commandGestures;
            this.currentTcs = tcs;
         }

         public void Do(CommandGesture toDo, RogueKeyGesture keyGesture)
         {
            if (currentTcs == null)
            {
               return;
            }

            if (toDo is CommandGestureSingle)
            {
               var value = ((CommandGestureSingle)toDo).GetPackage(keyGesture);

               var tcs = currentTcs;
               currentTcs = null;
               tcs.SetResult(new InputResult(value));
            }
            else if (toDo is CommandGesture1D)
            {
               var toDo1D = ((CommandGesture1D)toDo);
               var value = toDo1D.GetPackage(keyGesture);
               var delta = toDo1D.GetValue(keyGesture);
               var tcs = currentTcs;
               currentTcs = null;
               tcs.SetResult(new InputResult1D(value, delta));
            }
            else if (toDo is CommandGesture2D)
            {
               var toDo2D = ((CommandGesture2D)toDo);
               var delta = toDo2D.GetDirectionFromKeys(keyGesture);
               var value = toDo2D.GetPackage(null, delta);
               var tcs = currentTcs;
               currentTcs = null;
               tcs.SetResult(new InputResult2D(value, null,  delta));
            }
         }

         public void Do(CommandGestureSingle toDo, RogueMouseGesture mouseGesture)
         {
            if (currentTcs == null)
            {
               return;
            }
            var value = toDo.GetPackage(mouseGesture);

            var tcs = currentTcs;
            currentTcs = null;
            tcs.SetResult(new InputResult(value));
         }

         public void Do(CommandGesture1D toDo, int wheelDelta)
         {
            if (currentTcs == null)
            {
               return;
            }
            var value = toDo.GetPackage(wheelDelta);

            var tcs = currentTcs;
            currentTcs = null;
            tcs.SetResult(new InputResult1D(value, wheelDelta));
         }

         public void Do(CommandGesture2D toDo, Loc point, Vector delta)
         {
            if (currentTcs == null)
            {
               return;
            }
            var value = toDo.GetPackage(point, delta);

            var tcs = currentTcs;
            currentTcs = null;
            tcs.SetResult(new InputResult2D(value, point, delta));
         }
      }

      Window window;
      TerminalControl terminalControl;

      Stack<Request> requests = new Stack<Request>();

      Loc lastTerminalPoint = new Loc(-1, -1);

      public WpfInputSystem(Window window, TerminalControl terminalControl)
      {
         this.window = window;
         this.terminalControl = terminalControl;

         this.window.KeyDown += window_KeyDown;

         this.terminalControl.MouseDown += terminalControl_MouseDown;

         this.terminalControl.MouseMove += terminalControl_MouseMove;

         this.terminalControl.MouseWheel += terminalControl_MouseWheel;
      }

      public bool IsKeyPressed(RogueKey key)
      {
         return Keyboard.GetKeyStates(key.ToWpfKey()).HasFlag(KeyStates.Down);
      }

      void terminalControl_MouseWheel(object sender, MouseWheelEventArgs e)
      {
         if (this.requests.Count > 0)
         {
            var request = this.requests.Peek();

            if (request is GestureRequest)
            {
               var gestureRequest = (GestureRequest)request;

               var mouseGesture = new DraconicEngine.Input.RogueMouseGesture(
                     DraconicEngine.Input.RogueMouseAction.WheelMove,
                     (RogueModifierKeys)Keyboard.Modifiers);

               var commandGesture = gestureRequest.CommandGestures.OfType<CommandGesture1D>().SingleOrDefault(d => mouseGesture.Matches(d.MouseGesture));

               if (commandGesture != null)
               {
                  gestureRequest.Do(commandGesture, e.Delta);
               }
            }
         }
      }

      void terminalControl_MouseMove(object sender, MouseEventArgs e)
      {
         if (this.requests.Count > 0)
         {
            var sceenPoint = e.GetPosition(this.terminalControl);
            var terminalPoint = this.terminalControl.ScreenToTerminal(sceenPoint);

            if (terminalPoint != lastTerminalPoint)
            {
               var request = this.requests.Peek();

               if (request is GestureRequest)
               {
                  
                  var gestureRequest = (GestureRequest)request;

                  var mouseGesture = new RogueMouseGesture(
                        RogueMouseAction.Movement,
                        (RogueModifierKeys)Keyboard.Modifiers);

                  var commandGesture = gestureRequest.CommandGestures.OfType<CommandGesture2D>().SingleOrDefault(d => mouseGesture.Matches(d.MouseGesture));

                  if (commandGesture != null)
                  {
                     requests.Pop();
                     gestureRequest.Do(commandGesture, terminalPoint, terminalPoint - lastTerminalPoint);
                     lastTerminalPoint = terminalPoint;
                  }
               }
            }
         }
      }

      void terminalControl_MouseDown(object sender, MouseButtonEventArgs e)
      {
         if (this.requests.Count > 0)
         {
            var request = this.requests.Peek();

            if (request is GestureRequest)
            {
               //var sceenPoint = e.GetPosition(this.terminalControl);
               //var terminalPoint = this.terminalControl.ScreenToTerminal(sceenPoint);

               var gestureRequest = (GestureRequest)request;

               var mouseGesture = new RogueMouseGesture(
                  (RogueMouseAction)e.ChangedButton,
                  (RogueModifierKeys)Keyboard.Modifiers);
               
               var gesture = gestureRequest.CommandGestures.OfType<CommandGestureSingle>().SingleOrDefault(d => mouseGesture.Matches(d.MouseGesture));

               if (gesture != null)
               {
                  gestureRequest.Do(gesture, mouseGesture);
               }
            }
         }
      }

      void window_KeyDown(object sender, KeyEventArgs e)
      {
         if (requests.Count > 0)
         {
            var request = requests.Peek();
            if (request is GestureRequest)
            {
               var gestureRequest = (GestureRequest)request;

               var rogueKeyEvent = e.ToRogueKeyEvent();

               foreach (var gesture in gestureRequest.CommandGestures)
               {
                  var matchingGesture = gesture.KeyGestures.FirstOrDefault(kg => kg.Matches(rogueKeyEvent));
                  if (matchingGesture != null)
                  {
                     requests.Pop();

                     gestureRequest.Do(gesture, matchingGesture);
                     return;
                  }
               }
            }
            else
            {
               requests.Pop();
               var keyRequest = (KeyRequest)request;

               keyRequest.Tcs.SetResult(e.ToRogueKeyEvent());
            }
         }
      }

      public Task<RogueKeyEvent> GetKeyPressAsync()
      {
         var inputRequest = new TaskCompletionSource<RogueKeyEvent>();

         this.requests.Push(new KeyRequest(inputRequest));

         return inputRequest.Task;
      }

      public Task<InputResult> GetCommandAsync(IEnumerable<Input.CommandGesture> gestures)
      {
         var tcs = new TaskCompletionSource<InputResult>();
         
         var gestureRequest = new GestureRequest(gestures.ToList(), tcs);

         this.requests.Push(gestureRequest);

         return tcs.Task;
      }
   }
}