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

    public enum TokenType
    {
        // One-chars
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

        // Comparitors and Equal
        BANG, BANG_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,

        // Literals
        IDENTIFIER, STRING, NUMBER,

        // Other Keywords
        AND, ELSE, FALSE, FUN, FOR, IF, NIL, OR,
        PRINT, RETURN, THIS, TRUE, VAR, WHILE,

        EOF
    };

    public class Token
    {
        TokenType type;
        String lexeme;
        Object literal;
        int line;

        // Token: constructor for tokens
        public Token(TokenType type, string lexeme, object literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public String toString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }

    public class Scanner
    {
        String source;
        List<Token> tokens = new List<Token>();
        int start = 0;
        int cur = 0;
        int line = 1;

        public Scanner(String source)
        {
            this.source = source;
        }

        public List<Token> scanTokens()
        {
            while (!done())
            {
                start = cur;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        void scanToken()
        {
            char c = advance();
            switch (c)
            {
                //SINGLE-CHARACTER OPERATIONS
                case '(':
                    addToken(TokenType.LEFT_PAREN); break;
                case ')':
                    addToken(TokenType.RIGHT_PAREN); break;
                case '{':
                    addToken(TokenType.LEFT_BRACE); break;
                case '}':
                    addToken(TokenType.RIGHT_BRACE); break;
                case ',':
                    addToken(TokenType.COMMA); break;
                case '.':
                    addToken(TokenType.DOT); break;
                case '-':
                    addToken(TokenType.MINUS); break;
                case '+':
                    addToken(TokenType.PLUS); break;
                case ';':
                    addToken(TokenType.SEMICOLON); break;
                case '*':
                    addToken(TokenType.STAR); break;


                //MIGHT BE SINGLE OR TWO-CHARACTER OPS
                case '!':
                    if (match('=')) addToken(TokenType.BANG_EQUAL);
                    else addToken(TokenType.BANG);
                    break;
                case '=':
                    if (match('=')) addToken(TokenType.EQUAL_EQUAL);
                    else addToken(TokenType.EQUAL);
                    break;
                case '<':
                    if (match('=')) addToken(TokenType.LESS_EQUAL);
                    else addToken(TokenType.LESS);
                    break;
                case '>':
                    if (match('=')) addToken(TokenType.GREATER_EQUAL);
                    else addToken(TokenType.GREATER);
                    break;

                //CHECK FOR COMMENTS
                case '/':
                    if (match('/')){
                        // don't read the rest of the line if it's a comment
                        while (peek() != '\n' && !done()) advance();
                    }
                    else addToken(TokenType.SLASH);
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                case '"':
                    scanString();
                    break;

                default:
                    //FIXME: add number literals and keywords
                    Console.WriteLine("Unexpected Chracter"); break;

            }
        }

        void scanString()
        {
            while (peek() != '"' && !done())
            {
                if (peek() == '\n') line++;
                advance();
            }
            if (done())
            {
                Console.WriteLine("Unterminated String");
                return;
            }
            advance();

            string value = source.Substring(start + 1, cur - start - 2);
            addToken(TokenType.STRING, value);
        }

        bool match(char test)
        {
            if (done()) return false;
            if (source[cur] != test) return false;
            cur++;
            return true;
        }

        char peek()
        {
            if (done()) return '\0';
            return source[cur];
        }

        char advance()
        {
            return source[cur++];
        }

        void addToken(TokenType type)
        {
            addToken(type, null);
        }

        void addToken(TokenType type, object literal)
        {
            string text = source.Substring(start, cur-start);
            tokens.Add(new Token(type, text, literal, line));
        }

        bool done()
        {
            return cur >= source.Length;
        }
    }

}