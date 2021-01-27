using CppCompiler.Entities;
using CppCompiler.Enums;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CppCompiler.Analysers
{
    class LexicalAnalyser
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

        public IEnumerable<Token> Execute(string texto)
        {
            Dictionary<TokenType, string> patterns = new Dictionary<TokenType, string>();

            patterns.Add(TokenType.LeftBracers, @"([{])");
            patterns.Add(TokenType.RightBracers, @"([}])");
            patterns.Add(TokenType.LongConstant, @"(\d+[l])");
            patterns.Add(TokenType.FloatingPointConstant, @"([-+]?[0-9]*\.?[0-9]*([.]|[E])[-+]?[0-9]*)");
            patterns.Add(TokenType.CharConstant, "('(.)')");
            patterns.Add(TokenType.StringConstant, "(\"([^\\\"]|\\.)*\")");
            patterns.Add(TokenType.Identifier, @"([_]*[a-zA-Z][0-9]*)+");
            patterns.Add(TokenType.IntegerConstant, @"(\d+)");
            patterns.Add(TokenType.LeftParenthesis, @"([(])");
            patterns.Add(TokenType.RightParenthesis, @"([)])");
            patterns.Add(TokenType.GreaterThanOrEqualToOperator, @"([>][=])");
            patterns.Add(TokenType.LessThanOrEqualToOperator, @"([<][=])");
            patterns.Add(TokenType.LessThanOperator, "([<])");
            patterns.Add(TokenType.GreaterThanOperator, "([>])");
            patterns.Add(TokenType.PowOperator, @"([\^])");
            patterns.Add(TokenType.EqualToOperator, @"([=][=])");
            patterns.Add(TokenType.OrOperator, @"([|][|])");
            patterns.Add(TokenType.AssignmentOperator, "([=])");
            patterns.Add(TokenType.AdditionOperator, "([+])");
            patterns.Add(TokenType.SubtractionOperator, "([-])");
            patterns.Add(TokenType.MultiplicationOperator, "([*])");
            patterns.Add(TokenType.AndOperator, "([&][&])");
            patterns.Add(TokenType.NotEqualToOperator, "([!][=])");
            patterns.Add(TokenType.ModuleOperator, "([%])");
            patterns.Add(TokenType.DivisionOperator, @"([/])");
            patterns.Add(TokenType.Semicolon, @"([;])");
            patterns.Add(TokenType.LeftBrackets, @"([[])");
            patterns.Add(TokenType.RightBrackets, @"([]])");
            patterns.Add(TokenType.Comma, @"([,])");

            List<Token> result = new List<Token>();

            MatchCollection myMatches = Regex.Matches(texto, string.Join("|", patterns.Values));
            foreach (Match currentMatch in myMatches)
            {
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
        }
    }
}
