#if INTERACTIVE
module RuleManager
#else
module DragonRisingF.RuleManager
#endif

open DraconicEngineF
open Game
open Entities
open DomainTypes
open DomainFunctions
open FSharpx.Stm
open System
open FSharpx.Option
open GameRules
open FSharpx.Collections
open WorldState

type Rule = | Rule of (World -> Fact -> bool) * (World -> Fact -> (bool * Fact list)) * int

let processFact (rules: Rule list) world fact =
   let rec loop facts = 
      let (fact', rest) = Queue.uncons facts
      let nextQueue = 
         rules 
         |> List.filter (fun (Rule (f, _, _)) -> f world fact')
         |> List.map (fun (Rule (_, r, _)) -> r world fact')
         |> List.takeUntil (fun (i, _) -> i)
         |> List.collect (fun (_, f) -> f)
         |> List.fold (fun q f -> Queue.conj f q) rest
      if not nextQueue.IsEmpty then loop nextQueue

   Queue.ofList [fact] |> loop