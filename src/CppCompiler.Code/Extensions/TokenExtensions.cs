using CppCompiler.Entities;
using CppCompiler.Enums;
using System.Collections.Generic;

namespace CppCompiler.Extensions
{
    internal static class TokenExtensions
    {
        internal static Token Next(this Token token, List<Token> tokens)
        {
            return tokens[tokens.IndexOf(token) + 1];
        }

        internal static Token Invert(this Token token)
        {
            if (token.TokenValue == ">")
                return new Token(TokenType.LessThanOrEqualToOperator, "<=");
            else if (token.TokenValue == "<")
                return new Token(TokenType.GreaterThanOrEqualToOperator, ">=");
            else if (token.TokenValue == "<=")
                return new Token(TokenType.GreaterThanOperator, ">");
            else if (token.TokenValue == ">=")
                return new Token(TokenType.LessThanOperator, "<");
            else if (token.TokenValue == "!=")
                return new Token(TokenType.EqualToOperator, "==");
            else if (token.TokenValue == "==")
                return new Token(TokenType.NotEqualToOperator, "!=");
            else
                return null;
        }
    }
}
