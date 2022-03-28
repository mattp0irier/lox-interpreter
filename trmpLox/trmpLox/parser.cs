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

        public List<Statement> Parse()
        {
            List<Statement> statements = new();
            while (!AtEnd())
            {
                statements.Add(Declaration());
            }
            return statements;
        }

        Expression Expr()
        {
            return Assignment();
        }

        Statement Declaration()
        {
            if (Match(TokenType.VAR)) return VarDeclaration();
            return Stmt();
        }

        Statement VarDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expression? initializer = null;
            if (Match(TokenType.EQUAL))
            {
                initializer = Expr();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new VarStmt(name, initializer);
        }

        Statement Stmt()
        {
            if (Match(TokenType.FOR)) return forStmt();
            if (Match(TokenType.IF)) return ifStmt();
            if (Match(TokenType.PRINT)) return PrintStmt();
            if (Match(TokenType.WHILE)) return WhileStmt();
            if (Match(TokenType.LEFT_BRACE)) return new BlockStmt(Block());

            return ExprStmt();
        }

        Statement forStmt()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");
            Statement? initializer;
            if (Match(TokenType.SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(TokenType.VAR))
            {
                initializer = VarDeclaration();
            }
            else
            {
                initializer = ExprStmt();
            }

            Expression? condition = null;
            if (!Check(TokenType.SEMICOLON))
            {
                condition = Expr();
            }
            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

            Expression? increment = null;
            if (!Check(TokenType.SEMICOLON))
            {
                increment = Expr();
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

            Statement body = Stmt();

            if (increment != null)
            {
                body = new BlockStmt(new List<Statement>{ body, new ExpressionStmt(increment) });
            }

            if (condition == null)
            {
                condition = new Literal(true);
            }
            body = new WhileStmt(condition, body);

            if (initializer != null)
            {
                body = new BlockStmt(new List<Statement> { initializer, body });
            }


            return body;
        }

        Statement WhileStmt()
        {
            Consume(TokenType.LEFT_PAREN, "Expect  '(' after 'while'.");
            Expression condition = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            Statement body = Stmt();

            return new WhileStmt(condition, body);
        }

        Statement ifStmt()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            Expression condition = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

            Statement thenBranch = Stmt();
            Statement? elseBranch = null;
            if (Match(TokenType.ELSE))
            {
                elseBranch = Stmt();
            }

            return new IfStmt(condition, thenBranch, elseBranch);
        }

        Statement PrintStmt()
        {
            Expression expr = Expr();
            Consume(TokenType.SEMICOLON, "Expect ; after value.");
            return new PrintStmt(expr);
        }

        Statement ExprStmt()
        {
            Expression expr = Expr();
            Consume(TokenType.SEMICOLON, "Expect ; after value.");
            return new ExpressionStmt(expr);
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

            if (Match(TokenType.IDENTIFIER))
            {
                return new Variable(Previous());
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

        private Expression Assignment()
        {
            Expression expression = Or();

            if (Match(TokenType.EQUAL))
            {
                Token equals = Previous();
                Expression value = Assignment();

                if (expression is Variable) {
                    Token name = ((Variable)expression).name;
                    return new Assign(name, value);
                }
            }

            return expression;
        }

        private Expression Or()
        {
            Expression expr = And();

            while (Match(TokenType.OR))
            {
                Token op = Previous();
                Expression right = And();
                expr = new Logical(expr, op, right);
            }

            return expr;
        }

        private Expression And()
        {
            Expression expr = Equality();

            while (Match(TokenType.AND))
            {
                Token op = Previous();
                Expression right = Equality();
                expr = new Logical(expr, op, right);
            }

            return expr;
        }

        private List<Statement> Block()
        {
            List<Statement> statements = new List<Statement>();

            while (!Check(TokenType.RIGHT_BRACE) && !AtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Block not closed with }");
            return statements;
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
