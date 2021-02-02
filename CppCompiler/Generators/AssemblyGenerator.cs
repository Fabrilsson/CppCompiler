using CppCompiler.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
            var file = File.Create($@"{Directory.GetCurrentDirectory()}\programaSimples.asm");

            file.Close();

            var stringList = new List<string>() { "global _main\n\n", "extern  _GetStdHandle@4", "extern  _WriteFile@20", 
                "extern  _ExitProcess@4\n\n", "section .text\n", "_main:\n\n" };

            stringList.Add($"section .data\n");

            foreach (var item in _syntaticAnalyserResult.VarStack)
            {
                if (item.TokenType == TokenType.IntType || item.TokenType == TokenType.FloatType)
                    stringList.Add($"{item.TokenValue} DQ 0");
            }

            File.WriteAllLines($@"{Directory.GetCurrentDirectory()}\programaSimples.asm", stringList);

            var proc = new Process();

            proc.StartInfo = new ProcessStartInfo();
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.FileName = Environment.GetEnvironmentVariable("comspec");
            proc.StartInfo.Arguments = $@"/c nasm -f win32 {Directory.GetCurrentDirectory()}\programaSimples.asm";

            proc.Start();
        }
    }
}
