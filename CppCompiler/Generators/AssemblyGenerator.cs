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

            stringList.Add("mov ebp, esp");
            stringList.Add("sub esp, 4\n\n");

            stringList.Add("push -11");
            stringList.Add("call _GetStdHandle@4");
            stringList.Add("mov ebx, eax\n\n");



            foreach (var item in _syntaticAnalyserResult.C3EList)
            {
                if(item.Operator?.TokenType == TokenType.AssignmentOperator)
                {
                    if (item.LeftValue?.TokenType != TokenType.TempVariable && item.RightValue?.TokenType != TokenType.TempVariable)
                    {
                        stringList.Add($"mov edx, {item.RightValue.TokenValue}h");
                        stringList.Add($"add edx, 30h");
                        stringList.Add($"mov [{item.LeftValue.TokenValue}], edx");
                    }
                    else if(item.LeftValue?.TokenType == TokenType.TempVariable)
                    {
                        stringList.Add($"mov edx, {item.RightValue.TokenValue}h");
                        stringList.Add($"add edx, 30h");
                        stringList.Add($"mov ebx, edx");
                    }
                    else if (item.RightValue?.TokenType == TokenType.TempVariable)
                    {
                        stringList.Add($"mov edx, ebx");
                        stringList.Add($"add edx, 30h");
                        stringList.Add($"mov [{item.LeftValue.TokenValue}], edx");
                    }
                }

                if(item.Operator?.TokenType == TokenType.AdditionOperator)
                {
                    stringList.Add($"mov edx, {item.LeftValue.TokenValue}");
                    stringList.Add($"add edx, {item.RightValue.TokenValue}");
                    stringList.Add($"mov [{item.LeftMostValue.TokenValue}], edx");
                }

                if (item.LeftMostOperator?.TokenType == TokenType.Label)
                {
                    stringList.Add($"{item.LeftMostOperator.TokenValue}");
                }

                if(item.LeftMostOperator?.TokenType == TokenType.GotoCommand)
                {
                    var elementIndex = _syntaticAnalyserResult.C3EList.IndexOf(item);

                    var element = _syntaticAnalyserResult.C3EList[elementIndex - 1];

                    if (element.LeftMostOperator?.TokenType == TokenType.IfCommand && element.LeftValue.TokenType != TokenType.TempVariable)
                    {
                        stringList.Add($"mov edx, {element.LeftValue.TokenValue}");
                        stringList.Add($"mov eax, {element.RightValue.TokenValue}");
                        stringList.Add($"cmp edx, eax");

                        if (element.Operator?.TokenType == TokenType.LessThanOperator)
                        {

                            stringList.Add($"jae END_WHILE");
                        }
                    }
                }
            }

            
            stringList.Add("push 0");
            stringList.Add("lea eax, [ebp-4]");
            stringList.Add("push eax");
            stringList.Add("push 1");
            stringList.Add("push num");
            stringList.Add("push ebx");
            stringList.Add("call _WriteFile@20");


            stringList.Add("\n\npush 0");
            stringList.Add("call _ExitProcess@4\n\n");

            stringList.Add($"section .data\n");

            foreach (var item in _syntaticAnalyserResult.VarStack)
            {
                if (item.TokenType == TokenType.IntType || item.TokenType == TokenType.FloatType)
                    stringList.Add($"{item.TokenValue} DB 0");
            }

            File.WriteAllLines($@"{Directory.GetCurrentDirectory()}\programaSimples.asm", stringList);

            var proc = new Process();

            proc.StartInfo = new ProcessStartInfo();
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            proc.StartInfo.FileName = Environment.GetEnvironmentVariable("comspec");
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.UseShellExecute = false;
            //proc.StartInfo.Arguments = $@"/c nasm -f win32 {Directory.GetCurrentDirectory()}\programaSimples.asm";

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
