namespace CppCompiler.Extensions
{
    internal static class TokenValueExtensions
    {
        internal static string Invert(this string tokenValue)
        {
            if (tokenValue == ">")
                return "<=";
            else if (tokenValue == "<")
                return ">=";
            else if (tokenValue == "<=")
                return ">";
            else if (tokenValue == ">=")
                return "<";
            else if (tokenValue == "!=")
                return "==";
            else if (tokenValue == "==")
                return "!=";
            else 
                return null;
        }
    }
}
