using System;

namespace CppCompiler.Generators
{
    internal class AssemblyGenerator
    {
        private SyntaticAnalyserResult _syntaticAnalyserResult;

        internal AssemblyGenerator(SyntaticAnalyserResult syntaticAnalyserResult)
        {
            _syntaticAnalyserResult = syntaticAnalyserResult ?? throw new ArgumentNullException(nameof(syntaticAnalyserResult));
        }

        internal void Generate()
        {

        }
    }
}
