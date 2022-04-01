using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trmpLox
{
    // Parser class
    internal class Parser
    {
        private class ParseError : Exception { }
        readonly List<Token> tokens;
        int cur = 0;

        // Parser: constructor with list of tokens as parameter
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        // Parse: add parsed statements to statements list
        public List<Statement> Parse()
        {
            List<Statement> statements = new();
            while (!AtEnd())
            {
                statements.Add(Declaration());
            }
            return statements;
        }

        // Expr: call Assignment
        Expression Expr()
        {
            return Assignment();
        }

        // Declaration: try to match function or variable declarations
        Statement Declaration()
        {
            try
            {
                if (Match(TokenType.FUN)) return Function("function");
                if (Match(TokenType.VAR)) return VarDeclaration();
                return Stmt();
            }
            catch(ParseError error)
            {
                Synchronize(); // skip to end of function or variable declaration
                return null;
            }
        }

        // VarDeclaration: parse variable declaration
        Statement VarDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name."); // consume identifier

            Expression? initializer = null;
            if (Match(TokenType.EQUAL)) // if match equal sign, the variable is being initialized
            {
                initializer = Expr(); // parse expression for variable initialized
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration."); // consume semicolon
            return new VarStmt(name, initializer);
        }

        // Stmt: parse statement
        Statement Stmt()
        {
            // match for different type of statements, match and call correct function
            if (Match(TokenType.FOR)) return forStmt();
            if (Match(TokenType.IF)) return ifStmt();
            if (Match(TokenType.PRINT)) return PrintStmt();
            if (Match(TokenType.RETURN)) return ReturnStmt();
            if (Match(TokenType.WHILE)) return WhileStmt();
            if (Match(TokenType.LEFT_BRACE)) return new BlockStmt(Block());

            // else it is an expression statement
            return ExprStmt();
        }

        // forStmt: parse for loop
        Statement forStmt()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'."); // consume left paren

            // Initializer
            Statement? initializer;
            if (Match(TokenType.SEMICOLON)) // if semicolon, no initialization statement
            {
                initializer = null;
            }
            else if (Match(TokenType.VAR)) // if variable declaration, parse variable declaration
            {
                initializer = VarDeclaration();
            }
            else // else parse expression statement
            {
                initializer = ExprStmt();
            }

            // Condition
            Expression? condition = null;
            if (!Check(TokenType.SEMICOLON)) // if not semicolon, parse condition expression
            {
                condition = Expr();
            }
            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition."); // consume semicolon

            // Increment
            Expression? increment = null;
            if (!Check(TokenType.SEMICOLON)) // if not semicolon, parse increment exrpession
            {
                increment = Expr();
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses."); // consume semicolon

            Statement body = Stmt();

            // if increment is not null, create a new block statement with the body and then the increment expression statement (turn it into a while loop)
            if (increment != null)
            {
                body = new BlockStmt(new List<Statement>{ body, new ExpressionStmt(increment) });
            }

            // if condition not defined, then it is true
            if (condition == null)
            {
                condition = new Literal(true);
            }

            // convert to while loop
            body = new WhileStmt(condition, body);

            // if initializer is defined, create while block
            if (initializer != null)
            {
                body = new BlockStmt(new List<Statement> { initializer, body });
            }

            return body;
        }

        // WhileStmt: parse while loop
        Statement WhileStmt()
        {
            Consume(TokenType.LEFT_PAREN, "Expect  '(' after 'while'."); // consume left paren
            Expression condition = Expr(); // parse condition expression
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition."); // consume right paren
            Statement body = Stmt(); // parse body as statement

            return new WhileStmt(condition, body);
        }

        // IfStmt: parse if statement
        Statement ifStmt()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'."); // consume left paren
            Expression condition = Expr(); // parse condition expression
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition."); // consume right paren

            Statement thenBranch = Stmt(); // parse true branch as statement
            Statement? elseBranch = null;
            if (Match(TokenType.ELSE)) // if match an else token, parse else branch as statement
            {
                elseBranch = Stmt();
            }

            return new IfStmt(condition, thenBranch, elseBranch);
        }

        // PrintStmt: parse print statement
        Statement PrintStmt()
        {
            Expression expr = Expr(); // parse expression after print token
            Consume(TokenType.SEMICOLON, "Expect ; after value."); // consume semicolon
            return new PrintStmt(expr);
        }

        // ReturnStmt: parse return statement
        Statement ReturnStmt()
        {
            Token keyword = Previous(); // get return token
            Expression? value = null;
            if (!Check(TokenType.SEMICOLON)) // if not semicolon, parse expression (otherwise it is just "return;")
            {
                value = Expr();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after return value."); // consume semicolon
            return new ReturnStmt(keyword, value);
        }

        // ExprStmt: parse expression statement
        Statement ExprStmt()
        {
            Expression expr = Expr(); // parse expression
            Consume(TokenType.SEMICOLON, "Expect ; after value."); // consume semicolon
            return new ExpressionStmt(expr); // return statement
        }

        // Function: parse function defintition
        FunctionStmt Function(string kind)
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect " + kind + " name."); // consume function identifier
            Consume(TokenType.LEFT_PAREN, "Expect '(' after " + kind + " name."); // consume left paren for params
            List<Token> parameters = new();
            if (!Check(TokenType.RIGHT_PAREN)) // if not right paren (meaning no params)
            {
                do
                {
                    if (parameters.Count >= 255) // limit number of params
                    {
                        Console.WriteLine("Can't have more than 255 parameters.");
                    }
                    parameters.Add(Consume(TokenType.IDENTIFIER, "Expect parameter name.")); // parse parameter by consuming identifier
                } while (Match(TokenType.COMMA)); // continue doing this while there are commas
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters."); // consume right paren

            Consume(TokenType.LEFT_BRACE, "Expect '{' before " + kind + " body."); // consume left brace starting body
            List<Statement> body = Block(); // parse body of function
            return new FunctionStmt(name, parameters, body);
        }

        // Equality: parse equality statements
        Expression Equality()
        {
            Expression expr = Comparison(); // check for comparisons first (this will start the calls to parse any numerical or logical expression chain

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) // while you match an equal or not equal (they can be chained)
            {
                Token op = Previous(); // get operator
                Expression right = Comparison(); // check for comparison on right
                expr = new Binary(expr, op, right); // create new binary expression
            }

            return expr;
        }

        // Comparison: parse comparisons
        Expression Comparison()
        {
            Expression expr = Term(); // check for term first

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL)) // while match any comparitor
            {
                Token op = Previous(); // get operator
                Expression right = Term(); // check for term on right
                expr = new Binary(expr, op, right); // create new binary expression
            }

            return expr;
        }

        // Term: parse terms
        Expression Term()
        {
            Expression expr = Factor(); // check for factor first

            while (Match(TokenType.PLUS, TokenType.MINUS)) // while match to + or -
            {
                Token op = Previous(); // get operator
                Expression right = Factor(); // check for factor
                expr = new Binary(expr, op, right); // create new binary expression
            }

            return expr;
        }

        // Factor: parse factors
        Expression Factor()
        {
            Expression expr = Unary(); // check for unary factor first

            while (Match(TokenType.STAR, TokenType.SLASH)) // while match to * or / (mult or div)
            {
                Token op = Previous(); // get operator
                Expression right = Unary(); // check for unary
                expr = new Binary(expr, op, right); // create new binary expression
            }

            return expr;
        }

        // Unary: parse unarys
        Expression Unary()
        {
            if(Match(TokenType.BANG, TokenType.MINUS)) // if match a ! (logical not) or - (negative)
            {
                Token op = Previous(); // get operator
                Expression right = Unary(); // check for unary on right
                return new Unary(op, right); // create new unary expression
            }

            return Call(); // parse arguments in function call
        }

        // Call: parse function call
        private Expression Call()
        {
            Expression expr = Primary(); // parse basic types

            while (true) // while there are left parens, parse function call
            {
                if (Match(TokenType.LEFT_PAREN)) // if match left paren, parse function call
                {
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        // FinishCall: parse the remainder of a function call
        private Expression FinishCall(Expression callee)
        {
            List<Expression> args = new List<Expression>();
            if (!Check(TokenType.RIGHT_PAREN)) // if not match right paren
            {
                if (args.Count >= 255) // check for param limit
                {
                    Console.WriteLine("Can't have more than 255 arguments.");
                    Peek();
                }
                args.Add(Expr()); // parse expression and add to args list
                while (Match(TokenType.COMMA)) // while match to comma, keep parsing args
                {
                    args.Add(Expr());
                }
            }

            Token paren = Consume(TokenType.RIGHT_PAREN, "Missing ')' after args."); // consume right paren
            return new Call(callee, paren, args);
        }

        // Primary: parse basic types
        Expression Primary()
        {
            // match literals
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.NIL)) return new Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Literal(Previous().GetLiteral());
            }

            // match identifier
            if (Match(TokenType.IDENTIFIER))
            {
                return new Variable(Previous());
            }

            // match grouping
            if (Match(TokenType.LEFT_PAREN))
            {
                Expression expr = Expr(); // parse expression in grouping
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression."); // consume right paren
                return new Grouping(expr);
            }
            Console.WriteLine("Found nothing in Primary()");
            return new Literal(null);
        }

        // Assignment: parse assignment
        private Expression Assignment()
        {
            Expression expression = Or(); // check for logical operators

            if (Match(TokenType.EQUAL))
            {
                Token equals = Previous();
                Expression value = Assignment();

                if (expression is Variable) { // check if it is a variable, and treat it as assignment
                    Token name = ((Variable)expression).name;
                    return new Assign(name, value);
                }
            }

            return expression;
        }

        // Or: parse or logical expression
        private Expression Or()
        {
            Expression expr = And(); // parse and expressoin

            while (Match(TokenType.OR)) // while match or logical operator
            {
                Token op = Previous();
                Expression right = And(); // parse and operator
                expr = new Logical(expr, op, right);
            }

            return expr;
        }

        // And: parse and logical expression
        private Expression And()
        {
            Expression expr = Equality(); // parse equality (this will cascade all the way down to literals)

            while (Match(TokenType.AND)) // while match and logical operator
            {
                Token op = Previous();
                Expression right = Equality(); // parse equality
                expr = new Logical(expr, op, right);
            }

            return expr;
        }

        // Block: parse code blocks
        private List<Statement> Block()
        {
            List<Statement> statements = new List<Statement>();

            while (!Check(TokenType.RIGHT_BRACE) && !AtEnd()) // while not at end of file or right brace
            {
                statements.Add(Declaration()); // add to statement list for code block
            }

            Consume(TokenType.RIGHT_BRACE, "Block not closed with }"); // consume right brace
            return statements;
        }

        // Match: match token to type
        bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types) // loop through token types sought to be matched
            {
                if (Check(type)) // check for match
                {
                    Advance(); // advance index
                    return true;
                }
            }
            return false; // return false if no match
        }

        // Consume: match token else throw error
        Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance(); // if match token, advance

            throw Error(Peek(), message); // else throw error
        }

        // Check: check eof, else return type of token
        bool Check(TokenType type)
        {
            if (AtEnd()) return false;
            return Peek().GetType() == type; // return if token matches type
        }

        // Advance: if not EOF, advance index and return previous token
        Token Advance()
        {
            if (!AtEnd()) cur++;
            return Previous();
        }

        // AtEnd: check for EOF token
        bool AtEnd()
        {
            return Peek().GetType() == TokenType.EOF;
        }

        // Peek: return token at current position
        Token Peek()
        {
            return tokens[cur];
        }

        // Previous: return token at previous position
        Token Previous()
        {
            return tokens[cur - 1];
        }

        // ParseError: call main error function and pass message
        private ParseError Error(Token token, string msg)
        {
            TrMpLox.Error(token, msg);
            return new ParseError();
        }

        // Synchronize: advance to get passed non fatal error
        void Synchronize()
        {
            Advance(); // advance

            while (!AtEnd()) // while not eof
            {
                if (Previous().type == TokenType.SEMICOLON) return; // if match ;, return

                switch (Peek().type) // if match to another statement, return
                {
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR: 
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN: return;

                }

                Advance(); // keep advancing
            }
        }
    }
}
