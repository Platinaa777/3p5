# Closures

### Code:

```python
let x = 1

func f [] {
    x <- x + x
}

f[]
dump x
f[]
dump x
"
```

### Output:

```python
"2"
"4"
```