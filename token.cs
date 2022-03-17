using System;

namespace LoxInterpreter {

    enum TokenType 
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

    class Token {
        TokenType type;
        String lexeme;
        Object literal;
        int line; 

        // Token: constructor for tokens
        Token(TokenType type, string lexeme, object literal, int line) {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public String toString() {
            return type + " " + lexeme + " " + literal;
        }
    }
}