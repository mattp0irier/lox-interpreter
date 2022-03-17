using System;
using System.Collections;

namespace LoxInterpreter {

    class Scanner {
        private String source;
        private List<Token> tokens = new List<Token>();
        int start = 0;
        int cur = 0;
        int line = 1;

        Scanner(String source) {
            this.source = source;
        }

        List<Token> scanTokens()
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
            switch (c) {
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
                default:
                    Console.WriteLine("Unexpected Chracter"); break;

            }
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
            string text = source.Substring(start, current);
            tokens.Add(new Token(type, text, literal, line));
        }

        bool done()
        {
            return cur >= source.Length;
        }
    }

}