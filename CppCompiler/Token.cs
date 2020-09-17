namespace CppCompiler
{
    class Token
    {
        public TokenType TokenType { get; set; }

        public string TokenValue { get; set; }

        public Token(TokenType tokenType, string tokenValue)
        {
            TokenType = tokenType;
            TokenValue = tokenValue;
        }
    }
}
