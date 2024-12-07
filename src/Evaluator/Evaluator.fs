namespace Evaluator

open Parser

type Closure =
    Closure of parameters:Id list * body:Statement list * env:Environment
and
    Environment = Environment of context: Map<Id, Expression> * functions: Map<Id, Closure>

module Evaluator =
    
    let eval tree env =
        1           

    let toString evaluationResult =
       "result" 
    
    let evaluate (astTree: Statement list) =
        let res = eval astTree Map.empty
        toString res
        