open Parser
open FParsec

let test parser strInput =
    match run parser strInput with
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
let x2_1 = test Parser.pExpr "\"dsada\""

printfn "bool literal: true"
let x2_2 = test Parser.pExpr "true"

printfn "bool literal: false"
let x2_3 = test Parser.pExpr "false"

printfn "int literal: 123123"
let x2_4 = test Parser.pExpr "123123"

printfn "float literal: 3.141231232131"
let x2_5 = test Parser.pExpr "3.141231232131"

printfn "\nTest 3: Testing Let\n"

printfn "x variable"
let x3_1 = test Parser.pStatement "let x = 1"

printfn "xx1323123 variable"
let x3_2 = test Parser.pStatement "let x1323123 = 2"

printfn "_x132 variable"
let x3_3 = test Parser.pStatement "let _x132 = 3132"

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
let x5_1 = test Parser.pStatement "let x = 5"

printfn "case 'let x = 1 + 5'"
let x5_2 = test Parser.pStatement "let x = 1 + 5"

printfn "case 'let x = y + z'"
let x5_3 = test Parser.pStatement "let x = y + z"

printfn "case 'let x = (y + z) + t'"
let x5_4 = test Parser.pStatement "let x = (y + z) + t"

printfn "case 'let x = (y + z) + (t + d)'"
let x5_5 = test Parser.pStatement "let x = (y + z) + (t + d)"

printfn "case 'let x = (y + z) + (t + d) + 5'"
let x5_6 = test Parser.pStatement "let x = (y + z) + (t + d) + 5"

printfn "case 'let x = (((y + z) + t) + 5)'"
let x5_7 = test Parser.pStatement "let x = (((y + z) + t) + 5)"

printfn "case 'let x = 1 + 2 + 3'"
let x5_8 = test Parser.pStatement "let x = 1 + 2 + 3"

printfn "\nTest 6: If condition\n"

let cond1 = "
if x == 5 {
    let z = 10
} else {
    let t = 5
}
"

printfn $"\n%A{cond1}\n"
let x6_1 = test Parser.pStatement cond1

let cond2 = "
if x == 5 {
    let z = 10
}
"

printfn $"\n%A{cond2}\n"
let x6_2 = test Parser.pStatement cond2

let cond3 = "
if x >= 5 {
    let z = 10
} else {
    let t = 5
}
"
printfn $"\n%A{cond3}\n"
let x6_3 = test Parser.pStatement cond3

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
let x6_4 = test Parser.pStatement cond4

printfn "\nTest 7: ConsoleWrite testing\n"

printfn "case: ConsoleWrite x + 5"
let x7_1 = test Parser.pStatement "ConsoleWrite x + 5"

printfn "case: ConsoleWrite (x + 5)"
let x7_2 = test Parser.pStatement "ConsoleWrite (x + 5)"

printfn "\nTest 8: FuncDef parsing\n"

let funcDef1 = "
func f [a,b] {
    let x = 5
    let y = 10
}
"
printfn $"\n%A{funcDef1}\n"
let x8_1 = test Parser.pStatement funcDef1

let funcDef2 = "
func f [a,b] {
    if (a == b) {
        let x = 5
    } else {

    }
}
"
printfn $"\n%A{funcDef2}\n"
let x8_2 = test Parser.pStatement funcDef2

let funcDef3 = "
func f [a,b] {
    func ff [x,y] {
        let t = 5
        let t2 = x == y
    }
}
"
printfn $"\n%A{funcDef3}\n"
let x8_3 = test Parser.pStatement funcDef3

printfn "\nTest 9: FuncCall parsing\n"

let funcCall1 = "
f [x, y]
"
printfn $"\n%A{funcCall1}\n"
let x9_1 = test Parser.pStatement funcCall1


let funcCall2 = "
f []
"
printfn $"\n%A{funcCall2}\n"
let x9_2 = test Parser.pStatement funcCall2

let funcCall3 = "
f [1, 2]
"
printfn $"\n%A{funcCall3}\n"
let x9_3 = test Parser.pStatement funcCall3
