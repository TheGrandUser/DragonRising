[<AutoOpen>]
module RogueToWPF

open DraconicEngineF.InputTypes
open DraconicEngineF.DisplayCore
open System.Runtime.InteropServices
open System.Text
open System.Windows.Input
open System.Windows.Media

module private Invokes =
   open System
   type MapType =
   | MAPVK_VK_TO_VSC = 0x0
   | MAPVK_VSC_TO_VK = 0x1
   | MAPVK_VK_TO_CHAR = 0x2
   | MAPVK_VSC_TO_VK_EX = 0x3

   [<DllImport("user32.dll")>]
   extern int ToUnicode(UInt32 wVirtKey, UInt32 wScanCode, byte[] lpKeyState,
          [<Out>][<MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4s)>] StringBuilder pwszBuff,
          int cchBuff, UInt32 wFlags)
   [<DllImport("user32.dll")>]
   extern bool GetKeyboardState(byte[] lpKeyState)
   [<DllImport("user32.dll")>]
   extern UInt32 MapVirtualKey(UInt32 uCode, MapType uMapType)


let toWpfKey (rogueKey: RogueKey) = int rogueKey |> enum<Key>
let toRogueKey (wpfKey: Key) = int wpfKey |> enum<RogueKey>
let toRogueModifiers (key: ModifierKeys) = int key |> enum<RogueModifierKeys>

let getCharFromKey key =
   let virtualKey = KeyInterop.VirtualKeyFromKey key
   let keyboardState = Array.zeroCreate 256
   Invokes.GetKeyboardState keyboardState |> ignore
   let scanCode = Invokes.MapVirtualKey ((uint32 virtualKey), Invokes.MapType.MAPVK_VK_TO_VSC)
   let stringBuilder = new StringBuilder 2
   let result = Invokes.ToUnicode(uint32 virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0u)
   match result with
   | -1
   | 0 -> None
   | _ -> Some stringBuilder.[0]

let toRogueKeyEvent (args: KeyEventArgs) =
   { RogueKeyEvent.key = toRogueKey args.Key; modifiers = toRogueModifiers Keyboard.Modifiers; character = getCharFromKey args.Key }

let toSystemColor (rc: RogueColor) =
   Color.FromRgb(rc.Red, rc.Green, rc.Blue)