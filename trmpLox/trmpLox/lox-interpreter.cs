using System;
using System.IO;
using System.Collections.Generic;

namespace LoxInterpreter
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
                runFile(args[0]);
            }
            else
            {
                runPrompt();
            }
        }

        static void runPrompt()
        {
            string curLine;
            while (true)
            {
                Console.Write("> ");
                curLine = Console.ReadLine();
                if (curLine.Length == 0) break;
                run(curLine);
            }
        }

        static void runFile(string filename)
        {
            string input = File.ReadAllText(filename);
            run(input);
        }

        static void run(string line)
        {
            Console.WriteLine("Okay, I'll run the line");
            Scanner scanner = new Scanner(line);
            List<Token> tokens = scanner.scanTokens();

            foreach (Token cur in tokens)
            {
                //Console.WriteLine("there is a token here");
                Console.WriteLine(cur.toString());
            }
        }

    }

}