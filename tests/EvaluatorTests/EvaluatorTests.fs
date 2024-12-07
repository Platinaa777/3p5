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

// printfn "Test 1: Evaluate simple cases\n"
//
// let code_1_1 = "let x = 5"
// pr(code_1_1)
// let tree_1_1 = Parser.parseCode code_1_1
// let result_1_1 = evaluateTree tree_1_1
//
// ln()
//
// let code_1_2 = "
// let x = 5
// dump x
// "
// pr(code_1_2)
// let tree_1_2 = Parser.parseCode code_1_2
// let result_1_2 = evaluateTree tree_1_2
//
// ln()
//
// let code_1_3 = "
// let x = 5
// dump \"Hi! \" * x
// "
// pr(code_1_3)
// let tree_1_3 = Parser.parseCode code_1_3
// let result_1_3 = evaluateTree tree_1_3
//
// ln()
//
// printfn "Test 2: If conditions\n"
//
// let code_2_1 = "
// let x = 5
// if (x == 5) {
//     dump \"congrats\"
// }
// "
// pr(code_2_1)
// let tree_2_1 = Parser.parseCode code_2_1
// let result_2_1 = evaluateTree tree_2_1
//
// ln()
//
// let code_2_2 = "
// let x = 5
// if (x != 5) {
//     dump \"congrats\"
// } else {
//     dump \"oh no...\"
// }
// "
// pr(code_2_2)
// let tree_2_2 = Parser.parseCode code_2_2
// let result_2_2 = evaluateTree tree_2_2
//
// ln()
//
// let code_2_3 = "
// let x = 5
// let z = 123
// if (z > x) {
//     dump \"congrats\"
// } else {
//     dump \"oh no...\"
// }
//
// dump (\"end of program...\")
// "
// pr(code_2_3)
// let tree_2_3 = Parser.parseCode code_2_3
// let result_2_3 = evaluateTree tree_2_3
//
// ln()
//
// let code_2_4 = "
// let x = 5
// let z = 123
// if (true) {
//     dump x + z
//     let x = 100
//     dump x + z
// } else {
//     dump \"oh no...\"
// }
//
// dump (\"end of program...\")
// "
// pr(code_2_4)
// let tree_2_4 = Parser.parseCode code_2_4
// let result_2_4 = evaluateTree tree_2_4
//
// ln()
//
// printfn "Test 3: Function testing\n"
//
// let code_3_1 = "
// func printSum [a,b] {
//     dump a+b
// }
//
// printSum[100,300]
// "
// pr(code_3_1)
// let tree_3_1 = Parser.parseCode code_3_1
// let result_3_1 = evaluateTree tree_3_1
//
// ln()
//
// let code_3_2 = "
// func printSum [a,b] {
//     return (a + b)
// }
//
// let x = printSum[100,300]
// dump x
// "
// pr(code_3_2)
// let tree_3_2 = Parser.parseCode code_3_2
// let result_3_2 = evaluateTree tree_3_2
//
// ln()

let code_3_3 = "
func fact [n] {
    dump n
    if n == 1 {
        return 1
    } else {
        let f = fact[n - 1]
        return (n * f)
    }
}

let res = fact[5]
dump \"result:\"
dump res
"
pr(code_3_3)
let tree_3_3 = Parser.parseCode code_3_3
let result_3_3 = evaluateTree tree_3_3

ln()