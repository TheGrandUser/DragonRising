#if INTERACTIVE
module GenerationTools
#else
module DraconicEngineF.GenerationTools
#endif

open System
open TryParser
open FSharpx.Collections

let private rand = new Random();

let grid2D width height initial =
   Array2D.create width height initial

let setGrid (grid: 'a option[,]) (r,c) (value: 'a) =
   grid.[r, c] <- Some value

let imageToGrid cellSize (x, y) = (int (x/cellSize), int (y/cellSize))

let grabRandom l =
   let i = rand.Next(List.length l)
   let (a, b) = List.splitAt i l
   match b with
   | h :: t -> (h, List.append a t)
   | [] -> failwith "Shouldn't get here: grabRandom"

let generateRandomPointAround minDist (x,y) =
   let r1 = rand.NextDouble()
   let r2 = rand.NextDouble()
   let radius = minDist * (r1+1.0)
   let angle = 2.0 * Math.PI * r2
   (x + radius + (cos angle), y + radius * (sin angle))

let generatePoisson width height minDist newPointsCount =
   let cellSize = minDist / sqrt 2.0
   let gridWidth = int (ceil width/cellSize)
   let gridHeight = int (ceil height/cellSize)
   let grid = grid2D gridWidth gridHeight None
   let toGrid = imageToGrid cellSize
   let setGrid p = setGrid grid (toGrid p) p

   let firstPoint = ((rand.NextDouble() * width), (rand.NextDouble() * height))
   setGrid firstPoint

   let pointGen = generateRandomPointAround minDist

   let squareAroundPoint (x, y) =
      let (minX, maxX) = (max 0 (x-2), min gridWidth (x+2))
      let (minY, maxY) = (max 0 (y-2), min gridHeight (y+2))
      seq {
         for r = minY to maxY do
            for c = minX to maxX do
               let gp = grid.[c, r]
               if gp.IsSome then
                  yield gp.Value
      }
   let minDistSq = minDist * minDist
   let isInRectangle (x, y) = x >= 0 && x < gridWidth && y >= 0 && y < gridHeight
   let inNeighborhood (x, y) =
      let gridPoint = toGrid (x, y)
      squareAroundPoint gridPoint |> Seq.exists (fun (x', y') -> x'*x + y'*y < minDistSq)

   
   let rec loop samplePoints processList =
      match processList with
      | [] -> (samplePoints, processList)
      | l ->
         let (point, rest) = grabRandom l
         let newPoints = List.init newPointsCount (fun i -> pointGen point) |> List.filter (fun p -> (toGrid p |> isInRectangle) && (inNeighborhood p |> not))
         loop (List.append newPoints samplePoints) (List.append newPoints rest)

   loop [] []

      

