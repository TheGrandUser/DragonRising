// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

//#load "Util.fs"
//#load "CoreObjects.fs"
#r "bin/debug/FSharpx.Collections.dll"
#r "bin/debug/FSharpx.Extras.dll"
#r "bin/debug/DraconicEngineF.dll"
open DraconicEngineF.CoreObjects
open DraconicEngineF.Entities
open DraconicEngineF.ItemSelection
open FSharpx.Stm
open FSharpx.Extras
// Define your library scripting code here

type FooKey = FooKey of int
type Foo = { name: string; num: int }

let noCancel = System.Threading.CancellationToken.None

let printPair a b =
   printfn "%A - %A" a b

let stringifyPair (a, b) =
   sprintf "%A - %A" a b

let testRepo () =
   let foo1 = { name = "Foo 1"; num = 27 }
   let foo2 = { name = "Foo 2"; num = 13 }
   let fooRepo = makeRepository<FooKey, Foo> [(FooKey 1, foo1)] noCancel

   let fooMapGetter = makeMapGetter fooRepo
   let map1 = fooMapGetter()
   let setFoo = makeSetter fooRepo
   setFoo (FooKey 2) foo2
   let map2 = fooMapGetter()
   (map1, map2)

let (map1, map2) = testRepo()

let vec1 = Vector(4, 5)
let vec2 = Vector(-1, 3)
let loc1 = Loc (15, 2)
let loc2 = loc1+vec1-vec2

IsDistanceWithin loc1 loc2 10
IsDistanceWithin loc1 loc2 5
IsDistanceWithin loc1 loc2 6

let origin = Loc()

IsInRectangle origin loc1 loc2
IsInRectangle origin loc2 loc1
let loc3 = loc1 + Vector(1, 0)

AreAdjacent loc1 loc2
AreAdjacent loc1 loc3

getLineFromTo loc1 loc2

let rect1 = TerminalRect (loc1, vec1)
let (loc4, loc5) = (loc3 - vec1, loc3 + (Vector (2, 1)))
let rect2 = TerminalRect (loc4, (loc5-loc4))

Intersects rect1 rect2
Intersection rect1 rect2

let items1 = [(2, 0); (3, 2)]
[0..5] |> List.map (fun l -> fromDungeonLevel 0 l items1)

let r = System.Random()
let items2 = [(1, 0.43); (2, 0.25); (3, 0.68)]

List.init 6 (fun i -> randomUniformChoice r items2)

type creatureComp = { hp: int; }

let playerComps = [{hp = 20}] |> List.toSeq |> Seq.map (fun i -> i :> obj)
let playerLoc = Loc(3, 4)
let player = makeEntity "Player" playerComps playerLoc

let comps = getComponent<creatureComp> player