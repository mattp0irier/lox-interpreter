using System;
using System.IO;

namespace LoxInterpreter {
    class Lox {
        static void Main(string[] args) {
            if(args.Length > 1) {
                Console.WriteLine("Invalid arguments");
            }
            else if (args.Length == 1) {
                runFile(args[0]);
            }
            else {
                runPrompt();
            }
        }

        // runPrompt: prompt user for input until no prompt is given
        void runPrompt(){
            string line;
            while (1){
                // prompt user for input
                Console.Write("> ");
                line = Console.ReadLine();

                // terminate if user gives blank command
                if (line.length() == 0) break;
                run(line);
            }
        }

        // runFile: run file passed as argument
        void runFile(string fileName) {
            string content = File.ReadAllText("filename");
            run(content);
        }

        void run(string content) {
            // cerate new scanner
            Scanner scanner(content);

            // separate user input into tokens
            vector<Token> tokens = scanner.scanTokens();
            // scanner.printTokens();

            // Objects used for parsing and interpreting expressions as they are read in
            // Parser *parser = new Parser(tokens);
            // Interpreter interpreter;
            // EXP *currentExpression;
            // string resultString;

            // // First expression is necessary before entering loop
            // currentExpression = parser->getNextExpression();
            // // Expressions return S-Expressions
            // S_EXP *result;

            // while(currentExpression != NULL) {
            //     result = interpreter.eval(currentExpression, emptyEnv());

            //     // print function returns null and does not need to be printed again
            //     if (result != NULL) {
            //         // Use respective toString() function based on result type
            //         if (result->type == "Number") {
            //             NUM_SXP* n = (NUM_SXP*)result;
            //             cout << n->toString() << endl;
            //         }
            //         else if (result->type == "Symbol") {
            //             SYM_SXP* n = (SYM_SXP*)result;
            //             cout << n->toString() << endl;
            //         }
            //         else if (result->type == "List") {
            //             LIST_SXP* n = (LIST_SXP*)result;
            //         // cout << n->toString() << endl;
            //         }
            //         else if (result->type == "TRUE"){
            //             cout << "T" << endl;
            //         }
            //         else if (result->type == "()"){
            //             cout << result->type << endl;
            //         }
            //         else {
            //             // we shouldn't get here
            //             cout << result->type << endl;
            //             cout << "error" << endl;
            //         }
            //     }
            //     // returns NULL if no more expressions are found
            //     currentExpression = parser->getNextExpression();
            // }
        }
    }
}