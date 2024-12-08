namespace Evaluator

open Parser

type Function =
    Function of parameters:Id list * body:Expression list * env:Environment
and
    // context - variable context
    // function - all functions in code
    Environment = Environment of context: Map<Id, Value> * functions: Map<Id, Function>
    
type ExecuteState =
    | Computed of Value
    | Exited of Value 

module Evaluator =
    let private unit = Int 0
    let private funof (op: Operator) =
        match op with
        | Add -> (fun left right ->
                    match left, right with
                    | Int a, Int b -> Int (a + b)
                    | Float a, Float b -> Float(a + b)
                    | Int a, Float b -> Float(float a + b)
                    | Float a, Int b -> Float(a + float b)
                    | Str s1, Str s2 -> Str (s1 + s2)
                    | List l1, List l2 -> List (l1 @ l2)
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
                    | List l1, List l2 -> List (Set.toList (Set.ofList l1 |> Set.union (Set.ofList l2)))
                    | _ -> failwith "Invalid operand types for Multiply")
        | Divide -> (fun left right ->
                    match left, right with
                    | smth, Int b when b = 0 -> failwith $"Error: division by zero {smth} / {b}"
                    | Int a, Int b -> Int (a / b)
                    | Float a, Float b -> Float(a / b)
                    | Int a, Float b -> Float(float a / b)
                    | Float a, Int b -> Float(a / float b)
                    | List l1, List l2 -> List (Set.toList (Set.ofList l1 |> Set.intersect (Set.ofList l2)))
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
        | RemoveFromList -> (fun left right -> 
                                match left, right with
                                | List l, value -> 
                                    let updatedList = List.filter (fun v -> v <> value) l
                                    List updatedList
                                | _ -> failwith "Invalid operand types for RemoveFromList")
        | AddToList -> (fun left right -> 
                        match left, right with
                        | List l, value -> 
                            let updatedList = l @ [value]
                            List updatedList
                        | _ -> failwith "Invalid operand types for AddToList")
        
    let private returnEvalResult(res: ExecuteState, env: Environment) =
        match res with
        | Computed value -> (Computed value, env)
        | Exited value -> (Exited value, env)
        
    let private handleVarCase (name: Id) (env: Environment) =
        match env with
           | Environment(context, _) ->
               match Map.tryFind name context with
               | Some value -> (Computed value, env)
               | None -> failwithf $"Variable %A{name} not found"
               
    let rec private valueToString (value: Value): string =
        match value with
        | Int t -> t.ToString() 
        | Float f -> f.ToString("F3")
        | Bool b -> b.ToString()
        | Str s -> s
        | List values ->
            let elements = values
                           |> List.map valueToString
                           |> String.concat ", "
            $"[{elements}]" // like in python [1, 2, 3]
    
    let rec private eval (expr: Expression) (env: Environment): ExecuteState * Environment =
       match expr with
       | Literal(value) -> (Computed value, env)
       | Variable(name) ->
           handleVarCase name env
       | Operation(leftOperand, operator, rightOperand) ->
           handleOperationCase leftOperand operator rightOperand env
       | Condition(expression, trueScope, falseScope) ->
            let conditionResult, _ = eval expression env
            match conditionResult with
            | Computed value ->
                match value with
                | Bool(true) ->
                    let v, finalEnv = evalScope trueScope env
                    returnEvalResult(v, finalEnv)
                | Bool(false) ->
                    let v, finalEnv = 
                        match falseScope with
                        | Some scope ->
                            let v2, finalEnv = evalScope scope env
                            returnEvalResult(v2, finalEnv)
                        | None -> returnEvalResult(Computed(unit), env) // no else block
                    (v, finalEnv)
                | _ -> failwith "Condition expression must evaluate to a boolean"
            | _ -> failwith "Condition expression failed to evaluate properly"
       | Dump(message) ->
            let messageResult, _ = eval message env
            match messageResult with
            | Computed(v) ->
                let printStr = valueToString v
                printfn $"%A{printStr}"
                (Computed(unit), env)
            | _ -> failwith "Dump cannot contain return expression"
       | Let(varName, valueExpr) ->
            let res, _ = eval valueExpr env
            match res with
            | Computed(value) ->
                match env with
                | Environment (context, functions) ->
                    let updatedContext = Map.add varName value context
                    let updatedEnv = Environment(updatedContext, functions)
                    (res, updatedEnv)
            | Exited(value) ->
                match env with
                | Environment (context, functions) ->
                    let updatedContext = Map.add varName value context
                    let updatedEnv = Environment(updatedContext, functions)
                    (Computed value, updatedEnv)
        | Assign(var_name, valueExpr) ->
            let valueResult, updatedEnv = eval valueExpr env
            match valueResult with
            | Computed(value) ->
                match updatedEnv with
                | Environment(context, functions) ->
                    let newContext = Map.add var_name value context
                    let newEnv = Environment(newContext, functions)
                    (Computed(value), newEnv)
            | _ -> failwith "Failed to compute value for assignment"
       | FuncDef(name, parameters, body) ->
            match env with
            | Environment(context, functions) ->
                let closure = Function(parameters, body, env)
                let updatedFunctions = Map.add name closure functions
                let updatedEnv = Environment(context, updatedFunctions)
                (Computed unit, updatedEnv)
       | FuncCall(funcName, arguments) ->
            handleFuncCallCase funcName arguments env
       | Return result ->
           match result with
               | Literal t -> (Exited t, env)
               | Variable var ->
                   let v, _ = handleVarCase var env
                   match v with
                   | Computed(v2) | Exited(v2) -> (Exited(v2), env)
               | FuncCall(funcName, arguments) ->
                   let v, _ = handleFuncCallCase funcName arguments env
                   match v with
                   | Computed(v2) | Exited(v2) -> (Exited(v2), env)
               | Operation(leftOperand, operator, rightOperand) ->
                   handleOperationCase leftOperand operator rightOperand env
               | _ -> failwith $"Unhandled expression in Return: %A{result}"
    and
        private evalScope (scope: Expression list) (initEnv: Environment) : ExecuteState * Environment =
            let rec evalWithControl (expressions: Expression list) (env: Environment): ExecuteState * Environment =
                match expressions with
                | [] -> (Computed unit, env) // If there are no expressions, return the standard value
                | expr::rest ->
                    match eval expr env with
                    | Exited value, newEnv -> (Exited value, newEnv) // interrupt execution and return Exited
                    | Computed value, newEnv ->
                        match expr with
                        | Return _ -> (Exited value, newEnv) // expr is Return value, thus, we have to calculate body and then return it as Exited value
                        | _ -> evalWithControl rest newEnv // continue eval
            evalWithControl scope initEnv
    and
        private handleFuncCallCase (funcName: Id) (arguments: Expression list) (env: Environment) =
        match env with
            // current context and functions
            | Environment(context, functions) ->
                match Map.tryFind funcName functions with
                | Some (Function(parameters, body, _)) ->
                    if List.length parameters <> List.length arguments then
                        failwithf $"Function %s{funcName} called with incorrect number of arguments"
                    else
                        // eval all args in function
                        let evaluatedArgs = 
                            arguments
                            |> List.map (fun arg -> fst (eval arg env))
                        let newContext =
                            List.zip parameters evaluatedArgs
                            |> List.fold (fun acc (param, value) ->
                                match value with
                                | Computed v -> Map.add param v acc
                                | _ -> failwith "Error while creating new closure context"
                            ) context
                        let newEnv = Environment(newContext, functions)
                        let result, e = evalScope body newEnv
                        (result, e)
                | None ->
                    failwithf $"Function %s{funcName} not found"
    and
        private handleOperationCase (left: Expression) (op: Operator) (right: Expression) (env: Environment) =
           let leftExprResult, _ = eval left env
           let rightExprResult, _ = eval right env
           match leftExprResult, rightExprResult with
           | Computed value1, Computed value2 ->
               let func = funof op
               let value = func value1 value2 
               (Computed value, env)
           | _ -> failwith "Operation expression should contain only calculating expression without return"
           
    
    let evaluate (astTree: Expression list) =
        let initEnv = Environment(Map.empty, Map.empty)
        let finalEnv = List.fold (fun (_, env) expr -> eval expr env) (Computed unit, initEnv) astTree
        finalEnv
        