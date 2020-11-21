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

        private Stack<string> _temporaryVarStack;

        private Stack<string> _c3eStack;

        private int _temporaryVarCounter;

        public SyntacticAnalyser(List<Token> tokens)
        {
            _tokens = tokens;
            _lookAhead = tokens.FirstOrDefault();
            _syntaticAnalyserResults = new List<SyntaticAnalyserResult>();
            _temporaryVarStack = new Stack<string>();
            _c3eStack = new Stack<string>();
            _temporaryVarCounter = 0;
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

        private void E()
        {
            var leftValue = TT();
            R(leftValue);
            MatchToken();
        }

        private void R(string leftValue)
        {
            if (_lookAhead.TokenType == TokenType.AdditionOperator)
            {
                var opVal = MatchToken();
                var rightValue = TT();

                string newLeftValue = string.Empty, newRightValue = string.Empty;

                if (_temporaryVarStack.Any() && rightValue is null)
                    newRightValue = _temporaryVarStack.Pop();

                if (_temporaryVarStack.Any() && leftValue is null)
                    newLeftValue = _temporaryVarStack.Pop();

                _syntaticAnalyserResults.Add(new SyntaticAnalyserResult
                {
                    Operator = opVal,
                    LeftValue = leftValue is null ? newLeftValue : leftValue,
                    RightValue = rightValue is null ? newRightValue : rightValue
                });

                GenerateC3E(leftValue is null ? newLeftValue : leftValue, opVal, rightValue is null ? newRightValue : rightValue);

                if (_temporaryVarStack.Any() && leftValue is null)
                    newLeftValue = _temporaryVarStack.Pop();

                R(newLeftValue);
            }
            else if (_lookAhead.TokenType == TokenType.SubtractionOperator)
            {
                var opVal = MatchToken();
                var rightValue = TT();

                string newLeftValue = string.Empty, newRightValue = string.Empty;

                if (_temporaryVarStack.Any() && rightValue is null)
                    newRightValue = _temporaryVarStack.Pop();

                if (_temporaryVarStack.Any() && leftValue is null)
                    newLeftValue = _temporaryVarStack.Pop();

                _syntaticAnalyserResults.Add(new SyntaticAnalyserResult
                {
                    Operator = opVal,
                    LeftValue = leftValue is null ? newLeftValue : leftValue,
                    RightValue = rightValue is null ? newRightValue : rightValue
                });

                GenerateC3E(leftValue is null ? newLeftValue : leftValue, opVal, rightValue is null ? newRightValue : rightValue);

                if (_temporaryVarStack.Any() && leftValue is null)
                    newLeftValue = _temporaryVarStack.Pop();

                R(newLeftValue);
            }
        }

        private string TT()
        {
            var leftValue = F();
            SS(leftValue);

            return leftValue;
        }

        private void SS(string leftValue)
        {
            if (_lookAhead.TokenType == TokenType.MultiplicationOperator)
            {
                var opVal = MatchToken();
                var rightValue = F();

                string newLeftValue = string.Empty, newRightValue = string.Empty;

                if (_temporaryVarStack.Any() && rightValue is null)
                    newRightValue = _temporaryVarStack.Pop();

                if (_temporaryVarStack.Any() && leftValue is null)
                    newLeftValue = _temporaryVarStack.Pop();

                _syntaticAnalyserResults.Add(new SyntaticAnalyserResult
                {
                    Operator = opVal,
                    LeftValue = leftValue is null ? newLeftValue : leftValue,
                    RightValue = rightValue is null ? newRightValue : rightValue
                });

                GenerateC3E(leftValue is null ? newLeftValue : leftValue, opVal, rightValue is null ? newRightValue : rightValue);

                if (_temporaryVarStack.Any() && leftValue is null)
                    newLeftValue = _temporaryVarStack.Pop();

                SS(newLeftValue);
            }
            else if (_lookAhead.TokenType == TokenType.DivisionOperator)
            {
                var opVal = MatchToken();
                var rightValue = F();

                string newLeftValue = string.Empty, newRightValue = string.Empty;

                if (_temporaryVarStack.Any() && rightValue is null)
                    newRightValue = _temporaryVarStack.Pop();

                if (_temporaryVarStack.Any() && leftValue is null)
                    newLeftValue = _temporaryVarStack.Pop();

                _syntaticAnalyserResults.Add(new SyntaticAnalyserResult
                {
                    Operator = opVal,
                    LeftValue = leftValue is null ? newLeftValue : leftValue,
                    RightValue = rightValue is null ? newRightValue : rightValue
                });

                GenerateC3E(leftValue is null ? newLeftValue : leftValue, opVal, rightValue is null ? newRightValue : rightValue);

                if (_temporaryVarStack.Any() && leftValue is null)
                    newLeftValue = _temporaryVarStack.Pop();

                SS(newLeftValue);
            }
        }

        private string F()
        {
            var leftValue = G();
            H(leftValue);

            return leftValue;
        }

        private void H(string leftValue)
        {
            if (_lookAhead.TokenType == TokenType.PowOperator)
            {
                var opVal = MatchToken();
                var rightValue = G();

                string newLeftValue = string.Empty, newRightValue = string.Empty;

                if (_temporaryVarStack.Any() && rightValue is null)
                    newRightValue = _temporaryVarStack.Pop();

                if (_temporaryVarStack.Any() && leftValue is null)
                    newLeftValue = _temporaryVarStack.Pop();

                _syntaticAnalyserResults.Add(new SyntaticAnalyserResult
                {
                    Operator = opVal,
                    LeftValue = leftValue is null ? newLeftValue : leftValue,
                    RightValue = rightValue is null ? newRightValue : rightValue
                });

                GenerateC3E(leftValue is null ? newLeftValue : leftValue, opVal, rightValue is null ? newRightValue : rightValue);

                if (_temporaryVarStack.Any() && leftValue is null)
                    newLeftValue = _temporaryVarStack.Pop();

                H(newLeftValue);
            }
        }

        private string G()
        {
            if(_lookAhead.TokenType == TokenType.LeftParenthesis)
            {
                MatchToken();
                var leftValue = TT();
                R(leftValue);
                MatchToken();

                return null;
            }
            else if (_lookAhead.TokenType.IsNumber())
            {
                return MatchToken();
            }
            else if (_lookAhead.TokenType == TokenType.Identifier)
            {
                return MatchToken();
            }

            return null;
        }

        private string MatchToken()
        {
            var token = _lookAhead;
            NextToken();
            return token.TokenValue;
        }

        private void NextToken()
        {
            _lookAhead = _lookAhead.Next(_tokens);
        }

        private void GenerateC3E(string leftValue, string opVal, string rightValue)
        {
            if (opVal != "=")
            {
                _c3eStack.Push($"T{_temporaryVarCounter} = {leftValue} {opVal} {rightValue}");
                _temporaryVarStack.Push($"T{_temporaryVarCounter}");
                _temporaryVarCounter++;
            }
            else
            {
                _c3eStack.Push($"{leftValue} {opVal} {rightValue}");
            }
        }
    }
}
