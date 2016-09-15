type Result<'s, 'e> = 
   | Success of 's
   | Error of 'e

type TargetKind = 
   | Creature
   | Allies
   | Enemies
   | Object

type TargetRange = 
   | Self
   | Touch
   | Direction
   | Range of int

type AbilityDef = 
   { range : TargetRange }

System.IO.Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__)

#r "bin/debug/FSharpx.Collections.dll"
#r "bin/debug/FSharpx.Extras.dll"

open FSharpx.Option

type GameTime = 
   | GameTime of int

type Entity = { id: int; name: string; values: Map<string, int> }

type World = 
   { entities : Entity list }

type TurnEvent = 
   | ImmediateChange of (World -> World)
   | Message of string
   | LaterChange of GameTime

type Expr = 
   | TargetValue of string
   | CasterValue of string
   | Constant of float
   | And of Expr * Expr
   | Or of Expr * Expr
   | Xor of Expr * Expr
   | Not of Expr
   | Equal of Expr * Expr
   | NotEqual of Expr * Expr
   | LessThan of Expr * Expr
   | GreaterThan of Expr * Expr
   | EqualOrLessThan of Expr * Expr
   | EqualOrGreaterThan of Expr * Expr

type EntityFilter = Entity -> bool
type EntitiesFilter = Entity -> Entity list -> bool
let allowAll e = true
let mustBeCreature e = true
let allies e = true
let enemies e = true
let isDead e = false

type TargetSelection =
   | UpTo of int
   | Exactly of int

type Interaction = {
   selectTarget: TargetRange -> EntityFilter -> Entity option
   selectTargets: TargetRange -> TargetSelection -> EntitiesFilter -> Entity list option
   selectLocation: TargetRange -> (int * int)
   }

let RevivifyDef = Touch
let getValue key e : int option = None
let currentTime w = 0
let selectTarget i constraints = None
let selectOneOrTwoTargets i range constraints = None
let RestoreLife e w = w
let DealDamage e d t w = w

let getSpellDC c = 16
let checkReflex dc t = false
let getLevel e = 1

let areAdjacent e1 e2 = true

let boostHP value e w = w

module AIUnderstanding =

   type BuffSummary = 
      | Stat of string
      | Speed
      | Offense
      | Defense

   type DebuffSummary =
      | Stat of string
      | Speed
      | Offense
      | Defense

   type CreatureEffectSummary =
      | Damage
      | Healing
      | Buff of BuffSummary
      | Debuff of DebuffSummary
   type ObjectEffectSummary =
      | Damage
      | Repair
   type TerrainEffectSummary =
      | Eases
      | Hinder
      | Passage
      | Barrier
   type AbilityTargetSummary =
      | AffectsSelf of CreatureEffectSummary list
      | AffectsAlly of CreatureEffectSummary list
      | AffectsEnemy of CreatureEffectSummary list
      | AffectsCreature of CreatureEffectSummary list
      | AffectsObject of ObjectEffectSummary list
      | AffectsTerrain of TerrainEffectSummary
      | AffectsMultiple of AbilityTargetSummary
      | AffectsBoth of AbilityTargetSummary * AbilityTargetSummary


module Spells = 
   let AcidSplash w c i =
      let entityFilter e prior =
         match prior with
         | [] -> enemies e
         | p :: _ -> enemies e && areAdjacent p e
         
      match i.selectTargets (Range 60) (UpTo 2) entityFilter with
      | Some targets -> 
         let dc = getSpellDC c
         let level = getLevel c
         let dieRoll =
            if level < 5 then "1d6"
            else if level < 11 then "2d6"
            else if level < 17 then "3d6"
            else "4d6"
         targets
         |> List.map (fun target ->
            if checkReflex dc target |> not then DealDamage target dieRoll "Acid" |> ImmediateChange
            else Message (sprintf "%s evaded the acid splash" target.name))
         |> Some
      | None -> None

   let Aid w c i =
      match i.selectTargets (Range 30) (UpTo 3) (fun e _ -> allies e) with
      | Some targets ->
         targets
         |> List.map (fun target -> ImmediateChange (boostHP 5 target))
         |> Some
      | None -> None
   
   let Revivify w c i =
      let entityFilter e =
         allies e && 
         match getValue "TimeOfDeath" e with
         | Some v -> (currentTime w) - v <= 10
         | None -> false
         
      match i.selectTarget Touch entityFilter with
      | Some target -> [ ImmediateChange <| RestoreLife target ] |> Some
      | None -> None
