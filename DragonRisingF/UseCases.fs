module DragonRisingF.UseCases
open DraconicEngineF.CoreObjects
open DraconicEngineF.Entities
open DragonRisingF.DomainTypes
open DragonRisingF.UITypes

let all _ = true
let asAsync<'T> t = async { return t }

let turnAction a = (Some a, Some TurnAdvancing)
let noTurn = (None, None)
let endGame = (None, Some FinalTurn)

let selectEntityInRange filter range player: Async<Entity option> = async { return None }
let selectItemFromInventory itemFilter player: Async<Entity option> =
   //let inventory 
   async { return None }

let getCommandAction player command = 
   match command with
   | WaitCommand -> turnAction IdleAction |> asAsync
   | QuitCommand -> endGame |> asAsync
   | Abort -> noTurn |> asAsync
   | MovementCommand dir -> turnAction (MoveAction { direction = dir }) |> asAsync
   | InteractWithCreatureCommand e -> noTurn |> asAsync
   | AttackCommand e -> turnAction (AttackEntityAction { target=e; weapon = None}) |> asAsync
   | DropItemCommand -> async {
      let! item = selectItemFromInventory all player
      return match item with
               | Some i -> turnAction (PickUpItemAction { itemToPick = i })
               | None -> noTurn
      }
   | GoDownCommand -> noTurn |> asAsync
   | LookCommand -> noTurn |> asAsync
   | ManipulateCommand e -> noTurn |> asAsync
   | PickUpItemCommand -> noTurn |> asAsync
   | UseItemCommand itemFilter -> noTurn |> asAsync