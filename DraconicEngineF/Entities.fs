#if INTERACTIVE
module Entities
#else
module DraconicEngineF.Entities
#endif
open CoreTypes
open System
open System.Collections.Generic
open FSharpx.Stm
open FSharpx.Extras

type EntityId = EntityId of int

type ComponentSet = ComponentSet of Dictionary<Type, obj>

type Entity = { name: string; location: TVar<Loc>; id: EntityId; components: TVar<ComponentSet>; blocks: TVar<bool> }

let getLocation { location = l } = readTVar l |> atomically
let setLocation { location = l } newLoc = writeTVar l newLoc |> atomically

let getBlocks { blocks = l } = readTVar l |> atomically
let setBlocks { blocks = l } newLoc = writeTVar l newLoc |> atomically

let setComponent e c = 
   let {components = cs} = e
   stm {
      let! (ComponentSet cs') = readTVar cs
      cs'.[c.GetType()] <- c
      return c
   } |> atomically

let getComponent<'T> e = 
   let {components = cs} = e
   stm {
      let! (ComponentSet cs') = readTVar cs
      let (r, c) = cs'.TryGetValue(typedefof<'T>)
      return
         match (r, c) with
         | true, (:? 'T as c') -> Some c'
         | _, _ -> None
   } |> atomically

let getAllComponents e =
   let {components = cs} = e
   stm {
      let! (ComponentSet cs') = readTVar cs
      return Seq.toList cs'.Values
   } |> atomically
   
let removeComponent<'T> e =
   let {components = cs} = e
   stm {
      let! (ComponentSet cs') = readTVar cs
      let (r, c) = cs'.TryGetValue(typedefof<'T>)
      return cs'.Remove(typedefof<'T>)
   } |> atomically

type EntityFilter = Entity -> bool

let nextId = newTVar 1

let getNextId () = 
   stm {
      let! id = readTVar nextId
      do! writeTVar nextId (id+1)
      return EntityId id
   } |> atomically

let createNewEntity name blocks (components : #seq<'T>) location = 
   let valueSelector = fun comp -> comp :> obj
   let compsDict = 
      System.Linq.Enumerable.ToDictionary<obj, Type>(components |> Seq.map valueSelector, (fun comp -> comp.GetType()))
   { name = name
     location = newTVar location
     id = getNextId()
     components = ComponentSet compsDict |> newTVar
     blocks = newTVar blocks }

let makeEntity name b id l comps = 
   let valueSelector = fun comp -> comp :> obj
   let compsDict = 
      System.Linq.Enumerable.ToDictionary<obj, Type>(comps |> List.map valueSelector, (fun comp -> comp.GetType()))
   { name = name
     id = EntityId id
     location = newTVar l
     components = ComponentSet compsDict |> newTVar
     blocks = newTVar b }


let idToInt (EntityId id) = id