#if INTERACTIVE
module InputTypes
#else
module DraconicEngineF.InputTypes
#endif

open System
open System.Threading
open System.Threading.Tasks
open System.Reactive.Linq
open System.Reactive.Threading.Tasks

type RogueKey =
   | None = 0
   | Cancel = 1
   | Backspace = 2
   | Tab = 3
   | LineFeed = 4
   | Clear = 5
   | Return = 6
   | Enter = 6
   | Pause = 7
   | CapsLock = 8
   | Capital = 8
   | HangulMode = 9
   | KanaMode = 9
   | JunjaMode = 10
   | FinalMode = 11
   | KanjiMode = 12
   | HanjaMode = 12
   | Escape = 13
   | ImeConvert = 14
   | ImeNonConvert = 15
   | ImeAccept = 16
   | ImeModeChange = 17
   | Space = 18
   | PageUp = 19
   | Prior = 19
   | PageDown = 20
   | Next = 20
   | End = 21
   | Home = 22
   | Left = 23
   | Up = 24
   | Right = 25
   | Down = 26
   | Select = 27
   | Print = 28
   | Execute = 29
   | Snapshot = 30
   | PrintScreen = 30
   | Insert = 31
   | Delete = 32
   | Help = 33
   | D0 = 34
   | D1 = 35
   | D2 = 36
   | D3 = 37
   | D4 = 38
   | D5 = 39
   | D6 = 40
   | D7 = 41
   | D8 = 42
   | D9 = 43
   | A = 44
   | B = 45
   | C = 46
   | D = 47
   | E = 48
   | F = 49
   | G = 50
   | H = 51
   | I = 52
   | J = 53
   | K = 54
   | L = 55
   | M = 56
   | N = 57
   | O = 58
   | P = 59
   | Q = 60
   | R = 61
   | S = 62
   | T = 63
   | U = 64
   | V = 65
   | W = 66
   | X = 67
   | Y = 68
   | Z = 69
   | LWin = 70
   | RWin = 71
   | Apps = 72
   | Sleep = 73
   | NumPad0 = 74
   | NumPad1 = 75
   | NumPad2 = 76
   | NumPad3 = 77
   | NumPad4 = 78
   | NumPad5 = 79
   | NumPad6 = 80
   | NumPad7 = 81
   | NumPad8 = 82
   | NumPad9 = 83
   | Multiply = 84
   | Add = 85
   | Separator = 86
   | Subtract = 87
   | Decimal = 88
   | Divide = 89
   | F1 = 90
   | F2 = 91
   | F3 = 92
   | F4 = 93
   | F5 = 94
   | F6 = 95
   | F7 = 96
   | F8 = 97
   | F9 = 98
   | F10 = 99
   | F11 = 100
   | F12 = 101
   | F13 = 102
   | F14 = 103
   | F15 = 104
   | F16 = 105
   | F17 = 106
   | F18 = 107
   | F19 = 108
   | F20 = 109
   | F21 = 110
   | F22 = 111
   | F23 = 112
   | F24 = 113
   | NumLock = 114
   | Scroll = 115
   | LeftShift = 116
   | RightShift = 117
   | LeftCtrl = 118
   | RightCtrl = 119
   | LeftAlt = 120
   | RightAlt = 121
   | BrowserBack = 122
   | BrowserForward = 123
   | BrowserRefresh = 124
   | BrowserStop = 125
   | BrowserSearch = 126
   | BrowserFavorites = 127
   | BrowserHome = 128
   | VolumeMute = 129
   | VolumeDown = 130
   | VolumeUp = 131
   | MediaNextTrack = 132
   | MediaPreviousTrack = 133
   | MediaStop = 134
   | MediaPlayPause = 135
   | LaunchMail = 136
   | SelectMedia = 137
   | LaunchApplication1 = 138
   | LaunchApplication2 = 139
   | OemSemicolon = 140
   | Oem1 = 140
   | OemPlus = 141
   | OemComma = 142
   | OemMinus = 143
   | OemPeriod = 144
   | OemQuestion = 145
   | Oem2 = 145
   | OemTilde = 146
   | Oem3 = 146
   | AbntC1 = 147
   | AbntC2 = 148
   | OemOpenBrackets = 149
   | Oem4 = 149
   | OemPipe = 150
   | Oem5 = 150
   | OemCloseBrackets = 151
   | Oem6 = 151
   | OemQuotes = 152
   | Oem7 = 152
   | Oem8 = 153
   | OemBackslash = 154
   | Oem102 = 154
   | ImeProcessed = 155
   | System = 156
   | OemAttn = 157
   | DbeAlphanumeric = 157
   | OemFinish = 158
   | DbeKatakana = 158
   | DbeHiragana = 159
   | OemCopy = 159
   | DbeSbcsChar = 160
   | OemAuto = 160
   | DbeDbcsChar = 161
   | OemEnlw = 161
   | OemBackTab = 162
   | DbeRoman = 162
   | DbeNoRoman = 163
   | Attn = 163
   | CrSel = 164
   | DbeEnterWordRegisterMode = 164
   | ExSel = 165
   | DbeEnterImeConfigureMode = 165
   | EraseEof = 166
   | DbeFlushString = 166
   | Play = 167
   | DbeCodeInput = 167
   | DbeNoCodeInput = 168
   | Zoom = 168
   | NoName = 169
   | DbeDetermineString = 169
   | DbeEnterDialogConversionMode = 170
   | Pa1 = 170
   | OemClear = 171
   | DeadCharProcessed = 172

type RogueMouseAction =
   | None = 0
   | LeftClick = 1
   | RightClick = 2
   | MiddleClick = 3
   | WheelClick = 4
   | LeftDoubleClick = 5
   | RightDoubleClick = 6
   | MiddleDoubleClick = 7
   | Movement = 8
   | WheelMove = 9

[<Flags>]
type RogueModifierKeys =
   | None = 0
   | Alt = 1
   | Control = 2
   | Shift = 4
   | Special = 8

type RogueKeyGesture = RogueKey * RogueModifierKeys
type RogueMouseGesture = RogueMouseAction * RogueModifierKeys

type KeyToDelta1D = RogueKeyGesture -> int
type KeyToDelta2D = RogueKeyGesture -> Vector

type DeltaToValue1D<'T> = int -> 'T
type DeltaToValue2D<'T> = Vector -> 'T
type LocDeltaToValue2D<'T> = (Loc * Vector) -> 'T
type LocToValue2D<'T> = Loc -> 'T

type KeyCommandGesture<'T> =
   | CommandGesture of RogueKeyGesture list * 'T
   | CommandGesture1D of RogueKeyGesture list * KeyToDelta1D * DeltaToValue1D<'T>
   | CommandGesture2D of RogueKeyGesture list * KeyToDelta2D * DeltaToValue2D<'T>

type MouseCommandGesture<'T> =
   | CommandMouseClick of RogueMouseGesture * LocToValue2D<'T>
   | CommandMouseWheel of RogueMouseGesture * DeltaToValue1D<'T>
   | CommandMouseMove of RogueMouseGesture * LocDeltaToValue2D<'T>

type InputResult<'T> =
   | InputResultSingle of 'T
   | InputResult1D of 'T * int
   | InputResultPoint of 'T * Loc
   | InputResult2D of 'T * Vector
   | InputResultLocDelta of 'T * Vector * Loc

type obs<'T> = IObservable<'T>
type InputStreams = 
   { keyDown : RogueKeyGesture obs
     mouseMove : (RogueMouseGesture * Loc * Vector) obs
     mouseClick : (RogueMouseGesture * Loc) obs
     mouseWheel : (RogueMouseGesture * Loc * int) obs }

let getGestures cg =
   match cg with
   | CommandGesture (g, _) -> g
   | CommandGesture1D (g, _, _) -> g
   | CommandGesture2D (g, _, _) -> g

let getKeyGestureReady commandGesture keyGesture =
   match commandGesture with
   | CommandGesture (_, value) -> value
   | CommandGesture1D (_, k2d, d2v) -> keyGesture |> k2d |> d2v
   | CommandGesture2D (_, k2d, d2v) -> keyGesture |> k2d |> d2v

let makeSimpleGestures keys: RogueKeyGesture list = keys |> List.map (fun k -> (k, RogueModifierKeys.None))

let fourWayMove = 
   [RogueKey.Up; RogueKey.Down; RogueKey.Left; RogueKey.Right; RogueKey.NumPad2; RogueKey.NumPad4; RogueKey.NumPad6; RogueKey.NumPad8]
   |> List.map (fun k -> (k, RogueModifierKeys.None))

let eightWayMove = 
   [RogueKey.Up; RogueKey.Down; RogueKey.Left; RogueKey.Right; RogueKey.NumPad2; RogueKey.NumPad4; RogueKey.NumPad6; RogueKey.NumPad8]
   |> List.map (fun k -> (k, RogueModifierKeys.None))

let toCycle (key, _) =
   match key with
   | RogueKey.Up
   | RogueKey.Right
   | RogueKey.NumPad8
   | RogueKey.NumPad6 -> 1
   | RogueKey.Down
   | RogueKey.Left
   | RogueKey.NumPad2
   | RogueKey.NumPad4 -> -1
   | _ -> 0

let toTask ct obs =
   TaskObservableExtensions.ToTask (obs, ct)
let obsToAsync<'T> ct (obs: IObservable<'T>) =
   obs |> Observable.FirstAsync
       |> toTask ct
       |> Async.AwaitTask

let getKeyCommandObs (streams: InputStreams) (keyGestures: KeyCommandGesture<'a> list) =
   streams.keyDown 
   |> Observable.map (fun args -> 
      keyGestures 
         |> List.toSeq
         |> Seq.map (fun cg -> 
            let keyGestures = getGestures cg
            let matchingGesture = keyGestures |> List.tryFind ((=) args)
            (cg, matchingGesture))
         |> Seq.filter (fun (_, mg) -> mg.IsSome)
         |> Seq.map (fun (cg, mg) -> getKeyGestureReady cg mg.Value )
         |> Seq.tryHead)
   |> Observable.filter (fun maybe -> maybe.IsSome)
   |> Observable.map (fun maybe -> InputResultSingle maybe.Value)

let getMouseCommandObs (streams: InputStreams) (mouseGestures: MouseCommandGesture<'a> list) =
   let staticMouseGestureCommands = 
         mouseGestures 
         |> List.filter (fun cg -> match cg with | CommandMouseClick (g, value) -> true | _ -> false)
         |> List.map (fun cg -> 
            let asSome = match cg with | CommandMouseClick (mouse, value) -> Some (mouse, value) | _ -> None
            asSome.Value)

   streams.mouseClick
   |> Observable.map (fun (args, termPoint) ->
      staticMouseGestureCommands
         |> List.toSeq
         |> Seq.filter (fun (mouseGesture, _) -> args = mouseGesture)
         |> Seq.map (fun (_, v) -> InputResultPoint (v termPoint, termPoint))
         |> Seq.tryHead)
   |> Observable.filter (fun maybe -> maybe.IsSome)
   |> Observable.map (fun maybe -> maybe.Value)

let getCommandAsync streams (keyGestures, mouseGestures, ct) = 
   let keyInputResults = getKeyCommandObs streams keyGestures
   let mouseClickInputResults = getMouseCommandObs streams mouseGestures
   async {
   let! result = Observable.Merge(keyInputResults, mouseClickInputResults) |> obsToAsync ct

   return result
   }