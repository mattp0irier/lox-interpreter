using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trmpLox
{
    internal class Parser
    {
        readonly List<Token> tokens;
        int cur = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expression Parse()
        {
            return Expr();
        }

        Expression Expr()
        {
            return Equality();
        }

        Expression Equality()
        {
            Expression expr = Comparison();

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token op = Previous();
                Expression right = Comparison();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        Expression Comparison()
        {
            Expression expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token op = Previous();
                Expression right = Term();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        Expression Term()
        {
            Expression expr = Factor();

            while (Match(TokenType.PLUS, TokenType.MINUS))
            {
                Token op = Previous();
                Expression right = Factor();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        Expression Factor()
        {
            Expression expr = Unary();

            while (Match(TokenType.STAR, TokenType.SLASH))
            {
                Token op = Previous();
                Expression right = Unary();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        Expression Unary()
        {
            if(Match(TokenType.BANG, TokenType.MINUS))
            {
                Token op = Previous();
                Expression right = Unary();
                return new Unary(op, right);
            }

            return Primary();
        }

        Expression Primary()
        {
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.NIL)) return new Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Literal(Previous().GetLiteral());
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expression expr = Expr();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
            }
            Console.WriteLine("Found nothing in Primary()");
            return new Literal(null);
        }

        Boolean Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            else Console.WriteLine(message);
            return new Token(TokenType.EOF, "Error", "Error", -1);
        }

        Boolean Check(TokenType type)
        {
            if (AtEnd()) return false;
            return Peek().GetType() == type;
        }

        Token Advance()
        {
            if (!AtEnd()) cur++;
            return Previous();
        }

        Boolean AtEnd()
        {
            return Peek().GetType() == TokenType.EOF;
        }

        Token Peek()
        {
            return tokens[cur];
        }

        Token Previous()
        {
            return tokens[cur - 1];
        }
    }
}
