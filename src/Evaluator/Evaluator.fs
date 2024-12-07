namespace Evaluator

open Parser

type Function =
    Function of parameters:Id list * body:Expression list * env:Environment
and
    // context - variable context
    // function - all functions in code
    Environment = Environment of context: Map<Id, Value> * functions: Map<Id, Function>

module Evaluator =
    let private unit env = (Int 0, env)
    let private funof (op: Operator) =
        match op with
        | Add -> (fun left right ->
                    match left, right with
                    | Int a, Int b -> Int (a + b)
                    | Float a, Float b -> Float(a + b)
                    | Int a, Float b -> Float(float a + b)
                    | Float a, Int b -> Float(a + float b)
                    | Str s1, Str s2 -> Str (s1 + s2)
                    | _ -> failwith "Invalid operand types for Add")
        | Subtract -> (fun left right ->
                    match left, right with
                    | Int a, Int b -> Int (a - b)
                    | Float a, Float b -> Float(a - b)
                    | Int a, Float b -> Float(float a - b)
                    | Float a, Int b -> Float(a - float b)
                    | _ -> failwith "Invalid operand types for Subtract")
        | Multiply -> (fun left right ->
                    match left, right with
                    | Int a, Int b -> Int (a * b)
                    | Float a, Float b -> Float(a * b)
                    | Int a, Float b -> Float(float a * b)
                    | Float a, Int b -> Float(a * float b)
                    | Str s, Int i -> Str (String.replicate i s)
                    | _ -> failwith "Invalid operand types for Multiply")
        | Divide -> (fun left right ->
                    match left, right with
                    | smth, Int b when b = 0 -> failwith $"Error: division by zero {smth} / {b}"
                    | Int a, Int b -> Int (a / b)
                    | Float a, Float b -> Float(a / b)
                    | Int a, Float b -> Float(float a / b)
                    | Float a, Int b -> Float(a / float b)
                    | _ -> failwith "Invalid operand types for Divide")
        | Mod -> (fun left right ->
                    match left, right with
                    | smth, Int b when b = 0 -> failwith $"Error: mod by zero {smth} %% {b}"
                    | Int a, Int b -> Int (a % b)
                    | Float a, Float b -> Float(a % b)
                    | Int a, Float b -> Float(float a % b)
                    | Float a, Int b -> Float(a % float b)
                    | _ -> failwith "Invalid operand types for Mod")
        | GreaterThan -> (fun left right ->
                        match left, right with
                        | Int a, Int b -> Bool (a > b)
                        | Float a, Float b -> Bool (a > b)
                        | Int a, Float b -> Bool (float a > b)
                        | Float a, Int b -> Bool (a > float b)
                        | _ -> failwith "Invalid operand types for GreaterThan")
        | LessThan -> (fun left right ->
                        match left, right with
                        | Int a, Int b -> Bool (a < b)
                        | Float a, Float b -> Bool (a < b)
                        | Int a, Float b -> Bool (float a < b)
                        | Float a, Int b -> Bool (a < float b)
                        | _ -> failwith "Invalid operand types for LessThan")
        | GreaterThanOrEqual -> (fun left right ->
                                   match left, right with
                                   | Int a, Int b -> Bool (a >= b)
                                   | Float a, Float b -> Bool (a >= b)
                                   | Int a, Float b -> Bool (float a >= b)
                                   | Float a, Int b -> Bool (a >= float b)
                                   | _ -> failwith "Invalid operand types for GreaterThanOrEqual")
        | LessThanOrEqual -> (fun left right ->
                              match left, right with
                              | Int a, Int b -> Bool (a <= b)
                              | Float a, Float b -> Bool (a <= b)
                              | Int a, Float b -> Bool (float a <= b)
                              | Float a, Int b -> Bool (a <= float b)
                              | _ -> failwith "Invalid operand types for LessThanOrEqual")
        | Equal -> (fun left right ->
                    match left, right with
                    | Int a, Int b -> Bool (a = b)
                    | Float a, Float b -> Bool (a = b)
                    | Str a, Str b -> Bool (a = b)
                    | Bool a, Bool b -> Bool (a = b)
                    | _ -> failwith "Invalid operand types for Equal")
        | NotEqual -> (fun left right ->
                        match left, right with
                        | Int a, Int b -> Bool (a <> b)
                        | Float a, Float b -> Bool (a <> b)
                        | Str a, Str b -> Bool (a <> b)
                        | Bool a, Bool b -> Bool (a <> b)
                        | _ -> failwith "Invalid operand types for NotEqual")
        | And -> (fun left right ->
                   match left, right with
                   | Bool a, Bool b -> Bool (a && b)
                   | _ -> failwith "Invalid operand types for And")
        | Or -> (fun left right ->
                  match left, right with
                  | Bool a, Bool b -> Bool (a || b)
                  | _ -> failwith "Invalid operand types for Or")
        
    let private handleVarCase (name: Id) (env: Environment) =
        match env with
           | Environment(context, _) ->
               match Map.tryFind name context with
               | Some value -> (value, env)
               | None -> failwithf $"Variable %A{name} not found"
               
    let rec private valueToString (value: Value): string =
        match value with
        | Int t -> t.ToString() 
        | Float f -> f.ToString("F3")
        | Bool b -> b.ToString()
        | Str s -> s
    
    let rec private eval (expr: Expression) (env: Environment): Value * Environment =
       match expr with
       | Literal(value) -> (value, env)
       | Variable(name) ->
           handleVarCase name env
       | Operation(leftOperand, operator, rightOperand) ->
           handleOperationCase leftOperand operator rightOperand env
       | Condition(expression, trueScope, falseScope) ->
            let conditionResult, _ = eval expression env
            match conditionResult with
            | Bool(true) ->
                let _, finalEnv = evalScope trueScope env
                (Bool(true), finalEnv)
            | Bool(false) ->
                let _, finalEnv = 
                    match falseScope with
                    | Some scope -> evalScope scope env
                    | None -> unit env // no else block
                (Bool(false), finalEnv)
            | _ -> failwith "Condition expression must evaluate to a boolean"
       | Dump(message) ->
            let messageResult, _ = eval message env
            let printStr = valueToString messageResult
            printfn $"%A{printStr}"
            unit env
       | Let(varName, valueExpr) ->
            let value, _ = eval valueExpr env
            match env with
            | Environment (context, functions) ->
                let updatedContext = Map.add varName value context
                let updatedEnv = Environment(updatedContext, functions)
                (value, updatedEnv)
       | FuncDef(name, parameters, body) ->
            match env with
            | Environment(context, functions) ->
                let closure = Function(parameters, body, env)
                let updatedFunctions = Map.add name closure functions
                let updatedEnv = Environment(context, updatedFunctions)
                unit updatedEnv
       | FuncCall(funcName, arguments) ->
            handleFuncCallCase funcName arguments env
       | Return result ->
           match result with
               | Literal t -> (t, env)
               | Variable var ->
                   let v, _ = handleVarCase var env
                   (v, env)
               | FuncCall(funcName, arguments) ->
                   let v, _ = handleFuncCallCase funcName arguments env
                   (v, env)
               | Operation(leftOperand, operator, rightOperand) ->
                   handleOperationCase leftOperand operator rightOperand env
               | _ -> failwith $"Unhandled expression in Return: %A{result}"
    and
        private evalScope (scope: Expression list) (initEnv: Environment) : Value * Environment =
            List.fold (fun (_, env) expr -> eval expr env) (unit initEnv) scope
    and
        private handleFuncCallCase (funcName: Id) (arguments: Expression list) (env: Environment) =
        match env with
            // current context and functions
            | Environment(context, functions) ->
                match Map.tryFind funcName functions with
                | Some (Function(parameters, body, funcEnv)) ->
                    if List.length parameters <> List.length arguments then
                        failwithf $"Function %s{funcName} called with incorrect number of arguments"
                    else
                        // eval all args in function
                        let evaluatedArgs = 
                            arguments
                            |> List.map (fun arg -> fst (eval arg env))
                        let newContext =
                            List.zip parameters evaluatedArgs
                            |> List.fold (fun acc (param, value) -> Map.add param value acc) context
                        let newEnv = Environment(newContext, functions)
                        evalScope body newEnv
                | None ->
                    failwithf $"Function %s{funcName} not found"
    and
        private handleOperationCase (left: Expression) (op: Operator) (right: Expression) (env: Environment) =
           let leftExprResult, _ = eval left env
           let rightExprResult, _ = eval right env
           let func = funof op
           ((func leftExprResult rightExprResult), env)
    
    let evaluate (astTree: Expression list) =
        let initEnv = Environment(Map.empty, Map.empty)
        let finalEnv = List.fold (fun (_, env) expr -> eval expr env) (unit initEnv) astTree
        finalEnv
        