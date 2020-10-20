﻿using CppCompiler.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CppCompiler.Analysers
{
    internal class SyntacticAnalyser
    {
        private List<Token> _tokens;

        private Token _lookAhead;

        private List<SyntaticAnalyserResult> _syntaticAnalyserResults;

        public SyntacticAnalyser(List<Token> tokens)
        {
            _tokens = tokens;
            _lookAhead = tokens.FirstOrDefault();
            _syntaticAnalyserResults = new List<SyntaticAnalyserResult>();
        }

        internal void Execute()
        {
            Main();

            D();
        }

        private void Main()
        {
            if (_lookAhead.TokenType.IsType())
                NextToken();
            else
                throw new Exception();

            if (_lookAhead.TokenValue.Equals("MAIN", StringComparison.OrdinalIgnoreCase))
                NextToken();
            else
                throw new Exception("Program does not contain a Main entrance method.");

            if (_lookAhead.TokenType == TokenType.LeftParenthesis)
                NextToken();
            else
                throw new Exception();

            if (_lookAhead.TokenType == TokenType.RightParenthesis)
                NextToken();
            else
                throw new Exception();

            if (_lookAhead.TokenType == TokenType.LeftChaves)
                NextToken();
            else
                throw new Exception();
        }

        private void D()
        {
            if (_lookAhead.TokenType == TokenType.RightChaves)
                return;

            S();
            D();
        }

        private void S()
        {
            if (_lookAhead.TokenType.IsType())
            {
                var tVal = T();
                var idVal = ID();

                _syntaticAnalyserResults.Add(new SyntaticAnalyserResult
                {
                    TokenType = tVal,
                    LeftValue = idVal
                });

                A(tVal);
            }
            else if (_lookAhead.TokenType == TokenType.Identifier ||
                _lookAhead.TokenType == TokenType.LeftParenthesis)
            {
                O();
            }

            if (_lookAhead.TokenType == TokenType.Semicolon)
                MatchSemicolon();
        }

        private string ID()
        {
            if (_lookAhead.TokenType == TokenType.Identifier)
            {
                var token = _lookAhead;
                NextToken();
                return token.TokenValue;
            }

            return null;
        }

        private void A(TokenType tokenType)
        {
            if(_lookAhead.TokenType == TokenType.Comma)
            {
                NextToken();
                var idVal = ID();

                _syntaticAnalyserResults.Add(new SyntaticAnalyserResult
                {
                    TokenType = tokenType,
                    LeftValue = idVal
                });

                A(tokenType);
            }
        }

        private void O()
        {
            if (_lookAhead.TokenType == TokenType.LeftParenthesis)
            {
                MatchLeftParenthesis();
                var leftValue = ID();
                var opVal = OP();

                if (_lookAhead.TokenType == TokenType.LeftParenthesis)
                    P(opVal, leftValue);
                else
                {
                    var rightValue = ID();
                    G(opVal, leftValue, rightValue);
                }

                if (_lookAhead.TokenType == TokenType.RightParenthesis)
                    MatchRightParenthesis();

                if (_lookAhead.TokenType.IsOperator())
                {
                    var op = OP();
                    P(op, "ans");
                }
            }
            else if (_lookAhead.TokenType == TokenType.Identifier)
            {
                var leftValue = ID();

                if (_lookAhead.TokenType.IsOperator())
                {
                    var opVal = OP();

                    if (_lookAhead.TokenType == TokenType.Identifier)
                    {
                        // if()
                    }

                    P(opVal, leftValue);
                }
            }
        }

        private void P(string opVal, string leftValue)
        {
            O();

            _syntaticAnalyserResults.Add(new SyntaticAnalyserResult
            {
                Operator = opVal,
                LeftValue = leftValue,
                RightValue = "ans"
            });
        }

        private void G(string opVal, string leftValue, string rightValue)
        {
            _syntaticAnalyserResults.Add(new SyntaticAnalyserResult
            {
                Operator = opVal,
                LeftValue = leftValue,
                RightValue = rightValue
            });
        }

        private void NextToken()
        {
            _lookAhead = _lookAhead.Next(_tokens);
        }

        private string OP()
        {
            if (_lookAhead.TokenType.IsOperator())
            {
                var token = _lookAhead;
                NextToken();
                return token.TokenValue;
            }

            return null;
        }

        private TokenType T()
        {
            if (_lookAhead.TokenType.IsType())
            {
                var token = _lookAhead;
                NextToken();
                return token.TokenType;
            }

            return TokenType.Undefined ;
        }

        private void MatchLeftParenthesis()
        {
            if (_lookAhead.TokenType == TokenType.LeftParenthesis)
            {
                NextToken();
            }
            else { }
        }

        private void MatchRightParenthesis()
        {
            if (_lookAhead.TokenType == TokenType.RightParenthesis)
            {
                NextToken();
            }
            else { }
        }

        private void MatchSemicolon()
        {
            if (_lookAhead.TokenType == TokenType.Semicolon)
            {
                NextToken();
            }
            else { }
        }
    }
}
