// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
System.IO.Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__)

#r "bin/debug/DraconicEngineF.dll"
#r "bin/debug/FSharpx.Collections.dll"
#r "bin/debug/FSharpx.Extras.dll"

#load "DomainTypes.fs"
#load "DomainFunctions.fs"

open DraconicEngineF
open DraconicEngineF.Entities

open DomainTypes
open DomainFunctions
// Define your library scripting code here

let getIntStat (name: string) (entity: Entity) = Some 0

let applyTemporaryCondition (effectId: EffectId) condition duration (user: Entity) = 
   EntityEffect (effectId, fun target -> [ConditionAddedFact { target = target; newCondition = condition; duration = Some duration }])
//let damageEffect 

let useIntStat statName (statToValue: int -> int) (effect: int -> Entity -> Effect) (goesWrong: PowerFailure) user =
   match getIntStat statName user with
   | Some s -> effect (statToValue s) user
   | None -> goesWrong ("No stat" + statName, user, IntToEffect effect)
   

let confusedFor6Id = 1
let confusedFor6: (Entity -> Effect) = applyTemporaryCondition confusedFor6Id Confused 6

let confusedForThirdOfCharisma = //: (Entity -> EntityEffect) =
   useIntStat "Charisma" (fun c -> c/3) (applyTemporaryCondition confusedFor6Id Confused)