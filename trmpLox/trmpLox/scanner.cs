using System;
using System.IO;
using System.Collections.Generic;
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
                if (match('/'))
                {
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
        string text = source.Substring(start, cur - start);
        tokens.Add(new Token(type, text, literal, line));
    }

    bool done()
    {
        return cur >= source.Length;
    }
}