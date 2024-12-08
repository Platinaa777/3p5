open System.IO
open Evaluator
open Parser

let evaluateTree (resultTree: Result<Expression list, string>) =
    match resultTree with
    | Ok resultValue -> Evaluator.evaluate resultValue
    | Error err ->
        printfn $"%A{err}"
        (Computed (Int 0), Environment(Map.empty, Map.empty))

[<EntryPoint>]
let main args =
    if args.Length <> 1 then
        printf "Incorrect count of args!"
        1
    else
        let code = File.ReadAllText(args[0])
        let tree = Parser.parseCode code
        match tree with
        | Ok _ ->
            let _ = evaluateTree tree
            0
        | Error err ->
            printfn $"Error while parsing %A{err}"
            1
