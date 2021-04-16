using CppCompiler.Analysers;
using CppCompiler.Generators;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace CppCompiler.Presentation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string text = File.ReadAllText($@"{Directory.GetCurrentDirectory()}..\..\..\programaSimples.txt");

            Code.Text = text;
        }

        private async void Compile_Click(object sender, RoutedEventArgs e)
        {
            string code = Code.Text;

            var lexicalAnalyser = new LexicalAnalyser();

            var tokens = await lexicalAnalyser.ExecuteAsync(code);

            var syntaticAnalyser = new SyntacticAnalyser(tokens.ToList());

            var result = await syntaticAnalyser.ExecuteAsync();

            foreach (var token in tokens)
            {
                var tokenName = $"TokenType: {token.TokenType}";

                var tokenValue = $"Lexema: {token.TokenValue}";

                Console.WriteLine(string.Format("{0,-40} {1,5}\n", tokenName, tokenValue));
            }

            var assemblyGenerator = new AssemblyGenerator(result);

            await assemblyGenerator.GenerateAsync();

            Console.ReadLine();
        }
    }
}
