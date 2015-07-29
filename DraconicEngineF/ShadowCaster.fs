#if INTERACTIVE
module ShadowCaster
#else
module DraconicEngineF.ShadowCaster
#endif

open DraconicEngineF.CoreObjects

type ColumnPortion = 
   { x : int
     bottomVector : Vector
     topVector : Vector }

let translateOrigin f x y a b = f (a + x) (b + y)

let translateOctant f octant = 
   match octant with
   | 0 -> f
   | 1-> fun x y -> f y x
   | 2 -> fun x y -> f -y x
   | 3 -> fun x y -> f -x y
   | 4 -> fun x y -> f -x -y
   | 5 -> fun x y -> f -y -x
   | 6 -> fun x y -> f y -x
   | 7 -> fun x y -> f x -y
   | _ -> f

let square x = x * x
let isInRadius x y radius = square (2 * x - 1) + square (2 * y - 1) <= 4 * square radius

let computeFoVForColumnPortion current isOpaque setFieldOfView radius (queue: System.Collections.Generic.Queue<ColumnPortion>) = 
   let {x=x; topVector=top; bottomVector=bottomVector} = current
   let topY =
      if x = 0 then 0
      else 
         let quotient = (2 * x + 1) * top.Y / (2 * top.X)
         let remainder = (2 * x + 1) * top.Y % (2 * top.X)
         if remainder > top.X then quotient + 1
         else quotient
   
   let bottomY = 
      if x = 0 then 0
      else 
         let quotient = (2 * x - 1) * bottomVector.Y / (2 * bottomVector.X)
         let remainder = (2 * x - 1) * bottomVector.Y % (2 * bottomVector.X)
         if remainder > bottomVector.X then quotient + 1
         else quotient
   
   let mutable topVector = top
   let mutable wasLastCellOpaque: bool option = None
   for y = topY to bottomY do
      let inRadius = isInRadius x y radius
      if inRadius then setFieldOfView x y
      let currentIsOpaque = not inRadius || isOpaque x y
      match (wasLastCellOpaque, currentIsOpaque) with
      | Some false, true  -> 
         let portion = { x = x+1; bottomVector = Vector(x*2-1, y*2+1); topVector = topVector }
         queue.Enqueue(portion)
         ()
      | Some true, false -> topVector <- Vector(x*2+1, y*2+1)
      | _, _ -> ()
      wasLastCellOpaque <- Some currentIsOpaque

   match wasLastCellOpaque with
   | Some false -> queue.Enqueue({ x = x+1; bottomVector=bottomVector; topVector=topVector})
   | _ -> ()

let computeFieldOfViewInOctantZero isOpaque setFieldOfView radius = 
   let queue = new System.Collections.Generic.Queue<ColumnPortion>()
   queue.Enqueue({x=0; bottomVector= Vector(1,0); topVector=Vector(1,1)})
   while queue.Count <> 0 do
      let current = queue.Dequeue()
      if(current.x <= radius) then
         computeFoVForColumnPortion current isOpaque setFieldOfView radius queue

let computeFieldOfViewWithShadowCasting x y radius isOpaque setFoV =
   let opaque = translateOrigin isOpaque x y |> translateOctant
   let fov = translateOrigin setFoV x y |> translateOctant

   for octant = 0 to 8 do
      computeFieldOfViewInOctantZero (opaque octant) (fov octant) radius
