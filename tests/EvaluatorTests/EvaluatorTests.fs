open Microsoft.FSharp.Collections
open Parser
open Evaluator

let evaluateTree (resultTree: Result<Expression list, string>) =
    match resultTree with
    | Ok resultValue -> Evaluator.evaluate resultValue
    | Error err ->
        printfn $"%A{err}"
        (Computed (Int 0), Environment(Map.empty, Map.empty))
        
let ln () = printfn "============="
let pr (str: string) = printfn $"Input:\n%A{str}\nOutput:"

printfn "Test 1: Evaluate simple cases\n"

let code_1_1 = "let x = 5"
pr(code_1_1)
let tree_1_1 = Parser.parseCode code_1_1
let result_1_1 = evaluateTree tree_1_1

ln()

let code_1_2 = "
let x = 5
dump x
"
pr(code_1_2)
let tree_1_2 = Parser.parseCode code_1_2
let result_1_2 = evaluateTree tree_1_2

ln()

let code_1_3 = "
let x = 5
dump \"Hi! \" * x
"
pr(code_1_3)
let tree_1_3 = Parser.parseCode code_1_3
let result_1_3 = evaluateTree tree_1_3

ln()

printfn "Test 2: If conditions\n"

let code_2_1 = "
let x = 5
if (x == 5) {
    dump \"congrats\"
}
"
pr(code_2_1)
let tree_2_1 = Parser.parseCode code_2_1
let result_2_1 = evaluateTree tree_2_1

ln()

let code_2_2 = "
let x = 5
if (x != 5) {
    dump \"congrats\"
} else {
    dump \"oh no...\"
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
    dump \"congrats\"
} else {
    dump \"oh no...\"
}

dump (\"end of program...\")
"
pr(code_2_3)
let tree_2_3 = Parser.parseCode code_2_3
let result_2_3 = evaluateTree tree_2_3

ln()

let code_2_4 = "
let x = 5
let z = 123
if (true) {
    dump x + z
    x <- 100
    dump x + z
} else {
    dump \"oh no...\"
}

dump (\"end of program...\")
"
pr(code_2_4)
let tree_2_4 = Parser.parseCode code_2_4
let result_2_4 = evaluateTree tree_2_4

ln()

printfn "Test 3: Function testing\n"

let code_3_1 = "
func printSum [a,b] {
    dump a+b
}

printSum[100,300]
"
pr(code_3_1)
let tree_3_1 = Parser.parseCode code_3_1
let result_3_1 = evaluateTree tree_3_1

ln()

let code_3_2 = "
func printSum [a,b] {
    return (a + b)
}

let x = printSum[100,300]
dump x
"
pr(code_3_2)
let tree_3_2 = Parser.parseCode code_3_2
let result_3_2 = evaluateTree tree_3_2

ln()

let code_3_3 = "
func fact [n] {
    if n == 1 {
        return 1
    } else {
        let f = fact[n - 1]
        return (n * f)
    }
}

let res = fact[5]
dump res
"
pr(code_3_3)
let tree_3_3 = Parser.parseCode code_3_3
let result_3_3 = evaluateTree tree_3_3

ln()

let code_3_4 = "
let x = 1

func f [] {
    x <- x + x
}

f[]
dump x
f[]
dump x
"
pr(code_3_4)
let tree_3_4 = Parser.parseCode code_3_4
let result_3_4 = evaluateTree tree_3_4

ln()

let code_3_5 = "
let x = 5 + 2 * 5
dump x
"
pr(code_3_5)
let tree_3_5 = Parser.parseCode code_3_5
let result_3_5 = evaluateTree tree_3_5

ln()

let code_3_6 = "
let x = (5 + 2) * 5
dump x
"
pr(code_3_6)
let tree_3_6 = Parser.parseCode code_3_6
let result_3_6 = evaluateTree tree_3_6

ln()

printfn "Test 4: List testing\n"

let code_4_1 = "
let l = [1, 2, 3]
dump l
"
pr(code_4_1)
let tree_4_1 = Parser.parseCode code_4_1
let result_4_1 = evaluateTree tree_4_1

ln()

let code_4_2 = "
let l1 = [1, 2, 3]
let l2 = [3,4,5]
dump (l1 + l2)
"
pr(code_4_2)
let tree_4_2 = Parser.parseCode code_4_2
let result_4_2 = evaluateTree tree_4_2

ln()

let code_4_3 = "
let l1 = [1, 2, 3]
let l2 = [3,4,5]
dump (l1 * l2)
"
pr(code_4_3)
let tree_4_3 = Parser.parseCode code_4_3
let result_4_3 = evaluateTree tree_4_3

ln()

let code_4_4 = "
let l1 = [1, 2, 3]
let l2 = [1, 2]
dump (l1 / l2)
"
pr(code_4_4)
let tree_4_4 = Parser.parseCode code_4_4
let result_4_4 = evaluateTree tree_4_4

ln()

let code_4_5 = "
let l1 = [1, 2, 3]
let l2 = l1
dump l2
"
pr(code_4_5)
let tree_4_5 = Parser.parseCode code_4_5
let result_4_5 = evaluateTree tree_4_5

ln()

let code_4_6 = "
func f[] {
    let x = [1, 2]
    return x
}
let l1 = f[]
dump l1
"
pr(code_4_6)
let tree_4_6 = Parser.parseCode code_4_6
let result_4_6 = evaluateTree tree_4_6

ln()

let code_4_7 = "
let l = [1, 2, 3]
l <- (l add 5)
dump l
"
pr(code_4_7)
let tree_4_7 = Parser.parseCode code_4_7
let result_4_7 = evaluateTree tree_4_7

ln()

let code_4_8 = "
let l = [1, 2, 3]
l <- (l remove 2)
dump l
"
pr(code_4_8)
let tree_4_8 = Parser.parseCode code_4_8
let result_4_8 = evaluateTree tree_4_8

ln()

let code_4_9 = "
let l1 = [1, 2, 3]
let l2 = (l1 remove 2)
dump l2
"
pr(code_4_9)
let tree_4_9 = Parser.parseCode code_4_9
let result_4_9 = evaluateTree tree_4_9

ln()

printfn "Test 5: IO operations testing\n"

let code_5_1 = "
let f = file(\"/home/denis/trash/fsharp-lab-test/test.txt\")
let t = f readFile \"text\" 
dump t
"
pr(code_5_1)
let tree_5_1 = Parser.parseCode code_5_1
let result_5_1 = evaluateTree tree_5_1

ln()

let code_5_2 = "
let f = file(\"/home/denis/trash/fsharp-lab-test/test.txt\")
let lines = f readFile \"lines\" 
dump lines
"
pr(code_5_2)
let tree_5_2 = Parser.parseCode code_5_2
let result_5_2 = evaluateTree tree_5_2

ln()

let code_5_3 = "
let f = file(\"/home/denis/trash/fsharp-lab-test/test.txt\")
f writeFile \"some_text_variable\" 
"
pr(code_5_3)
let tree_5_3 = Parser.parseCode code_5_3
let result_5_3 = evaluateTree tree_5_3

ln()

let code_5_4 = "
let l = [1,2,3]
let f = file(\"/home/denis/trash/fsharp-lab-test/test.txt\")
f writeFile l 
"
pr(code_5_4)
let tree_5_4 = Parser.parseCode code_5_4
let result_5_4 = evaluateTree tree_5_4

ln()
