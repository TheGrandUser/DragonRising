module WorldGeneration

open TinkerWorX.AccidentalNoiseLibrary
open GameTypes
open FSharpx

let private worldGenRandom = new System.Random()

let createRegions width height =
   //let gen = SadConsole.Game.WorldGeneration.WorldGenerator
   let seed = worldGenRandom.Next();
   //TinkerWorX.AccidentalNoiseLibrary.Noise.SimplexNoise(
   let heights = Array2D.init width height (fun x y -> 0.2 + 0.6 * Noise.SimplexNoise(float x * 6.0, float y * 6.0, seed, (fun x -> x)), worldGenRandom.Next())
   heights

let private chooseTerrain terrainHeights min max x y = 
   let height = Array2D.get terrainHeights x y
   let range = max - min
   let downSlopePoint = min - range * 0.2
   let upSlopePoint = max - range * 0.2
   let tileType = 
      if height < downSlopePoint then OpenSpace 
      elif height < min then DownSlope
      elif height < upSlopePoint then Floor
      elif height < max then UpSlope
      else Wall
   { t = tileType; seen = tileType <> Wall }

let private makeLayer terrainHeights width height min max =
   let tiles = Array2D.init width height (chooseTerrain terrainHeights min max)
   tiles

let createLocalTerrain (mainRegion: (float * int) [,]) x y width height=
   
   let regionHeight, seed = mainRegion.[x,y]

   let regionHeights = Array2D.init width height (fun x y -> regionHeight - 0.2 + 0.4 * Noise.SimplexNoise(float x * 0.06, float y * 0.06, seed, (fun x -> x)))

   let layer1 = makeLayer regionHeights width height 0.0 0.2
   let layer2 = makeLayer regionHeights width height 0.2 0.4
   let layer3 = makeLayer regionHeights width height 0.4 0.6
   let layer4 = makeLayer regionHeights width height 0.6 0.8
   let layer5 = makeLayer regionHeights width height 0.8 1.0

   { tiles = [layer1; layer2; layer3; layer4; layer5]; width = width; height = height }