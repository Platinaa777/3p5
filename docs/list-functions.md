# List functions

### Concatenation list

```python
let l1 = [1, 2, 3]
let l2 = [3,4,5]
dump (l1 + l2)
```

```python
[1, 2, 3, 3, 4, 5]
```

---

### Union lists

```python
let l1 = [1, 2, 3]
let l2 = [3,4,5]
dump (l1 * l2)
```

```python
[1, 2, 3, 4, 5]
```

---

### Intersection lists

```python
let l1 = [1, 2, 3]
let l2 = [1, 2]
dump (l1 / l2)
```

```python
[1, 2]
```

---

### Add element

```python
let l = [1, 2, 3]
l <- (l add 5)
dump l
```

```python
[1, 2, 3, 5]
```

---

### Remove element

```python
let l = [1, 2, 3]
l <- (l remove 2)
dump l
```

```python
[1, 3]
```