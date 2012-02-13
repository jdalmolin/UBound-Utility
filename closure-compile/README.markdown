Closure Compiler python script 
==============================

modified from the official google script located [Here](http://code.google.com/closure/compiler/docs/api-tutorial1.html) 

usage:
-------

```python closure.py {filename}```

It will read the first argument, which should contain a filename (and correct path if applicable). It will submit the contents to the Closure Compiler service - trimming whitespace and comments then write the file back to its calling directory with a .min.js extension.


