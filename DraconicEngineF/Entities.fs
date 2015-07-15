module DraconicEngineF.Entities
open CoreObjects
open System
open System.Collections.Generic

type EntityId = EntityId of int
type Entity = { name: string; location: Loc; id: EntityId }
type EntityFilter = Entity -> bool

type CompReply<'T> = AsyncReplyChannel<'T option>
type EnityMapMessage<'T> =
| SetComp of EntityId * 'T
| RemoveComp of EntityId
| GetComp of EntityId * CompReply<'T>

type EntityMapping<'T> = EntityMapping of MailboxProcessor<EnityMapMessage<'T>>
let private setTo (EntityMapping d) k v = SetComp (k, v) |> d.Post
let private removeFrom (EntityMapping d) k = RemoveComp k |> d.Post
//let private has (EntityMapping d) k = d.ContainsKey(k)
let private getFrom (EntityMapping d) k = d.PostAndReply (fun r -> GetComp(k, r))
let private getFromAsync (EntityMapping d) k = d.PostAndAsyncReply (fun r -> GetComp(k, r))
//let private update (EntityMapping d) k v = d.[k] <- v

let private onNext observer a =
   match observer with
   | Some o -> o a
   | None -> ()

type EntityUpdate<'T> =
| ComponentAdded of EntityId * 'T
| ComponentRemoving of EntityId * 'T
| ComponentRemoved of EntityId * 'T

let setComponent repository { id = i } c = setTo repository i c
let getComponent repository { id = i } = getFrom repository i   
let getComponentAsync repository { id = i } = getFromAsync repository i

let removeComponent repository { id = i } =
   match getFrom repository i with
   | Some c ->
      let c = getFrom repository i
      //ComponentRemoving (i, c) |> onNext observer
      removeFrom repository i
      //ComponentRemoved (i, c) |> onNext observer
      true
   | None -> false

type ListenerId = ListenerId of int
type MappingListeningMessage<'T> =
| Listen of ('T option -> unit) * ((unit -> unit) -> unit)
| Stop of ListenerId

let makeEntityPropMap<'T>() = EntityMapping (MailboxProcessor<EnityMapMessage<'T>>.Start(fun inbox ->
   let rec messageLoop entityMap = async {
         let! msg = inbox.Receive()
         let newMap =
            match msg with
            | SetComp (i, c) -> 
               entityMap |> Map.add i c
            | RemoveComp i -> entityMap |> Map.remove i
            | GetComp (i, r) ->
               entityMap |> Map.tryFind i |> r.Reply
               entityMap

         return! messageLoop newMap
   }
   
   messageLoop (Map.empty<EntityId, 'T>)
   ))