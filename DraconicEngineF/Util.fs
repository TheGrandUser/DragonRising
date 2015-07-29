module TryParser
let tryParseWith tryParseFunc = tryParseFunc >> function
   | true, v -> Some v
   | false, _ -> None

let hex s = System.Int32.TryParse(s, System.Globalization.NumberStyles.HexNumber, null)

let parseDate = tryParseWith System.DateTime.TryParse
let parseInt = tryParseWith System.Int32.TryParse
let parseHex = tryParseWith hex
let parseSingle = tryParseWith System.Single.TryParse
let parseDouble = tryParseWith System.Double.TryParse

let (|Date|_|) = parseDate
let (|Int|_|) = parseInt
let (|Hex|_|) = parseHex
let (|Single|_|) = parseSingle
let (|Double|_|) = parseDouble

let thing = FSharpx.Collections.Queue.conj 1 FSharpx.Collections.Queue.empty
//module List =
//   let rec last = function
//       | hd :: [] -> hd
//       | hd :: tl -> last tl
//       | _ -> failwith "Empty list."


