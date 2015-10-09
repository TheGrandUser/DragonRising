module DataAccess

open DragonRisingF.DomainTypes

type ISaveManager =
   abstract member SaveGame: string -> World -> Async<unit>
   abstract member LoadGame: string -> Async<World>
   abstract member SavedGames: seq<string>
   abstract member LastSavedGame: string option


let saveGame (sm: ISaveManager) = sm.SaveGame
let loadGame (sm: ISaveManager) = sm.LoadGame
let getSavedGames (sm: ISaveManager) = sm.SavedGames
let lastSavedGame (sm: ISaveManager) = sm.LastSavedGame