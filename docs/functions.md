# Functions

Also support recursive functions (see `recursion.md`)

### Function with `return` statement 

```python
func printSum [a,b] {
    return (a + b)
}

let x = printSum[100,300]
dump x
```

### Function without `return`

```python
func printSum [a,b] {
    dump a+b
}

printSum[100,300]
```


### Output

```python
"400"
"400"
```

### Description of the Example

- Functions in this language behave similarly to **Python** functions. They can optionally use a return statement to provide a value, but it is not mandatory. A function can simply perform an operation without returning anything.