# Evaluator

This document explains the implementation details of the Evaluator module, 
focusing on supported operations, their behaviors, and type compatibility.

---

## Supported Operations

### Arithmetic Operations

- Addition (`+`):
  - Supported Types:
    - `Int` + `Int` -> `Int`
    - `Float` + `Float` → `Float`
    - `Int` + `Float` / `Float` + `Int` → `Float`
    - `Str` + `Str` → `Str` (concatenation)
    - `List` + `List` → `List` (concatenation)

- Subtraction (`-`):
  - Supported Types:
    - `Int` - `Int` → `Int`
    - `Float` - `Float` → `Float`
    - `Int` - `Float` / `Float` - `Int` → `Float`

- Multiplication (`*`):
  - Supported Types:
    - `Int` * `Int` → `Int`
    - `Float` * `Float` → `Float`
    - `Int` * `Float` / `Float` * `Int` → `Float`
    - `Str` * `Int` → `Str` (string repetition)
    - `List` * `List` → `List` (union of unique elements)

- Division (`/`):
  - Supported Types:
    - `Int` / `Int` → `Int` (integer division, error on division by zero)
    - `Float` / `Float` → `Float`
    - `Int` / `Float` / `Float` / `Int` → `Float`
    - `List` / `List` → `List` (intersection of elements)

- Modulo (`%`):
  - Supported Types:
    - `Int` % `Int` → `Int`
    - `Float` % `Float` → `Float`

### Comparison Operations

- Greater Than (`>`), Less Than (`<`):
  - **Supported Types:** 
    - Int
    - Float

- Greater Than or Equal (`>=`), Less Than or Equal (`<=`):
  - **Supported Types:** 
    - Int
    - Float

- Equality (`==`), Inequality (`!=`):
  - **Supported Types:**
    - Int
    - Float
    - Str
    - Bool

### Logical Operations

- And (`and`), Or (`or`):
  - **Supported Types:** 
    - Bool

---

# Tests for evalautor

```python
Test 1: Evaluate simple cases

"let x = 5"
=============
"
let x = 5
dump x
"
"5"
=============
"
let x = 5
dump "Hi! " * x
"
"Hi! Hi! Hi! Hi! Hi! "
=============
Test 2: If conditions

"
let x = 5
if (x == 5) {
    dump "congrats"
}
"
"congrats"
=============
"
let x = 5
if (x != 5) {
    dump "congrats"
} else {
    dump "oh no..."
}
"
"oh no..."
=============
"
let x = 5
let z = 123
if (z > x) {
    dump "congrats"
} else {
    dump "oh no..."
}

dump ("end of program...")
"
"congrats"
"end of program..."
=============
"
let x = 5
let z = 123
if (true) {
    dump x + z
    x <- 100
    dump x + z
} else {
    dump "oh no..."
}

dump ("end of program...")
"
"128"
"223"
"end of program..."
=============
Test 3: Function testing

"
func printSum [a,b] {
    dump a+b
}

printSum[100,300]
"
"400"
=============
"
func printSum [a,b] {
    return (a + b)
}

let x = printSum[100,300]
dump x
"
"400"
=============
"
func fact [n] {
    if n == 1 {
        return 1
    } else {
        let f = fact[n - 1]
        return (n * f)
    }
}

let res = fact[5]
dump res
"
"120"
=============
"
let x = 1

func f [] {
    x <- x + x
}

f[]
dump x
f[]
dump x
"
"2"
"4"
=============
"
let x = 5 + 2 * 5
dump x
"
"15"
=============
"
let x = (5 + 2) * 5
dump x
"
"35"
=============
Test 4: List testing

"
let l = [1, 2, 3]
dump l
"
"[1, 2, 3]"
=============
"
let l1 = [1, 2, 3]
let l2 = [3,4,5]
dump (l1 + l2)
"
"[1, 2, 3, 3, 4, 5]"
=============
"
let l1 = [1, 2, 3]
let l2 = [3,4,5]
dump (l1 * l2)
"
"[1, 2, 3, 4, 5]"
=============
"
let l1 = [1, 2, 3]
let l2 = [1, 2]
dump (l1 / l2)
"
"[1, 2]"
=============
"
let l1 = [1, 2, 3]
let l2 = l1
dump l2
"
"[1, 2, 3]"
=============
"
func f[] {
    let x = [1, 2]
    return x
}
let l1 = f[]
dump l1
"
"[1, 2]"
=============
"
let l = [1, 2, 3]
l <- (l add 5)
dump l
"
"[1, 2, 3, 5]"
=============
"
let l = [1, 2, 3]
l <- (l remove 2)
dump l
"
"[1, 3]"
=============
"
let l1 = [1, 2, 3]
let l2 = (l1 remove 2)
dump l2
"
"[1, 3]"
=============
Test 5: IO operations testing

"
let f = file("/home/denis/trash/fsharp-lab-test/test.txt")
let t = f readFile "text" 
dump t
"
"test"
=============
"
let f = file("/home/denis/trash/fsharp-lab-test/test.txt")
let lines = f readFile "lines" 
dump lines
"
"[test]"
=============
"
let f = file("/home/denis/trash/fsharp-lab-test/test.txt")
f writeFile "some_text_variable" 
"
=============
"
let l = [1,2,3]
let f = file("/home/denis/trash/fsharp-lab-test/test.txt")
f writeFile l 
"
=============
```