using CppCompiler.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        internal Task GenerateAsync()
        {
            return Task.Run(() =>
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
                            //_stringList.Add($"mov ah, {item.RightValue?.TokenValue}");
                            _stringList.Add($"mov DWORD [{item.LeftValue?.TokenValue}], {item.RightValue?.TokenValue}");
                        }

                        if (item.LeftValue?.TokenType == TokenType.Identifier)
                        {
                            if (item.RightValue?.TokenType == TokenType.Identifier || item.RightValue?.TokenType == TokenType.TempVariable)
                                _stringList.Add($"mov ah, [{item.RightValue?.TokenValue}]");
                            else
                                _stringList.Add($"mov ah, {item.RightValue?.TokenValue}");

                            _stringList.Add($"mov [{item.LeftValue?.TokenValue}], ah");
                        }
                    }

                    if (item.LeftMostOperator?.TokenType == TokenType.GotoCommand)
                    {
                        var previous = _syntaticAnalyserResult.C3EList.ElementAt(_syntaticAnalyserResult.C3EList.IndexOf(item) - 1);

                        if (previous.LeftMostOperator?.TokenType == TokenType.IfCommand)
                        {

                            if (previous.LeftValue?.TokenType == TokenType.TempVariable)
                            {
                                _stringList.Add($"cmp DWORD [{previous.LeftValue?.TokenValue}], {previous.RightValue?.TokenValue}");

                                if (previous.Operator?.TokenType == TokenType.EqualToOperator)
                                {
                                    _stringList.Add($"je {item.LeftMostValue?.TokenValue.Replace(':', ' ')}");
                                }
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
                    //sw.WriteLine($@"link /subsystem:console /nodefaultlib /entry:main programaSimples.obj kernel32.lib");
                    sw.WriteLine($@"gcc programaSimples.obj -o programaSimples.exe");
                }
            }
        }
    }
}
