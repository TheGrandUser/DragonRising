module GameTypes

open Microsoft.Xna.Framework

type EntityId = EntityId of int

type Size = | Small | Medium | Large | Huge
type Race = { name: string; size: Size }

type CreatureDetails = { id: EntityId; name: string; race: Race }

type Inventory = { id: EntityId; items: EntityId list }

type WorldDescription = {
   races: Race list
   }

type Locations = {
   locById: Map<EntityId, Loc>
   idByLoc: Map<Loc, EntityId>
   }

type DisplayDetails = { id: EntityId; chixel: int; fColor: Color }

type TileType =
   | Wall
   | Floor

type Tile = { t: TileType; seen: bool }
type TileMap = { tiles: Tile[,]; width: int; height: int }

type AdventureGameState = {
   playerId: EntityId

   creatures: Map<EntityId, CreatureDetails>
   locations: Locations
   //display: Map<EntityId, DisplayDetails>

   map: TileMap
   }