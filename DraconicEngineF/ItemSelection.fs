namespace DraconicEngineF

module ItemSelection =
   open System
   open System.Collections.Generic
   let r = System.Random()
   
   type ItemWeight =
      | Uniform of float
      | ByLevel of (float * int) list

   let fromDungeonLevel<'T> def level (items: ('T * int) list) =
      let selected = items |> List.tryFindBack (fun (_, l) -> l >= level)
      match selected with
      | Some (i, _) -> i
      | None -> def


   let randomUniformChoice<'T> (items: seq<('T * float)>) =
      if Seq.isEmpty items then None
      else
         let choice = r.NextDouble() * (items |> Seq.map (fun (_, w) -> w) |> Seq.sum)
         let (start, _) = items |> Seq.head
         items
         |> Seq.scan (fun (_, lastPlace) (item, weight) -> (item, lastPlace + weight)) (start, 0.0)
         |> Seq.filter (fun (i, w) -> choice <= w)
         |> Seq.map (fun (i, _) -> i)
         |> Seq.head |> Some

   let randomChoice<'T> level (items: seq<('T * ItemWeight)>) =
      let finalizedWeights = items
                             |> Seq.map (fun (i, w) ->
                                 let weight = match w with
                                              | Uniform w -> w
                                              | ByLevel w -> fromDungeonLevel 0.0 level w
                                 (i, weight))
                             |> Seq.filter (fun (_, w) -> w > 0.0)
                             |> Seq.toList
      if Seq.isEmpty finalizedWeights then None
      else randomUniformChoice finalizedWeights