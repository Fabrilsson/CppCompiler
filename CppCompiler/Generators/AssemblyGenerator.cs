using CppCompiler.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CppCompiler.Generators
{
    internal class AssemblyGenerator
    {
        private SyntaticAnalyserResult _syntaticAnalyserResult;

        private Dictionary<string, string> _tempVariables;

        private List<string> _stringList;

        internal AssemblyGenerator(SyntaticAnalyserResult syntaticAnalyserResult)
        {
            _syntaticAnalyserResult = syntaticAnalyserResult ?? throw new ArgumentNullException(nameof(syntaticAnalyserResult));

            _tempVariables = new Dictionary<string, string>();

            _stringList = new List<string>() { "global _main\n\n", "extern  _GetStdHandle@4", "extern  _WriteFile@20",
                "extern  _ExitProcess@4\n\n", "section .text\n", "_main:\n\n" };
        }

        internal void Generate()
        {
            CreateFile();

            AddAsmFileBegining();

            var localC3EList = new List<C3EAnalyserResult>(_syntaticAnalyserResult.C3EList);

            foreach (var item in _syntaticAnalyserResult.C3EList)
            {
                if (item.Operator?.TokenType == TokenType.AssignmentOperator)
                {
                    if (item.LeftValue?.TokenType == TokenType.TempVariable)
                    {
                        if (item.RightValue?.TokenType == TokenType.IntegerConstant)
                        {
                            if (!_tempVariables.ContainsKey($"{item.LeftValue?.TokenValue}"))
                            {
                                if (!_tempVariables.ContainsValue("ah"))
                                    _tempVariables.Add($"{item.LeftValue?.TokenValue}", "ah");
                                else
                                    _tempVariables.Add($"{item.LeftValue?.TokenValue}", "al");
                            }

                            _tempVariables.TryGetValue(item.LeftValue?.TokenValue, out string value);

                            _stringList.Add($"mov {value}, {item.RightValue?.TokenValue}");

                            localC3EList.Remove(item);

                            if (!localC3EList.Any(l => l.LeftValue?.TokenValue == item.LeftValue.TokenValue))
                                _tempVariables.Remove(item.LeftValue.TokenValue);
                        }
                    }

                    if(item.LeftValue?.TokenType == TokenType.Identifier)
                    {
                        _stringList.Add($"mov [{item.LeftValue?.TokenValue}], {item.RightValue?.TokenValue}");
                    }
                }

                if (item.LeftMostOperator?.TokenType == TokenType.IfCommand)
                {
                    if (item.LeftValue?.TokenType == TokenType.TempVariable)
                    {
                        if (item.Operator?.TokenType == TokenType.EqualToOperator)
                        {
                            if (_tempVariables.ContainsKey($"{item.LeftValue?.TokenValue}"))
                            {
                                var aaa = _tempVariables[$"{item.LeftValue?.TokenValue}"];
                                _stringList.Add($"cmp {aaa}, {item.RightValue?.TokenValue}");
                            }

                            localC3EList.Remove(item);

                            if (!localC3EList.Any(l => l.LeftValue?.TokenValue == item.LeftValue.TokenValue))
                                _tempVariables.Remove(item.LeftValue.TokenValue);
                        }
                    }
                }
            }

            AddAsmFileFinish();

            AddAsmFileDataSection();

            WriteToFile(_stringList);

            Compile();
        }

        private void CreateFile()
        {
            var file = File.Create($@"{Directory.GetCurrentDirectory()}\programaSimples.asm");

            file.Close();
        }

        private void AddAsmFileBegining()
        {
            _stringList.Add("mov ebp, esp");
            _stringList.Add("sub esp, 4\n\n");

            _stringList.Add("push -11");
            _stringList.Add("call _GetStdHandle@4");
            _stringList.Add("mov ebx, eax\n\n");
        }

        private void AddAsmFileDataSection()
        {
            _stringList.Add($"section .data\n");

            foreach (var item in _syntaticAnalyserResult.VarStack)
            {
                if (item.TokenType == TokenType.IntType || item.TokenType == TokenType.FloatType)
                    _stringList.Add($"{item.TokenValue} DB 0");
            }
        }

        private void AddAsmFileFinish()
        {
            _stringList.Add("\n\npush 0");
            _stringList.Add("call _ExitProcess@4\n\n");
        }

        private void WriteToFile(IEnumerable<string> stringList)
        {
            File.WriteAllLines($@"{Directory.GetCurrentDirectory()}\programaSimples.asm", stringList);
        }

        private void Compile()
        {
            var proc = new Process();

            proc.StartInfo = new ProcessStartInfo();
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            proc.StartInfo.FileName = Environment.GetEnvironmentVariable("comspec");
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();


            using (StreamWriter sw = proc.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine($@"nasm -f win32 {Directory.GetCurrentDirectory()}\programaSimples.asm");
                    sw.WriteLine($@"link /subsystem:console /nodefaultlib /entry:main programaSimples.obj kernel32.lib");
                    sw.WriteLine($@"gcc programaSimples.obj -o programaSimples.exe");
                }
            }
        }
    }
}
