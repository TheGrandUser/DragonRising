#if INTERACTIVE
module Entities
#else
module DraconicEngineF.Entities
#endif
open CoreObjects
open System
open System.Collections.Generic
open FSharpx.Stm
open FSharpx.Extras

type EntityId = EntityId of int

type ComponentSet = ComponentSet of Dictionary<Type, obj>

type Entity = { name: string; location: Loc; id: EntityId; components: TVar<ComponentSet> }


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

let createNewEntity name (components: #seq<'T>) location =
   let valueSelector = fun comp -> comp :> obj
   let compsDict = System.Linq.Enumerable.ToDictionary<obj, Type>(components |> Seq.map valueSelector, (fun comp -> comp.GetType()))
   { name = name; location = location; id = getNextId(); components = ComponentSet compsDict |> newTVar }

let makeEntity name id l comps =
   let valueSelector = fun comp -> comp :> obj
   let compsDict = System.Linq.Enumerable.ToDictionary<obj, Type>(comps |> List.map valueSelector, (fun comp -> comp.GetType()))
   { name = name; id = EntityId id; location=l; components = ComponentSet compsDict |> newTVar }

let idToInt (EntityId id) = id