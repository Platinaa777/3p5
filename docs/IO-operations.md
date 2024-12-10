# IO Operations

### Code `readFile "text"`

```python
let f = file("/home/denis/fsharp-lab-test/test.txt")
let t = f readFile "text" 
dump t
```

### Output

```python
test1
test2
```

Have test1 and test2 on different lines because separater was `\n`

---

### Code `readFile "lines"`

```python
let f = file("/home/denis/fsharp-lab-test/test.txt")
let lines = f readFile "lines" 
dump lines
```

### Output

```python
[test1, test2]
```

---

### Code `writeFile`

```python
let f = file("/home/denis/fsharp-lab-test/test.txt")
f writeFile "some_text_variable" 
```

### Output

no output, `writeFile` does not return it,
but in file we can notice - `some_test_variable`

---

### Code `writeFile`

```python
let l = [1,2,3]
let f = file("/home/denis/fsharp-lab-test/test.txt")
f writeFile l 
```

### Output

no output, `writeFile` does not return it,
but in file we can notice - `[1, 2, 3]`
