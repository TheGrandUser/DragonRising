System.IO.Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__)

#r "bin/debug/FSharpx.Collections.dll"
#r "bin/debug/FSharpx.Extras.dll"

#r "bin/debug/Newtonsoft.Json.dll"
#r "bin/debug/FsPickler.dll"
#r "bin/debug/FsPickler.Json.dll"

//#r "bin/debug/DraconicEngineF.dll"

#load "Util.fs"
#load "DisplayCore.fs"
#load "CoreTypes.fs"
#load "Entities.fs"

open Entities
open Nessos.FsPickler
open Nessos.FsPickler.Json
open Nessos.FsPickler.Combinators
open FSharpx.Stm
open System
open FSharpx.Collections

type IComponent = interface end
type CreatureComponent = { visionRadius: int } with interface IComponent end
type DrawnComponent = { seen: Character; explored: Character option } with interface IComponent end
type WithFuncComponent = { doStuff: (int -> int); info: string } with interface IComponent end
   
let something x = x * 2
let playerGlyph = { glyph = Glyph.At; color = { foreColor=RogueColors.white; backColor = None } }
let player = createNewEntity "Player" true ([ { visionRadius=6 } :> IComponent; { seen=playerGlyph; explored=None} :> IComponent; { doStuff = something; info = "blahblah"} :> IComponent]) (Loc(2, 3))

let makeTVarPickler (tp: Pickler<'T>) tag = 
   let reader s = tp.Read s tag |> newTVar
   let writer s a =
      let value = stm { return! readTVar a } |> atomically
      tp.Write s tag value
   Pickler.FromPrimitives(reader, writer)
//
//let makeEntity name id l comps =
//   let valueSelector = fun comp -> comp :> obj
//   let compsDict = System.Linq.Enumerable.ToDictionary<obj, Type>(comps |> List.map valueSelector, (fun comp -> comp.GetType()))
//   { name = name; id = EntityId id; location=l; components = ComponentSet compsDict |> newTVar }

let componentWrapper<'T when 'T :> IComponent> p = p |> Pickler.wrap (fun c -> c :> IComponent) (fun i -> i :?> 'T)

let creaturePickler = Pickler.auto<CreatureComponent> |> componentWrapper
let drawnPickler =  Pickler.auto<DrawnComponent> |> componentWrapper
let withFuncPickler = Pickler.auto<WithFuncComponent> |> componentWrapper

let componentPickler =
   let indexPicker (c: IComponent) =
      match c with
      | :? CreatureComponent as cc -> 0
      | :? DrawnComponent as dc -> 1
      | _ -> -1
   Pickler.alt indexPicker [creaturePickler; drawnPickler]
      

let makeEntityPickler compPickler =
   Pickler.product makeEntity
   ^+ Pickler.field (fun e -> e.name) Pickler.string
   ^+ Pickler.field (fun e -> getBlocks e) Pickler.bool
   ^+ Pickler.field (fun e -> e.id |> idToInt) Pickler.int
   ^+ Pickler.field (fun e -> getLocation e) Pickler.auto<Loc>
   ^. Pickler.field (fun e -> getAllComponents e) (Pickler.list compPickler)

      
let entityFactory =
   { new IPicklerFactory<Entity> with
      member __.Create (resolver:IPicklerResolver)=
         let objP = resolver.Resolve<obj>()
         makeEntityPickler objP
   }

FsPickler.RegisterPicklerFactory entityFactory

let entityPickler = makeEntityPickler Pickler.auto<obj>

let binary = FsPickler.CreateBinarySerializer()
let binaryResult = binary.Pickle (player)
let entityFromBinary: Entity = binary.UnPickle (binaryResult)

let json = FsPickler.CreateJsonSerializer(indent = false)
let jsonResult = json.PickleToString(player)
let loadedEntity: Entity = json.UnPickleOfString(jsonResult)

let comps = getAllComponents entityFromBinary
let withFunc = getComponent<WithFuncComponent> entityFromBinary
withFunc.Value.doStuff 3

binaryResult.Length
jsonResult.Length