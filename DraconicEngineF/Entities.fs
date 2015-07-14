module DraconicEngineF.Entities
open CoreObjects
open System
open System.Collections.Generic

type EntityId = EntityId of int
type Entity = { name: string; location: Loc; id: EntityId }

type byEntity<'T> = Dictionary<EntityId, 'T>
let private addTo (d: byEntity<'T>) k v = d.Add(k, v)
let private removeFrom (d: byEntity<'T>) k = d.Remove(k)
let private has (d: byEntity<'T>) k = d.ContainsKey(k)
let private getFrom (d: byEntity<'T>) k = d.[k]
let private update (d: byEntity<'T>) k v = d.[k] <- v

let private onNext observer a =
   match observer with
   | Some o -> o a
   | None -> ()

type EntityUpdate<'T> =
| ComponentAdded of EntityId * 'T
| ComponentRemoving of EntityId * 'T
| ComponentRemoved of EntityId * 'T

let addComponent repository observer {id=i} c =
   addTo repository i c
   ComponentAdded (i, c) |> onNext observer
let entityHas repository { id = i } = has repository i

let removeComponent repository observer {id=i} c =
   if has repository i then
      let c = getFrom repository i
      ComponentRemoving (i, c) |> onNext observer
      let result = repository.Remove(i)
      ComponentRemoved (i, c) |> onNext observer
      result
   else false

let getForEntity repository { id = i } = getFrom repository i
let updateEntity repository { id = i } c = update repository i c
