using System;
using System.IO;
using System.Collections.Generic;


namespace trmpLox
{
    class TrMpLox
    {
        public static bool hadError = false;
        public static bool hadRunTimeError = false;
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

            if (hadError) System.Environment.Exit(65);
            if (hadRunTimeError) System.Environment.Exit(70);
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

            if (hadError) return;

            interpreter.interpret(stmts);
        }
        public static void Error(int line, string msg)
        {
            Report(line, "", msg);
        }

        public static void Error(Token token, string msg)
        {
            if (token.type == TokenType.EOF)
                Report(token.line, " at end", msg);
            else
            {
                Report(token.line, "at '" + token.lexeme + "'", msg);
            }
        }

        public static void RuntimeError(runtimeError error)
        {
            Console.Error.WriteLine(error.Message + "\n[line: " + error.token.line + "]");
            hadRunTimeError = true;
        }

        public static void Report(int line, string where, string msg)
        {
            Console.Error.WriteLine("[line " + line + "] error" + where + ": " + msg);
            hadError = true;
        }

    }

}