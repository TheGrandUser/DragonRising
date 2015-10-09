#if INTERACTIVE
module GameRules
#else
module DragonRisingF.GameRules
#endif

open DraconicEngineF
open Game
open Entities
open DomainTypes
open DomainFunctions
open FSharpx.Stm
open System
open FSharpx.Option


let gameRandom = new Random()

let attackRule (action: AttackEntityDetails) = 
   let initComb = getComponent<CombatantComponent> action.initiator
   let initCrea = getComponent<CreatureComponent> action.initiator
   let targetComb = getComponent<CombatantComponent> action.target
   let targetCrea = getComponent<CreatureComponent> action.target
   
   if initComb.IsNone || initCrea.IsNone || targetComb.IsNone || targetCrea.IsNone then []
   else
      let roll = 1 + gameRandom.Next(20)
      let hitBy = roll + initComb.Value.power - (10 + targetComb.Value.defense)
      if hitBy > 0 then
         // message
         let damage = 
            match action.weapon with
            | Some w ->
               match getComponent<ItemComponent> w with
               | Some (Weapon (_, info)) -> info.damage
               | _ -> normalDamage 2
            | None -> normalDamage 1
         [AttackHitResult {attacker = action.initiator; target = action.target; weapon = action.weapon; damage = damage }]
      else
         [AttackMissResult { attacker = action.initiator; target = action.target; weapon = action.weapon }]
               
               
let damageRule (attack : AttackHitEvent) = 
   let target = (getComponent<CreatureComponent>attack.target).Value
   let finalDamage = attack.damage.amount
   let newHP = target.hp - finalDamage
   if newHP <= 0 then 
      let newTarget = { target with hp = newHP; isAlive = false }
      setComponent attack.target newTarget |> ignore
      [ CreatureKilled { killedCreature = attack.target
                         cause = Some "Weapon Attack"
                         killingEntity = Some attack.attacker } ]
   else 
      let newTarget = { target with hp = newHP }
      setComponent attack.target newTarget |> ignore
      [ CreatureDamage { creature = attack.target
                         damage = attack.damage
                         damagingEntity = Some attack.attacker } ]

//let entityKilledRule killedEvent = 
let reportDamageRule postMessage (world: World) (attack : AttackHitEvent): Fact list =
   if isTileVisible world.scene <| getLocation attack.target then
      let isSelf = world.scene.focusEntity = attack.target
      let prefix = if isSelf then "You suffer " else "The" + attack.target.name + " suffers "
      let message = 
         match attack.damage.kind with
         | "Normal"  -> prefix + attack.damage.amount.ToString() + " damage"
         | _ -> prefix + attack.damage.amount.ToString() + " " + attack.damage.kind + " damage"
      let color = if isSelf then RogueColors.red else RogueColors.lightBlue
      postMessage message color
   []

let moveRule (world: World) (action: MoveDetails): Fact list =
   let newLocation = getLocation action.initiator + Vector(action.direction)
   if isTileBlocked world.scene newLocation = NoBlock then
      setLocation action.initiator newLocation
   []

let dropItemRule (world: World) (action: DropItemDetails): Fact list =
   if removeFromInventory action.initiator action.itemToDrop then
      setLocation action.itemToDrop <| getLocation action.initiator
      addEntity world.scene.entityStore action.itemToDrop |> ignore
   []

let pickUpItemRule (world: World) (action: PickUpItemDetails): Fact list =
   let didPick = FSharpx.Option.maybe {
      let! inventory = getComponent<InventoryComponent> action.initiator
      let! item = getComponent<ItemComponent> action.itemToPick

      return tryAddToInventory action.initiator action.itemToPick
      }
   if didPick = (Some true) then removeEntity world.scene.entityStore action.itemToPick |> ignore
   
   []

let useItemRule (world : World) (action : UseItemDetails) : Fact list = 
   let facts = processFinalizedPlan action.finalizedPlan
   match getComponent<ItemComponent> action.item with
   | Some(Usable(info, Some charge)) -> 
      setComponent action.initiator <| Usable(info, Some { charge with charges = charge.charges - 1 }) |> ignore
      if charge.charges - 1 = 0 then 
         removeFromInventory action.initiator action.item |> ignore
         facts // :: item consumed fact?
      else facts
   | _ -> facts
   
let castSpellRule (world : World) (action: CastSpellDetails): Fact list =
   []

let addConditionRule (world : World) (action: AddConditionEvent): Fact list =
   match getComponent<ConditionComponent> action.target with
   | Some conditions when not <| List.contains action.newCondition conditions.conditions -> 
      let newConditions = action.newCondition :: conditions.conditions
      setComponent action.target {conditions = newConditions} |> ignore
      match action.duration with
      | Some d when d > 0 -> [LaterEvent (d, ConditionExpired { entity = action.target; expiringCondition = action.newCondition })]
      | _ -> []
   | _ -> []
   
let conditionExpiredRule {entity = entity; expiringCondition = ec }: Fact list =
   maybe {
      let! {conditions = c} = getComponent<ConditionComponent> entity
      let! c' = removeFrom c ec
      do setComponent entity {conditions = c'} |> ignore
      return ConditionRemoved { entity = entity; removedCondition = ec }
   } |> Option.toList

let onConfusedStatusAddedRuleFilter (world: World) (action: AddConditionEvent) = action.newCondition = Confused
let onConfusedStatusAddedRule behaviors (world : World) (action: AddConditionEvent): Fact list =
   match (getBehavior behaviors "Confused", getComponent<BehaviorComponent> action.target) with
   | (Some confusedBehavior, Some (BehaviorComponent currentBehaviors)) ->
      let behavior = confusedBehavior action.target
      setComponent action.target (BehaviorComponent (("Confused", behavior) :: currentBehaviors)) |> ignore
   | (_, _) -> ()
   []
   
let onConfusedStatusRemovedRuleFilter (world : World) (action: ConditionRemovedEvent) = action.removedCondition = Confused
let onConfusedStatusRemovedRule behaviors (world : World) (action: ConditionRemovedEvent): Fact list =
   maybe {
      let! (BehaviorComponent behaviors) = getComponent<BehaviorComponent> action.entity
      let behavior = List.find (fun (name, b) -> name = "Confused") behaviors
      let! behaviors' = removeFrom behaviors behavior
      do snd behavior |> stopBehavior
      do setComponent action.entity (BehaviorComponent behaviors') |> ignore
      return ()
   } |> ignore
   []
   
let creatureKilledRule (world: World) { killedCreature = killed } =
   match getComponent<DrawnComponent> killed with
   | Some drawn -> setComponent killed { drawn with seen = makeForeOnlyChar Glyph.Percent RogueColors.darkRed } |> ignore
   | None -> ()
   clearAnyBehaviors killed
   removeComponent<BehaviorComponent> killed |> ignore
   removeComponent<CreatureComponent> killed |> ignore
   sendToBack world.scene killed
   writeTVar killed.blocks false |> atomically
   []

let gameEndsOnPlayerDeathRuleFilter (world: World) (e: CreatureKilledEvent) = world.scene.focusEntity = e.killedCreature
let gameEndsOnPlayerDeathRule postMessage (e: CreatureKilledEvent) =
   postMessage "You have died!" RogueColors.red
   

let awardXPForKillRuleFilter (world : World) { killingEntity = ke } = ke = Some world.scene.focusEntity
let awardXPForKillRule postMessage (world : World) { killedCreature = killed; killingEntity = k } : Fact list =
   let killer = k.Value
   let creature = (getComponent<CreatureComponent> killed).Value
   postMessage (killed.name + " is dead! You gain " + creature.xp.ToString() + " experience points") RogueColors.orange
   []