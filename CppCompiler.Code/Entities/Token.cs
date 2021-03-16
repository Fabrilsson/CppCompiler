using CppCompiler.Enums;

namespace CppCompiler.Entities
{
    public class Token
    {
        public TokenType TokenType { get; set; }

        public string TokenValue { get; set; }

        public Token()
        {

        }

        public Token(TokenType tokenType, string tokenValue)
        {
            TokenType = tokenType;
            TokenValue = tokenValue;
        }
    }
}
