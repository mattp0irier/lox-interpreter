using System;
using System.IO;
using System.Collections.Generic;

namespace trmpLox
{
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
        public string lexeme;
        object literal;
        int line;

        // Token: constructor for tokens
        public Token(TokenType type, string lexeme, object literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public new TokenType GetType()
        {
            return this.type;
        }

        public new Object GetLiteral()
        {
            return this.literal;
        }

        public String toString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }

}
