using CppCompiler.Enums;

namespace CppCompiler
{
    internal class SyntaticAnalyserResult
    {
        internal TokenType TokenType { get; set; }

        internal string LeftValue { get; set; }

        internal string Operator { get; set; }

        internal string RightValue { get; set; }
    }
}
