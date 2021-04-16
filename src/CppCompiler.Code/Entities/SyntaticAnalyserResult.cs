using CppCompiler.Entities;
using System.Collections.Generic;

namespace CppCompiler
{
    public class C3EAnalyserResult
    {
        internal Token LeftMostValue { get; set; }
        internal Token LeftMostOperator { get; set; }
        internal Token LeftValue { get; set; }
        internal Token Operator { get; set; }
        internal Token RightValue { get; set; }
    }

    public class SyntaticAnalyserResult
    {
        internal List<C3EAnalyserResult> C3EList { get; set; }
        internal List<Token> VarStack { get; set; }
    }
}
