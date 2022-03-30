using System;
using System.IO;
using System.Collections.Generic;

namespace trmpLox
{
    class TrMpLox
    {
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: lox-interpreter [filename]");
                return;
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        static void RunPrompt()
        {
            Interpreter interpreter = new Interpreter();
            string curLine = "initial value";
            while (true)
            {
                Console.Write("> ");
                curLine = Console.ReadLine();
                if (curLine.Length == 0) break;
                Run(curLine, interpreter);
            }
        }

        static void RunFile(string filename)
        {
            string input = File.ReadAllText(filename);
            Run(input, null);
        }

        static void Run(string line, Interpreter? interpreter)
        {
            Scanner scanner = new(line);
            List<Token> tokens = scanner.scanTokens();
            Parser parser = new(tokens);
            List<Statement> stmts = parser.Parse();
            if (interpreter == null) interpreter = new Interpreter();


            //Console.WriteLine("Didn't Die");


            //foreach (Token cur in tokens)
            //{
            //Console.WriteLine("there is a token here");
            //Console.WriteLine(cur.toString());
            //}

            // Console.WriteLine();
            // Console.WriteLine(expr);
            // Console.WriteLine();
            Resolver resolver = new Resolver(interpreter);
            resolver.Resolve(stmts);

            interpreter.interpret(stmts);
        }

    }

}