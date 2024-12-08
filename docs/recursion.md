# Recursion

## Calculating factorial

```python
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
```

### Output

```
"120"
```

### Description of the Example

- This intermediate calculation is necessary because function calls **cannot be directly passed in the return statement**. Instead, the result of the function must be computed and assigned to a variable first. `let f = fact[n - 1]`
- **Variable Assignment:** The variable `res` stores the result of calling the fact function with `5` as the input.
- **Output**: The final result, the factorial of `5`, is printed using the dump command, which outputs `"120"`.