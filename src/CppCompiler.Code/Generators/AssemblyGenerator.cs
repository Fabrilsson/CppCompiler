using CppCompiler.Enums;
using CppCompiler.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CppCompiler.Generators
{
    public class AssemblyGenerator
    {
        private SyntaticAnalyserResult _syntaticAnalyserResult;

        private List<string> _stringList;

        public AssemblyGenerator(SyntaticAnalyserResult syntaticAnalyserResult)
        {
            _syntaticAnalyserResult = syntaticAnalyserResult ?? throw new ArgumentNullException(nameof(syntaticAnalyserResult));

            _stringList = new List<string>() { "global _main\n\n", "extern  _GetStdHandle@4", "extern  _WriteFile@20",
                "extern  _ExitProcess@4\n\n", "section .text\n", "_main:\n\n" };
        }

        public Task GenerateAsync()
        {
            return Task.Run(() =>
            {
                CreateFile();

                AddAsmFileBegining();

                var localC3EList = new List<C3EAnalyserResult>(_syntaticAnalyserResult.C3EList);

                for (int i = 0; i < _syntaticAnalyserResult.C3EList.Count; i++)
                {
                    var item = _syntaticAnalyserResult.C3EList[i];

                    if (item.Operator?.TokenType == TokenType.AssignmentOperator)
                    {
                        _stringList.Add($"mov DWORD [{item.LeftValue?.TokenValue}], {item.RightValue?.TokenValue}");
                    }

                    if (item.LeftMostOperator?.TokenType == TokenType.GotoCommand)
                    {
                        var previous = _syntaticAnalyserResult.C3EList.ElementAt(_syntaticAnalyserResult.C3EList.IndexOf(item) - 1);

                        if (previous.LeftMostOperator?.TokenType == TokenType.IfCommand)
                        {

                            _stringList.Add($"cmp DWORD [{previous.LeftValue?.TokenValue}], {previous.RightValue?.TokenValue}");

                            if (previous.Operator?.TokenType == TokenType.EqualToOperator)
                            {
                                _stringList.Add($"je {item.LeftMostValue?.TokenValue.Replace(':', ' ')}");
                            }

                            if (previous.Operator?.TokenType == TokenType.GreaterThanOrEqualToOperator)
                            {
                                _stringList.Add($"jge {item.LeftMostValue?.TokenValue.Replace(':', ' ')}");
                            }
                        }
                        else
                        {
                            _stringList.Add($"je {item.LeftMostValue?.TokenValue.Replace(':', ' ')}");
                        }
                    }

                    if (item.LeftMostValue?.TokenType == TokenType.TempVariable)
                    {
                        if (item.LeftMostOperator != null && item.LeftMostOperator.TokenType.IsOperator())
                        {
                            if (item.LeftValue?.TokenType == TokenType.FloatingPointConstant || item.RightValue?.TokenType == TokenType.FloatingPointConstant)
                            {

                            }

                            if (item.Operator != null && item.Operator.TokenType == TokenType.PowOperator)
                            {
                                _stringList.Add($"mov eax, [{item.LeftValue?.TokenValue}]");
                                _stringList.Add($"mov ecx, [{item.RightValue?.TokenValue}]");
                                _stringList.Add($"mov [{item.LeftMostValue?.TokenValue}], eax");
                                _stringList.Add($"loopPow:");
                                _stringList.Add($"mul DWORD [{item.LeftMostValue?.TokenValue}]");
                                _stringList.Add($"mov [{item.LeftMostValue?.TokenValue}], eax");
                                _stringList.Add($"mov eax, [{item.LeftValue?.TokenValue}]");
                                _stringList.Add($"dec ecx");
                                _stringList.Add($"cmp ecx, 1");
                                _stringList.Add($"je exitLoopPow");
                                _stringList.Add($"jmp loopPow");
                                _stringList.Add($"exitLoopPow:");
                            }

                            if (item.Operator != null && item.Operator.TokenType == TokenType.MultiplicationOperator)
                            {
                                _stringList.Add($"mov eax, [{item.LeftValue?.TokenValue}]");
                                _stringList.Add($"mul DWORD [{item.RightValue?.TokenValue}]");
                                _stringList.Add($"mov [{item.LeftMostValue?.TokenValue}], eax");
                            }
                        }
                    }

                    if (item.LeftMostOperator?.TokenType == TokenType.Label)
                        _stringList.Add($"{item.LeftMostOperator.TokenValue}");
                }

                AddAsmFileFinish();

                AddAsmFileDataSection();

                WriteToFile(_stringList);

                Compile();
            });
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
                if (item.TokenType == TokenType.IntType || item.TokenType == TokenType.FloatType || item.TokenType == TokenType.TempVariable)
                    _stringList.Add($"{item.TokenValue} DQ 0");
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
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.FileName = Environment.GetEnvironmentVariable("comspec");
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();


            using (StreamWriter sw = proc.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine($@"nasm -f win32 {Directory.GetCurrentDirectory()}\programaSimples.asm");
                    sw.WriteLine($@"gcc programaSimples.obj -o programaSimples.exe");
                }
            }

            string stdout = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
        }
    }
}
