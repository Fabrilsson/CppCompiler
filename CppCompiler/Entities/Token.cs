using CppCompiler.Enums;

namespace CppCompiler.Entities
{
    internal class Token
    {
        internal TokenType TokenType { get; set; }

        internal string TokenValue { get; set; }

        internal Token()
        {

        }

        internal Token(TokenType tokenType, string tokenValue)
        {
            TokenType = tokenType;
            TokenValue = tokenValue;
        }
    }
}
