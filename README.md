# Lox Interpreter in C#
## Matt Poirier and Trevor Russell

This project is for CS 403 Spring 2022. A subset of a Lox interpreter was created in C#. Robert Nystrom's JLox interpreter from his textbook *Crafting Interpreters* was used as a guide to create our interpreter.

---
## Compilation Instructions
This project was built and compiled inside of Visual Studio 2022 using .NET 6.0.

The command used to compile the project is `msbuild trmpLox.csproj`.

To run in User Input mode:
`./bin/Debug/net6.0/trmpLox`

To run with an input file:
`./bin/Debug/net6.0/trmpLox filename`

---
## Test Cases and Output
Several tests were run on the interpreter in order to verify the interpreter was working. Included test files are:

- simpleStuff.lox: Basic tests of all operations and functions; expected value for each result is printed
- testInput.lox: Creates a few functions and tests various operations using them

The outputs of our tests are stored in the files named `testNameOutput`.

---
## Limitations
Our project does not implement classes or any sort of inheritance that comes with it. (Chapters 12 and 13 of *Crafting Interpreters*.)