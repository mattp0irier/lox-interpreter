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
            string curLine = "initial value";
            while (true)
            {
                Console.Write("> ");
                curLine = Console.ReadLine();
                if (curLine.Length == 0) break;
                Run(curLine);
            }
        }

        static void RunFile(string filename)
        {
            string input = File.ReadAllText(filename);
            Run(input);
        }

        static void Run(string line)
        {
            Scanner scanner = new(line);
            List<Token> tokens = scanner.scanTokens();
            Parser parser = new(tokens);
            Expression expr = parser.Parse();
            Interpreter interpreter = new Interpreter();

            Console.WriteLine("Didn't Die");
            

            foreach (Token cur in tokens)
            {
                //Console.WriteLine("there is a token here");
                Console.WriteLine(cur.toString());
            }

            Console.WriteLine();
            Console.WriteLine(expr);
            Console.WriteLine();


            interpreter.interpret(expr);
        }

    }

}