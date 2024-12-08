# Parser

This parser is written in F# using the FParsec library. 
It is designed to parse a custom programming language with 
support for various data types, operations, and constructs. 
Below is a detailed overview of the parser's functionality and structure.

---

## Key Features

### Modular design

The parser is built using a modular approach, where smaller 
parsers are combined to construct more complex ones. For example:

- Individual parsers handle integers, floats, strings, booleans, and lists.
- These parsers are reused to parse expressions, variables, and functions, creating a cohesive and maintainable structure.

### Precedence and associativity

The parser includes an operator precedence table using `OperatorPrecedenceParser`. This allows:

- Operators like `+`, `-`, `*`, `/` to follow arithmetic precedence rules.
- Logical operators (`and`, `or`) and comparison operators (`>`, `<=`, `==`, etc.) to be evaluated correctly.
- Left associativity.

### Valid Variable Names

Variable names follow specific rules for validity:

- Can start with a `[a-zA-z]`, `@`, or `_`.
- Subsequent characters can include letters or digits.

### Supported Data Types

The parser recognizes the following data types:

- **Integer**: Whole numbers (e.g., `42`).
- **Float**: Decimal numbers (e.g., `3.14`).
- **Boolean**: `true` or `false`.
- **String**: Enclosed in double quotes (e.g., `"hello"`).
- **List**: A collection of elements, all of the same type, enclosed in square brackets (e.g., `[1, 2, 3]`).
- **File**: A special type representing a file path (e.g., `file("path/to/file")`). File existence is validated during parsing.

--- 

## Tests for creating AST and parsing

### Test 1: Parsing simple values


```python
value: "code1da sdas das dsada"
# Output:
Str "code1da sdas das dsada"

value: true
# Output:
Bool true

value: false
# Output:
Bool false

value: 3.21312312321
# Output:
Float 3.213123123

value: 51312312
# Output:
Int 51312312
```

### Test 2: Parsing literal expressions

```python
string literal: "dsada"
# Output:
Str "dsada"

bool literal: true
# Output:
Bool true

bool literal: false
# Output:
Bool false

int literal: 123123
# Output:
Int 123123

float literal: 3.141231232131
# Output:
Float 3.141231232
```

### Test 3: Testing `let`

```python
case: let x = 1
# Output:
Let ("x", Literal (Int 1))

case: let xx1323123 = 2
# Output:
Let ("x1323123", Literal (Int 2))

case: let _x132 = 3132
# Output:
Let ("_x132", Literal (Int 3132))
```
### Test 4: Parsing binary expressions

```python
case 1 + 3
# Output:
Operation (Literal (Int 1), Add, Literal (Int 3))

case 1 - x
# Output:
Operation (Literal (Int 1), Subtract, Variable "x")

case y / x
# Output:
Operation (Variable "y", Divide, Variable "x")

case y and x
# Output:
Operation (Variable "y", And, Variable "x")

```

### Test 5: `let` testing and difficult operations (formatted)

```python
case 'let x = 5'
# Output:
Let ("x", Literal (Int 5))

case 'let x = 1 + 5'
# Output:
Let ("x", Operation (Literal (Int 1), Add, Literal (Int 5)))

case 'let x = y + z'
# Output:
Let ("x", Operation (Variable "y", Add, Variable "z"))

case 'let x = (y + z) + t'
# Output:
Let ("x", Operation (Operation (Variable "y", Add, Variable "z"), Add, Variable "t"))

case 'let x = (y + z) + (t + d)'
# Output:
Let ("x", 
    Operation (
     Operation (Variable "y", Add, Variable "z"),
     Add,
     Operation (Variable "t", Add, Variable "d")))

case 'let x = (y + z) + (t + d) + 5'
# Output:
Let ("x", 
    Operation (
        Operation (Operation (Variable "y", Add, Variable "z"),
         Add, Operation (Variable "t", Add, Variable "d")),
          Add, Literal (Int 5)))

case 'let x = (((y + z) + t) + 5)'
# Output:
Let ("x", 
    Operation (
        Operation (
            Operation (Variable "y", Add, Variable "z"), Add, Variable "t"),
                Add,
                Literal (Int 5)))

case 'let x = 1 + 2 + 3'
# Output:
Let ("x", 
    Operation (
        Operation (Literal (Int 1), Add, Literal (Int 2)),
            Add,
            Literal (Int 3)))
```

### Test 6: If condition (formatted)

```python
"
if x == 5 {
    let z = 10
} else {
    let t = 5
}
"
# Output:
Condition (
    Operation (Variable "x", Equal, Literal (Int 5)),
     [Let ("z", Literal (Int 10))],
     Some [Let ("t", Literal (Int 5))])

"
if x == 5 {
    let z = 10
}
"
# Output:
Condition (
    Operation (Variable "x", Equal, Literal (Int 5)),
    [Let ("z", Literal (Int 10))], 
    None)

"
if x >= 5 {
    let z = 10
} else {
    let t = 5
}
"
# Output:
Condition (
    Operation (Variable "x", GreaterThanOrEqual, Literal (Int 5)),
    [Let ("z", Literal (Int 10))],
    Some [Let ("t", Literal (Int 5))])

"
if x == 5 {
    let z = 10
    if (x != 10) {
        let xx = 123
    }
} else {
    let t = 5
}
"
# Output:
Condition (
    Operation (Variable "x", Equal, Literal (Int 5)),
    [Let ("z", Literal (Int 10));
        Condition (Operation (Variable "x", NotEqual, Literal (Int 10)),
        [Let ("xx", Literal (Int 123))], 
        None)],
    Some [Let ("t", Literal (Int 5))])

```

### Test 7: Dump testing


```python
case: dump x + 5
# Output:
Dump (Operation (Variable "x", Add, Literal (Int 5)))

case: dump (x + 5)
# Output:
Dump (Operation (Variable "x", Add, Literal (Int 5)))
```

### Test 8: FuncDef parsing

```python
"
func f [a,b] {
    let x = 5
    let y = 10
}
"
# Output:
FuncDef ("f", ["a"; "b"],
    [Let ("x", Literal (Int 5));
    Let ("y", Literal (Int 10))])

"
func f [a,b] {
    if (a == b) {
        let x = 5
    } else {

    }
}
"
# Output:
FuncDef ("f", ["a"; "b"],
    [Condition (
        Operation (Variable "a", Equal, Variable "b"),
        [Let ("x", Literal (Int 5))], 
        Some [])])

"
func f [a,b] {
    func ff [x,y] {
        let t = 5
        let t2 = x == y
    }
}
"
# Output:
FuncDef ("f", ["a"; "b"],
    [FuncDef ("ff", ["x"; "y"],
        [Let ("t", Literal (Int 5));
        Let ("t2", Operation (Variable "x", Equal, Variable "y"))])
    ])

```

### Test 9: FuncCall parsing

```python
"
f [x, y]
"
# Output:
FuncCall ("f", [Variable "x"; Variable "y"])

"
f []
"
# Output:
FuncCall ("f", [])

"
f [1, 2]
"
# Output:
FuncCall ("f", [Literal (Int 1); Literal (Int 2)])

# Test 10: Test many statements
"
if (x == 5) {
    let z = 5
}

let x = 5

func f [a,b] {
    let c = a + b
}
"
# Output:
[Condition (
    Operation (Variable "x", Equal, Literal (Int 5)),
    [Let ("z", Literal (Int 5))], 
    None);
Let ("x", Literal (Int 5));
FuncDef ("f", ["a"; "b"],
    [Let ("c", Operation (Variable "a", Add, Variable "b"))])]

"
let x = 5
let y = 10
let z = x + y
"
# Output:
[Let ("x", Literal (Int 5));
Let ("y", Literal (Int 10)); 
Let ("z", Operation (Variable "x", Add, Variable "y"))]

"
func f1 [a,b] {
    let c = a + b
}
func f2 [x,y] {
    let zz = x + y
}
"
# Output:
[FuncDef ("f1", ["a"; "b"], 
    [Let ("c", Operation (Variable "a", Add, Variable "b"))]);
FuncDef ("f2", ["x"; "y"], 
    [Let ("zz", Operation (Variable "x", Add, Variable "y"))])]

```

### Test 11: List parsing

```python
"
let l = [1,2,3]
"
# Output:
[Let ("l", Literal (List [Int 1; Int 2; Int 3]))]

"
let l = ["H","S","E"]
"
# Output:
[Let ("l", Literal (List [Str "H"; Str "S"; Str "E"]))]

"
let l = [true,true,false]
"
# Output:
[Let ("l", Literal (List [Bool true; Bool true; Bool false]))]

```

### Test 12: IO parsing

```python
"
let l = file("/home/denis/trash/fsharp-lab-test/test.txt")
"
# Output:
[Let ("l", Literal (File "/home/denis/trash/fsharp-lab-test/test.txt"))]

"
let l = file("$invalid_path$")
"
# Output:
[]
```