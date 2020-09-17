using CppCompiler.Analysers;
using System;
using System.IO;
using System.Linq;

namespace CppCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText($@"{Directory.GetCurrentDirectory()}\programaSimples.txt");

            var lexicalAnalyser = new LexicalAnalyser();

            var tokens = lexicalAnalyser.Execute(text);

            var syntaticAnalyser = new SyntacticAnalyser(tokens.ToList());

            syntaticAnalyser.Execute();

            foreach (var token in tokens)
            {
                var tokenName = $"TokenType: {token.TokenType}";

                var tokenValue = $"Lexema: {token.TokenValue}";

                Console.WriteLine(string.Format("{0,-40} {1,5}\n", tokenName, tokenValue));
            }

            Console.ReadLine();
        }
    }
}
