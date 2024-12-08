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