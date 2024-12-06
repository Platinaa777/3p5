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
    | Assign of var_name:string * value:Expression
    | While of condition:Expression * body:Scope
    
and Scope = Statement list
and Closure = Closure of parameters:Id list * body:Scope * env:Environment 
and Environment = Environment of context: Map<Id, Expression> * parent_context:Option<Environment>

module Parser =
    let ss = spaces // only for me
    
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
    
    // i want that variable can start with default char, @ or underscore
    let validVarFirstChar c = Char.IsLetter(c) || c.Equals('@') || c.Equals('_')
    
    // for variable case
    let pVariableExpr: Parser<Expression, Unit> = many1Satisfy2 validVarFirstChar Char.IsLetterOrDigit |>> VariableExpr .>> ss
    
    let pOperator =
        choice [
            stringReturn "+" Add
            stringReturn "-" Subtract
            stringReturn "*" Multiply
            stringReturn "/" Divide
            stringReturn "%" Mod

            stringReturn ">" GreaterThan
            stringReturn "<" LessThan
            stringReturn ">=" GreaterThanOrEqual
            stringReturn "<=" LessThanOrEqual
            stringReturn "==" Equal
            stringReturn "!=" NotEqual

            // like python :D
            stringReturn "and" And
            stringReturn "or" Or
        ] .>> ss
    
    // parsing left operand, operator, operand
    let pBinaryExpr: Parser<Expression, Unit> =
        pipe3
            (pLiteralExpr <|> pVariableExpr)
            pOperator
            (pLiteralExpr <|> pVariableExpr)
            (fun left op right -> OperationExpr (left, op, right))
    
    let pExpr =
        choice [
            attempt pBinaryExpr
            pLiteralExpr;
            pVariableExpr;
        ]
        
    let pAssign: Parser<Statement, Unit> =
        let identifier =
            many1Satisfy2 validVarFirstChar Char.IsLetterOrDigit
            .>> ss

        pipe2
            (identifier .>> pstring "=" .>> ss)
            pExpr
            (fun name expr -> Assign(name, expr))

    let pStatement =
        choice [
            pAssign
        ]