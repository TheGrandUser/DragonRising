module GameTypes

open Microsoft.Xna.Framework

type EntityId = EntityId of int

type Size = | Small | Medium | Large | Huge
type Race = { name: string; size: Size }

type CreatureDetails = { id: EntityId; name: string; race: Race }

type Inventory = { id: EntityId; items: EntityId list }

module RegionTypes =
   type WorldCoord = WorldCoord of Loc // single cell on the world gen which has the primary biome type
   type TerrainPoint = { location: Loc; height: float }
   type Edge = { point1: WorldCoord * int; point2: WorldCoord * int }
   type Plant = string * Loc

   type PointStep = { points: TerrainPoint list; worldCoord: WorldCoord } // generate a bunch of points
   type PointsStep = Map<WorldCoord, PointStep>
   type EdgesStep = { edges: Edge list; worldCoord: WorldCoord } // connect the points
   type MajorRiversStep = { rivers: (TerrainPoint * int) list list } // 
   type CellHeightStep = { heightMap: float [,]; worldCoord: WorldCoord } // given the edges, and thus polygons, fill out the cells with gradient heights, possibly also noise heights
   type StreamsAndPlantsStep = { streams: TerrainPoint list list; plants: Plant list } // 


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