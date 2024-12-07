open Microsoft.FSharp.Collections
open Parser
open Evaluator

let evaluateTree (resultTree: Result<Expression list, string>) =
    match resultTree with
    | Ok resultValue -> Evaluator.evaluate resultValue
    | Error err ->
        printfn $"%A{err}"
        (Int 0, Environment(Map.empty, Map.empty))
        
let ln () = printfn "============="
let pr (str: string) = printfn $"%A{str}"

printfn "Test 1: Evaluate simple cases\n"

let code_1_1 = "let x = 5"
pr(code_1_1)
let tree_1_1 = Parser.parseCode code_1_1
let result_1_1 = evaluateTree tree_1_1

ln()

let code_1_2 = "
let x = 5
ConsoleWrite x
"
pr(code_1_2)
let tree_1_2 = Parser.parseCode code_1_2
let result_1_2 = evaluateTree tree_1_2

ln()

printfn "Test 2: If conditions\n"

let code_2_1 = "
let x = 5
if (x == 5) {
    ConsoleWrite \"congrats\"
}
"
pr(code_2_1)
let tree_2_1 = Parser.parseCode code_2_1
let result_2_1 = evaluateTree tree_2_1

ln()

let code_2_2 = "
let x = 5
if (x != 5) {
    ConsoleWrite \"congrats\"
} else {
    ConsoleWrite \"oh no...\"
}
"
pr(code_2_2)
let tree_2_2 = Parser.parseCode code_2_2
let result_2_2 = evaluateTree tree_2_2

ln()

let code_2_3 = "
let x = 5
let z = 123
if (z > x) {
    ConsoleWrite \"congrats\"
} else {
    ConsoleWrite \"oh no...\"
}

ConsoleWrite (\"end of program...\")
"
pr(code_2_3)
let tree_2_3 = Parser.parseCode code_2_3
let result_2_3 = evaluateTree tree_2_3

ln()

