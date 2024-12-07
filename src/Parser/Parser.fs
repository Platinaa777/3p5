namespace Parser

open FParsec
open System

type Id = string

type Value =
    | Int of int
    | Float of float
    | Bool of bool
    | Str of string
    
type Operator =
    | Add
    | Subtract
    | Multiply
    | Divide
    | Mod
    // comparisons operators
    | GreaterThan
    | LessThan
    | GreaterThanOrEqual
    | LessThanOrEqual
    | Equal
    | NotEqual
    // condition operators
    | And
    | Or
        
type Expression =
    | Literal of value:Value
    | Variable of name:Id
    | Operation of left_operand:Expression * operator:Operator * right_operand:Expression
    | Condition of condition:Expression * true_scope:Expression list * false_scope:Expression list option
    | Dump of message:Expression
    | Let of var_name:string * valueExpr:Expression
    | FuncDef of name:Id * parameters:Id list * body:Expression list
    | FuncCall of func_name:Id * arguments:Expression list
    | Return of result:Expression

module Parser =
    let ss = spaces // only for me
    let pStr str = pstring str .>> ss // for simplicity in code below
    
    // Numbers parsers
    let pInt: Parser<Value, Unit> =
        pint32 .>> notFollowedBy (pchar '.') .>> ss |>> Int
    let pFloat: Parser<Value, Unit> = pfloat .>> ss |>> Float
    
    // boolean parser
    let pBoolTrue = stringReturn "true" <| Bool(true) .>> ss
    let pBoolFalse = stringReturn "false" <| Bool(false) .>> ss
    let pBool: Parser<Value, Unit> =
        choice [
            pBoolTrue
            pBoolFalse
        ]
    
    // string parser
    let pString: Parser<Value, Unit> =
        (pchar '\"') >>. manyCharsTill anyChar (pchar '\"')
        |>> (fun s ->
            // printfn $"pString: %A{s}" // for debugging
            Str s)
        .>> ss

    // common parser for all Value types
    let pValue: Parser<Value, Unit> =
        choice [
            attempt pInt;
            pFloat;
            pBool;
            pString
        ]
        
    let pExpr, pExprRef = createParserForwardedToRef<Expression, Unit>()
    // for literal case
    let pLiteralExpr: Parser<Expression, Unit> = ss >>. pValue |>> Literal .>> ss
    
    // I want that variable can start with default char, @ or underscore
    let validVarFirstChar c = Char.IsLetter(c) || c.Equals('@') || c.Equals('_')
    let pVar: Parser<string, Unit> = many1Satisfy2 validVarFirstChar Char.IsLetterOrDigit .>> ss;
    
    // for variable case
    let pVariableExpr: Parser<Expression, Unit> = pVar |>> Variable .>> ss
        
    let operatorMap =
        dict [
            "+", Add
            "-", Subtract
            "*", Multiply
            "/", Divide
            "%", Mod
            ">", GreaterThan
            "<", LessThan
            ">=", GreaterThanOrEqual
            "<=", LessThanOrEqual
            "==", Equal
            "!=", NotEqual
            "and", And
            "or", Or
        ]
    
    // for precedence (define in fparsec library)
    let operatorParser = OperatorPrecedenceParser<Expression, Unit, Unit>()

    let pFuncCall: Parser<Expression, Unit> =
        pipe2
            (ss >>. pVar)
            (between (pStr "[") (pStr "]") (sepBy pExpr (pStr ",")))
            (fun funcName args ->
                printfn $"Parsed function call: {funcName} with args: {args}" // debugging
                FuncCall(funcName, args))
    
    let termParser =
        choice [
            attempt pFuncCall;
            pLiteralExpr;
            pVariableExpr;
            between (pStr "(") (pStr ")") operatorParser.ExpressionParser;
        ]

    operatorParser.TermParser <- termParser

    let addBinaryOperator opStrValue precedence associativity =
        match operatorMap.TryGetValue(opStrValue) with
        | true, operator ->
            operatorParser.AddOperator(
                InfixOperator(opStrValue, ss, precedence, associativity, (fun left right -> Operation(left, operator, right)))
            )
        | false, _ ->
            failwithf $"Unknown operator: %s{opStrValue}"

    let al = Associativity.Left
    
    addBinaryOperator "+" 4 al
    addBinaryOperator "-" 4 al
    addBinaryOperator "*" 5 al
    addBinaryOperator "/" 5 al
    addBinaryOperator "%" 5 al
    addBinaryOperator ">" 3 al
    addBinaryOperator "<" 3 al
    addBinaryOperator ">=" 3 al
    addBinaryOperator "<=" 3 al
    addBinaryOperator "==" 3 al
    addBinaryOperator "!=" 3 al
    addBinaryOperator "and" 2 al
    addBinaryOperator "or" 1 al

    let pLet: Parser<Expression, Unit> =
        pipe2
            (ss >>. pStr "let" >>. pVar .>> pStr "=")
            pExpr
            (fun name expr ->
                // printfn $"name: %A{name} ||| expr: %A{expr}" // debugging
                Let(name, expr))
            
    let pScope: Parser<Expression list, Unit> =
        ss >>. between (pStr "{") (pStr "}") (many pExpr)
    
    let pCondition: Parser<Expression, Unit> =
        pipe3
            (ss >>. pStr "if" >>. pExpr)
            pScope
            (pStr "else" >>. pScope |> opt) // opt because the else block cannot be 
            (fun cond body _else -> Condition(cond, body, _else))

    let pDump: Parser<Expression, Unit> =
        pStr "dump" >>. pExpr |>> Dump
        
    let pFuncDef: Parser<Expression, Unit> =
        pipe3
            (ss >>. pStr "func" >>. pVar)
            (between (pStr "[") (pStr "]") (sepBy pVar (pStr ",")))
            pScope
            (fun name parameters body -> FuncDef(name, parameters, body))
                    
    let pReturn: Parser<Expression, Unit> =
        pStr "return" >>. pExpr |>> Return
                    
    do pExprRef.Value <- choice [
            attempt pLet
            attempt pDump;
            attempt pFuncDef;
            attempt pCondition;
            attempt pFuncCall
            attempt pReturn;
            attempt operatorParser.ExpressionParser
        ]
    
    let parseCode (str: string): Result<Expression list, string> =
        match run (many pExpr) str with
        | Success (result, _, _) -> Result.Ok result
        | Failure (err, _, _) -> Result.Error err