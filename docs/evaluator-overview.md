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

Input:
"let x = 5"
Output:
=============
Input:
"
let x = 5
dump x
"
Output:
"5"
=============
Input:
"
let x = 5
dump "Hi! " * x
"
Output:
"Hi! Hi! Hi! Hi! Hi! "
=============
Test 2: If conditions

Input:
"
let x = 5
if (x == 5) {
    dump "congrats"
}
"
Output:
"congrats"
=============
Input:
"
let x = 5
if (x != 5) {
    dump "congrats"
} else {
    dump "oh no..."
}
"
Output:
"oh no..."
=============
Input:
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
Output:
"congrats"
"end of program..."
=============
Input:
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
Output:
"128"
"223"
"end of program..."
=============
Test 3: Function testing

Input:
"
func printSum [a,b] {
    dump a+b
}

printSum[100,300]
"
Output:
"400"
=============
Input:
"
func printSum [a,b] {
    return (a + b)
}

let x = printSum[100,300]
dump x
"
Output:
"400"
=============
Input:
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
Output:
"120"
=============
Input:
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
Output:
"2"
"4"
=============
Input:
"
let x = 5 + 2 * 5
dump x
"
Output:
"15"
=============
Input:
"
let x = (5 + 2) * 5
dump x
"
Output:
"35"
=============
Test 4: List testing

Input:
"
let l = [1, 2, 3]
dump l
"
Output:
"[1, 2, 3]"
=============
Input:
"
let l1 = [1, 2, 3]
let l2 = [3,4,5]
dump (l1 + l2)
"
Output:
"[1, 2, 3, 3, 4, 5]"
=============
Input:
"
let l1 = [1, 2, 3]
let l2 = [3,4,5]
dump (l1 * l2)
"
Output:
"[1, 2, 3, 4, 5]"
=============
Input:
"
let l1 = [1, 2, 3]
let l2 = [1, 2]
dump (l1 / l2)
"
Output:
"[1, 2]"
=============
Input:
"
let l1 = [1, 2, 3]
let l2 = l1
dump l2
"
Output:
"[1, 2, 3]"
=============
Input:
"
func f[] {
    let x = [1, 2]
    return x
}
let l1 = f[]
dump l1
"
Output:
"[1, 2]"
=============
Input:
"
let l = [1, 2, 3]
l <- (l add 5)
dump l
"
Output:
"[1, 2, 3, 5]"
=============
Input:
"
let l = [1, 2, 3]
l <- (l remove 2)
dump l
"
Output:
"[1, 3]"
=============
Input:
"
let l1 = [1, 2, 3]
let l2 = (l1 remove 2)
dump l2
"
Output:
"[1, 3]"
=============
Test 5: IO operations testing

Input:
"
let f = file("/home/denis/fsharp-lab-test/test.txt")
let t = f readFile "text" 
dump t
"
Output:
"test"
=============
Input:
"
let f = file("/home/denis/fsharp-lab-test/test.txt")
let lines = f readFile "lines" 
dump lines
"
Output:
"[test]"
=============
Input:
"
let f = file("/home/denis/fsharp-lab-test/test.txt")
f writeFile "some_text_variable" 
"
Output:
=============
Input:
"
let l = [1,2,3]
let f = file("/home/denis/fsharp-lab-test/test.txt")
f writeFile l 
"
Output:
=============
```