module main

open System
open FsXaml

type App = XAML<"App.xaml">

[<STAThread>]
[<EntryPoint>]
let main argv =
   let app = App()
   let window = new ViewModels.MainView()
   app.Run(window)