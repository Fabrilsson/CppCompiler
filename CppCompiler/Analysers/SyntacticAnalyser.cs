using CppCompiler.Extensions;
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

        private Stack<Token> _temporaryVarStack;

        private Stack<string> _c3eStack;

        private int _temporaryVarCounter;

        private int _c3eLineCounter;

        public SyntacticAnalyser(List<Token> tokens)
        {
            _tokens = tokens;
            _lookAhead = tokens.FirstOrDefault();
            _syntaticAnalyserResults = new List<SyntaticAnalyserResult>();
            _temporaryVarStack = new Stack<Token>();
            _c3eStack = new Stack<string>();
            _temporaryVarCounter = 0;
            _c3eLineCounter = 0;
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

            E();
            D();
        }

        private void DoThingy(string leftValue, Func<string> func)
        {
            var opVal = MatchToken();
            var rightValue = func();

            _syntaticAnalyserResults.Add(new SyntaticAnalyserResult
            {
                Operator = opVal.TokenValue,
                LeftValue = leftValue,
                RightValue = rightValue
            });

            GenerateC3E(leftValue, opVal, rightValue);
        }

        private void E()
        {
            var leftValue = TT();
            R(leftValue);
            MatchToken();
        }

        private string R(string leftValue)
        {
            Token newLeftValue = new Token();

            if (_temporaryVarStack.Any())
                newLeftValue = _temporaryVarStack.Pop();

            if (_lookAhead.TokenType == TokenType.AdditionOperator || _lookAhead.TokenType == TokenType.SubtractionOperator)
            {
                DoThingy(leftValue is null ? newLeftValue.TokenValue : leftValue, () => TT());

                return R(null);
            }

            return newLeftValue.TokenValue is null ? leftValue : newLeftValue.TokenValue;
        }

        private string TT()
        {
            var leftValue = F();
            leftValue = SS(leftValue);

            return leftValue;
        }

        private string SS(string leftValue)
        {
            Token newLeftValue = new Token();

            if (_temporaryVarStack.Any())
                newLeftValue = _temporaryVarStack.Pop();

            if (_lookAhead.TokenType == TokenType.MultiplicationOperator || 
                _lookAhead.TokenType == TokenType.DivisionOperator ||
                _lookAhead.TokenType == TokenType.OrOperator)
            {
                DoThingy(leftValue is null ? newLeftValue.TokenValue : leftValue, () => F());

                return SS(null);
            }

            return newLeftValue.TokenValue is null ? leftValue : newLeftValue.TokenValue;
        }

        private string F()
        {
            var leftValue = G();
            leftValue = H(leftValue);

            return leftValue;
        }

        private string H(string leftValue)
        {
            Token newLeftValue = new Token();

            if (_temporaryVarStack.Any())
                newLeftValue = _temporaryVarStack.Pop();

            if (_lookAhead.TokenType == TokenType.PowOperator)
            {
                DoThingy(leftValue is null ? newLeftValue.TokenValue : leftValue, () => G());

                return H(null);
            }
            else if (_lookAhead.TokenType == TokenType.AndOperator)
            {
                DoThingy(leftValue is null ? newLeftValue.TokenValue : leftValue, () => G());

                return H(null);
            }

            return newLeftValue.TokenValue is null ? leftValue : newLeftValue.TokenValue;
        }

        private string G()
        {
            if(_lookAhead.TokenType == TokenType.LeftParenthesis)
            {
                MatchToken();
                var leftValue = TT();
                leftValue = R(leftValue);
                MatchToken();

                return leftValue;
            }
            else if (_lookAhead.TokenType.IsNumber())
            {
                return MatchToken().TokenValue;
            }
            else if (_lookAhead.TokenType == TokenType.Identifier)
            {
                return MatchToken().TokenValue;
            }

            return null;
        }

        private Token MatchToken()
        {
            var token = _lookAhead;
            NextToken();
            return token;
        }

        private void NextToken()
        {
            _lookAhead = _lookAhead.Next(_tokens);
        }

        private void GenerateC3E(string leftValue, Token opVal, string rightValue)
        {
            if (opVal.TokenType.IsComparisonOperator())
            {
                _c3eStack.Push($"{_c3eLineCounter}. if {leftValue} {opVal.TokenValue.Invert()} {rightValue}");
                _c3eLineCounter++;
                _c3eStack.Push($"{_c3eLineCounter}. goto {_c3eLineCounter + 2}");
                _c3eLineCounter++;
                _c3eStack.Push($"{_c3eLineCounter}. T{_temporaryVarCounter} = 0");
                _c3eLineCounter++;
                _c3eStack.Push($"{_c3eLineCounter}. T{_temporaryVarCounter} = 1");
                _temporaryVarStack.Push(new Token(TokenType.BooleanConstant, $"T{_temporaryVarCounter}"));
                _temporaryVarCounter++;
            }
            else if (opVal.TokenType.IsLogicOperator())
            {
                _c3eStack.Push($"{_c3eLineCounter}. if {leftValue} {opVal.TokenValue} {rightValue}");
                _c3eLineCounter++;
                _c3eStack.Push($"{_c3eLineCounter}. goto {_c3eLineCounter + 2}");
                _c3eLineCounter++;
                _c3eStack.Push($"{_c3eLineCounter}. T{_temporaryVarCounter} = 0");
                _c3eLineCounter++;
                _c3eStack.Push($"{_c3eLineCounter}. T{_temporaryVarCounter} = 1");
                _temporaryVarStack.Push(new Token(TokenType.BooleanConstant, $"T{_temporaryVarCounter}"));
                _temporaryVarCounter++;
            }
            else if (opVal.TokenType != TokenType.AssignmentOperator)
            {
                _c3eStack.Push($"{_c3eLineCounter}. T{_temporaryVarCounter} = {leftValue} {opVal.TokenValue} {rightValue}");
                _temporaryVarStack.Push(new Token(TokenType.Undefined, $"T{_temporaryVarCounter}"));
                _temporaryVarCounter++;
            }
            else
            {
                _c3eStack.Push($"{leftValue} {opVal} {rightValue}");
            }
        }
    }
}
