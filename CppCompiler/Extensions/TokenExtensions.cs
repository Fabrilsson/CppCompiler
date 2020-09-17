using System.Collections.Generic;

namespace CppCompiler.Extensions
{
    internal static class TokenExtensions
    {
        internal static Token Next(this Token token, List<Token> tokens)
        {
            return tokens[tokens.IndexOf(token) + 1];
        }
    }
}
