using CppCompiler.Entities;
using CppCompiler.Enums;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CppCompiler.Analysers
{
    public class LexicalAnalyser
    {
        private static readonly Dictionary<string, TokenType> ReservedWords = new Dictionary<string, TokenType>(StringComparer.Ordinal)
        {
            {"if", TokenType.IfCommand},
            {"else", TokenType.ElseCommand},
            {"for", TokenType.ForCommand},
            {"while", TokenType.WhileCommand},
            {"do", TokenType.DoCommand},
            {"switch", TokenType.SwitchCommand},
            {"case", TokenType.CaseCommand},
            {"default", TokenType.DefaultCommand},
            {"break", TokenType.BreakCommand},
            {"continue", TokenType.ContinueCommand},
            {"return", TokenType.ReturnCommand},

            {"true", TokenType.BooleanConstant },
            {"false", TokenType.BooleanConstant },

            {"int", TokenType.IntType},
            {"float", TokenType.FloatType},
            {"long", TokenType.LongType},
            {"short", TokenType.ShortType},
            {"char", TokenType.CharType},
            {"unsigned", TokenType.UnsignedType},
            {"void", TokenType.VoidType},
            {"struct", TokenType.StructType}
        };

        public async Task<IEnumerable<Token>> ExecuteAsync(string texto)
        {
            var patterns = GetPatterns();

            List<Token> result = new List<Token>();

            MatchCollection myMatches = Regex.Matches(texto, string.Join("|", patterns.Values));

            var task = Task.Run(() =>
            {
                for (int i = 0; i < myMatches.Count; i++)
                {
                    var currentMatch = myMatches[i];

                    if (ReservedWords.TryGetValue(currentMatch.Value, out TokenType tokenType))
                    {
                        var token = new Token(tokenType, currentMatch.Value);

                        result.Add(token);
                    }
                    else
                    {
                        foreach (var pattern in patterns)
                        {
                            var match = Regex.Match(currentMatch.Value, pattern.Value);

                            if (!string.IsNullOrEmpty(match.Value))
                            {
                                var token = new Token(pattern.Key, currentMatch.Value);

                                result.Add(token);

                                break;
                            }
                        }
                    }
                }

                return result;
            });

            return await task;
        }

        private Dictionary<TokenType, string> GetPatterns()
        {
            return new Dictionary<TokenType, string>
            {
                { TokenType.LeftBracers, @"([{])" },
                { TokenType.RightBracers, @"([}])" },
                { TokenType.LongConstant, @"(\d+[l])" },
                { TokenType.FloatingPointConstant, @"([-+]?[0-9]*\.?[0-9]*([.]|[E])[-+]?[0-9]*)" },
                { TokenType.CharConstant, "('(.)')" },
                { TokenType.StringConstant, "(\"([^\\\"]|\\.)*\")" },
                { TokenType.Identifier, @"([_]*[a-zA-Z][0-9]*)+" },
                { TokenType.IntegerConstant, @"(\d+)" },
                { TokenType.LeftParenthesis, @"([(])" },
                { TokenType.RightParenthesis, @"([)])" },
                { TokenType.GreaterThanOrEqualToOperator, @"([>][=])" },
                { TokenType.LessThanOrEqualToOperator, @"([<][=])" },
                { TokenType.LessThanOperator, "([<])" },
                { TokenType.GreaterThanOperator, "([>])" },
                { TokenType.PowOperator, @"([\^])" },
                { TokenType.EqualToOperator, @"([=][=])" },
                { TokenType.OrOperator, @"([|][|])" },
                { TokenType.AssignmentOperator, "([=])" },
                { TokenType.AdditionOperator, "([+])" },
                { TokenType.SubtractionOperator, "([-])" },
                { TokenType.MultiplicationOperator, "([*])" },
                { TokenType.AndOperator, "([&][&])" },
                { TokenType.NotEqualToOperator, "([!][=])" },
                { TokenType.ModuleOperator, "([%])" },
                { TokenType.DivisionOperator, @"([/])" },
                { TokenType.Semicolon, @"([;])" },
                { TokenType.LeftBrackets, @"([[])" },
                { TokenType.RightBrackets, @"([]])" },
                { TokenType.Comma, @"([,])" }
            };
        }
    }
}
