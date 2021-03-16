using CppCompiler.Enums;

namespace CppCompiler.Extensions
{
    internal static class TokenTypeExtensions
    {
        internal static bool IsType(this TokenType tokenType)
        {
            return tokenType == TokenType.CharType ||
                tokenType == TokenType.FloatType ||
                tokenType == TokenType.IntType ||
                tokenType == TokenType.LongType ||
                tokenType == TokenType.ShortType ||
                tokenType == TokenType.StructType ||
                tokenType == TokenType.UnsignedType ||
                tokenType == TokenType.VoidType;
        }

        internal static bool IsOperator(this TokenType tokenType)
        {
            return tokenType == TokenType.AdditionOperator ||
                tokenType == TokenType.AndOperator ||
                tokenType == TokenType.AssignmentOperator ||
                tokenType == TokenType.CompoundAdditionOperator ||
                tokenType == TokenType.CompoundDivisionOperator ||
                tokenType == TokenType.CompoundModuleOperator ||
                tokenType == TokenType.CompoundMultiplicationOperator ||
                tokenType == TokenType.CompoundMultiplicationOperator ||
                tokenType == TokenType.CompoundSubtractionOperator ||
                tokenType == TokenType.DecrementOperator ||
                tokenType == TokenType.DivisionOperator ||
                tokenType == TokenType.EqualToOperator ||
                tokenType == TokenType.GreaterThanOperator ||
                tokenType == TokenType.GreaterThanOrEqualToOperator ||
                tokenType == TokenType.IncrementOperator ||
                tokenType == TokenType.LessThanOperator ||
                tokenType == TokenType.LessThanOrEqualToOperator ||
                tokenType == TokenType.ModuleOperator ||
                tokenType == TokenType.MultiplicationOperator ||
                tokenType == TokenType.NotEqualToOperator ||
                tokenType == TokenType.NotOperator ||
                tokenType == TokenType.OrOperator ||
                tokenType == TokenType.PowOperator ||
                tokenType == TokenType.SubtractionOperator;
        }

        internal static bool IsComparisonOperator(this TokenType tokenType)
        {
            return tokenType == TokenType.LessThanOperator ||
                tokenType == TokenType.LessThanOrEqualToOperator ||
                tokenType == TokenType.GreaterThanOperator ||
                tokenType == TokenType.GreaterThanOrEqualToOperator ||
                tokenType == TokenType.NotEqualToOperator ||
                tokenType == TokenType.EqualToOperator;
        }

        internal static bool IsLogicOperator(this TokenType tokenType)
        {
            return tokenType == TokenType.AndOperator ||
                tokenType == TokenType.OrOperator;
        }

        internal static bool IsNumber(this TokenType tokenType)
        {
            return tokenType == TokenType.IntegerConstant ||
                tokenType == TokenType.FloatingPointConstant ||
                tokenType == TokenType.LongConstant;
        }
    }
}
