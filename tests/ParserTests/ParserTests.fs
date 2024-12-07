open Parser
open FParsec

let test parser strInput =
    match run parser strInput with
    | Success (result, _, _) -> printfn $"{result}"
    | Failure (error, _, _) -> printfn $"%s{error}"

let testMany parser strInput =
    match run (many parser) strInput with
    | Success (result, _, _) -> printfn $"{result}"
    | Failure (error, _, _) -> printfn $"%s{error}"

printfn "Test 1: Parsing simple values\n"

printfn "value: \"code1da sdas das dsada\""
let x1_1 = test Parser.pString "\"code1da sdas das dsada\""

printfn "value: true"
let x1_2 = test Parser.pBool "true"

printfn "value: false"
let x1_3 = test Parser.pBool "false"

printfn "value: 3.21312312321"
let x1_4 = test Parser.pFloat "3.21312312321"

printfn "value: 51312312"
let x1_5 = test Parser.pInt "51312312"

printfn "\nTest 2: Parsing literal expressions\n"

printfn "string literal: \"dsada\""
let x2_1 = test Parser.pValue "\"dsada\""

printfn "bool literal: true"
let x2_2 = test Parser.pValue "true"

printfn "bool literal: false"
let x2_3 = test Parser.pValue "false"

printfn "int literal: 123123"
let x2_4 = test Parser.pValue "123123"

printfn "float literal: 3.141231232131"
let x2_5 = test Parser.pValue "3.141231232131"

printfn "\nTest 3: Testing Let\n"

printfn "case: let x = 1"
let x3_1 = test Parser.pExpr "let x = 1"

printfn "case: let xx1323123 = 2"
let x3_2 = test Parser.pExpr "let x1323123 = 2"

printfn "case: let _x132 = 3132"
let x3_3 = test Parser.pExpr "let _x132 = 3132"

printfn "\nTest 4: Parsing binary expressions\n"

printfn "case 1 + 3"
let x4_1 = test Parser.pExpr "1 + 3"

printfn "case 1 - x"
let x4_2 = test Parser.pExpr "1 - x"

printfn "case y / x"
let x4_3 = test Parser.pExpr "y / x"

printfn "case y and x"
let x4_4 = test Parser.pExpr "y and x"

printfn "\nTest 5: Let testing and difficult operations\n"

printfn "case 'let x = 5'"
let x5_1 = test Parser.pExpr "let x = 5"

printfn "case 'let x = 1 + 5'"
let x5_2 = test Parser.pExpr "let x = 1 + 5"

printfn "case 'let x = y + z'"
let x5_3 = test Parser.pExpr "let x = y + z"

printfn "case 'let x = (y + z) + t'"
let x5_4 = test Parser.pExpr "let x = (y + z) + t"

printfn "case 'let x = (y + z) + (t + d)'"
let x5_5 = test Parser.pExpr "let x = (y + z) + (t + d)"

printfn "case 'let x = (y + z) + (t + d) + 5'"
let x5_6 = test Parser.pExpr "let x = (y + z) + (t + d) + 5"

printfn "case 'let x = (((y + z) + t) + 5)'"
let x5_7 = test Parser.pExpr "let x = (((y + z) + t) + 5)"

printfn "case 'let x = 1 + 2 + 3'"
let x5_8 = test Parser.pExpr "let x = 1 + 2 + 3"

printfn "\nTest 6: If condition\n"

let cond1 = "
if x == 5 {
    let z = 10
} else {
    let t = 5
}
"

printfn $"\n%A{cond1}\n"
let x6_1 = test Parser.pExpr cond1

let cond2 = "
if x == 5 {
    let z = 10
}
"

printfn $"\n%A{cond2}\n"
let x6_2 = test Parser.pExpr cond2

let cond3 = "
if x >= 5 {
    let z = 10
} else {
    let t = 5
}
"
printfn $"\n%A{cond3}\n"
let x6_3 = test Parser.pExpr cond3

let cond4 = "
if x == 5 {
    let z = 10
    if (x != 10) {
        let xx = 123
    }
} else {
    let t = 5
}
"

printfn $"\n%A{cond4}\n"
let x6_4 = test Parser.pExpr cond4

printfn "\nTest 7: ConsoleWrite testing\n"

printfn "case: ConsoleWrite x + 5"
let x7_1 = test Parser.pExpr "ConsoleWrite x + 5"

printfn "case: ConsoleWrite (x + 5)"
let x7_2 = test Parser.pExpr "ConsoleWrite (x + 5)"

printfn "\nTest 8: FuncDef parsing\n"

let funcDef1 = "
func f [a,b] {
    let x = 5
    let y = 10
}
"
printfn $"\n%A{funcDef1}\n"
let x8_1 = test Parser.pExpr funcDef1

let funcDef2 = "
func f [a,b] {
    if (a == b) {
        let x = 5
    } else {

    }
}
"
printfn $"\n%A{funcDef2}\n"
let x8_2 = test Parser.pExpr funcDef2

let funcDef3 = "
func f [a,b] {
    func ff [x,y] {
        let t = 5
        let t2 = x == y
    }
}
"
printfn $"\n%A{funcDef3}\n"
let x8_3 = test Parser.pExpr funcDef3

printfn "\nTest 9: FuncCall parsing\n"

let funcCall1 = "
f [x, y]
"
printfn $"\n%A{funcCall1}\n"
let x9_1 = test Parser.pExpr funcCall1


let funcCall2 = "
f []
"
printfn $"\n%A{funcCall2}\n"
let x9_2 = test Parser.pExpr funcCall2

let funcCall3 = "
f [1, 2]
"
printfn $"\n%A{funcCall3}\n"
let x9_3 = test Parser.pExpr funcCall3

printfn "\nTest 10: Test many statements\n"

let manyCode1 = "
if (x == 5) {
    let z = 5
}

let x = 5

func f [a,b] {
    let c = a + b
}
"

printfn $"\n%A{manyCode1}\n"
let x10_1 = testMany Parser.pExpr manyCode1

let manyCode2 = "
let x = 5
let y = 10
let z = x + y
"

printfn $"\n%A{manyCode2}\n"
let x10_2 = testMany Parser.pExpr manyCode2

let manyCode3 = "
func f1 [a,b] {
    let c = a + b
}
func f2 [x,y] {
    let zz = x + y
}
"

printfn $"\n%A{manyCode3}\n"
let x10_3 = testMany Parser.pExpr manyCode3
