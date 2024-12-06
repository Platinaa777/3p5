open Parser
open FParsec

let test parser strInput =
    match run parser strInput with
    | Success (result, _, _) -> printfn "%O" result
    | Failure (error, _, _) -> printfn "%s" error

// VALUE TEST

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

// LITERAL TEST

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

printfn "\nTest 3: Parsing variable expressions\n"

printfn "x variable"
let x3_1 = test Parser.pExpr "x"

printfn "xx1323123 variable"
let x3_2 = test Parser.pExpr "x1323123"

printfn "_x132 variable"
let x3_3 = test Parser.pExpr "_x132"

printfn "\nTest 4: Parsing binary expressions\n"

printfn "case 1 + 3"
let x4_1 = test Parser.pExpr "1 + 3"

printfn "case 1 - x"
let x4_2 = test Parser.pExpr "1 - x"

printfn "case y / x"
let x4_3 = test Parser.pExpr "y / x"

printfn "case y and x"
let x4_4 = test Parser.pExpr "y and x"

printfn "\nTest 5: Parsing assignment\n"

printfn "case 'x = 5'"
let x5_1 = test Parser.pStatement "x = 5"

printfn "case 'x = 1 + 5'"
let x5_2 = test Parser.pStatement "x = 1 + 5"

printfn "case 'x = y + z'"
let x5_3 = test Parser.pStatement "x = y + z"