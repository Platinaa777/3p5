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
    | LiteralExpr of value:Value
    | VariableExpr of name:Id
    | OperationExpr of left_operand:Expression * operator:Operator * right_operand:Expression
    | LambdaExpr of parameters:Id list * body:Scope
    | AppExpr of arguments:Expression list * func:Expression
    | FuncExpr of func_name:string * parameters:Id list * body:Scope
    
and Statement =
    | Condition of condition:Expression * true_scope:Scope * false_scope:Scope option
    | ConsoleWrite of message:Expression
    | Let of var_name:string * value:Expression
    
and Scope = Statement list
and Closure = Closure of parameters:Id list * body:Scope * env:Environment 
and Environment = Environment of context: Map<Id, Expression>

module Parser =
    let ss = spaces // only for me
    let pStr str = pstring str .>> ss // for simplicity in code below
    
    // Numbers parsers
    let pInt: Parser<Value, Unit> = pint32 .>> ss |>> Int
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
            pFloat;
            pInt;
            pBool;
            pString
        ]     
    
    // for literal case
    let pLiteralExpr: Parser<Expression, Unit> = pValue |>> LiteralExpr .>> ss
    
    // I want that variable can start with default char, @ or underscore
    let validVarFirstChar c = Char.IsLetter(c) || c.Equals('@') || c.Equals('_')
    let pVar = many1Satisfy2 validVarFirstChar Char.IsLetterOrDigit .>> ss;
    
    // for variable case
    let pVariableExpr: Parser<Expression, Unit> = pVar |>> VariableExpr .>> ss
    
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
    
    // for precendence (define in fparsec library)
    let operatorParser = OperatorPrecedenceParser<Expression, Unit, Unit>()

    let termParser =
        choice [
            pLiteralExpr
            pVariableExpr
            between (pStr "(") (pStr ")") operatorParser.ExpressionParser
        ]

    operatorParser.TermParser <- termParser

    let addBinaryOperator opStrValue precedence associativity =
        match operatorMap.TryGetValue(opStrValue) with
        | true, operator ->
            operatorParser.AddOperator(
                InfixOperator(opStrValue, ss, precedence, associativity, (fun left right -> OperationExpr(left, operator, right)))
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

    let pExpr = operatorParser.ExpressionParser
            
    let pLet: Parser<Statement, Unit> =
        pipe2
            (pStr "let" >>. pVar .>> pStr "=")
            pExpr
            (fun name expr ->
                // printfn $"name: %A{name} ||| expr: %A{expr}" // debugging
                Let(name, expr))
            
    let pStatement, pStatementRef = createParserForwardedToRef<Statement, Unit>()
    
    let pBlock: Parser<Statement list, Unit> =
        ss >>. between (pStr "{") (pStr "}") (many pStatement)
    
    let pCondition: Parser<Statement, Unit> =
        pipe3
            (ss >>. pStr "if" >>. pExpr)
            pBlock
            (pStr "else" >>. pBlock |> opt) // opt because the else block cannot be 
            (fun cond body _else -> Condition(cond, body, _else))

    let pConsoleWrite: Parser<Statement, Unit> =
        pStr "ConsoleWrite" >>. pExpr |>> ConsoleWrite
        
    do pStatementRef.Value <- choice [
            pLet;
            pCondition;
            pConsoleWrite;
        ]
    
    
    