using CppCompiler.Entities;
using CppCompiler.Enums;
using CppCompiler.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CppCompiler.Analysers
{
    public class SyntacticAnalyser
    {
        private List<Token> _tokens;

        private Token _lookAhead;

        private List<C3EAnalyserResult> _c3eAnalyserResults;

        private SyntaticAnalyserResult _syntaticAnalyserResult;

        private Stack<Token> _varStack;

        private Stack<Token> _temporaryVarStack;

        private Stack<Token> _temporaryVarStackAcumulator;

        private Stack<string> _c3eStack;

        private int _temporaryVarCounter;

        private int _c3eLineCounter;

        public SyntacticAnalyser(List<Token> tokens)
        {
            _tokens = tokens;
            _lookAhead = tokens.FirstOrDefault();
            _c3eAnalyserResults = new List<C3EAnalyserResult>();
            _syntaticAnalyserResult = new SyntaticAnalyserResult();
            _varStack = new Stack<Token>();
            _temporaryVarStack = new Stack<Token>();
            _temporaryVarStackAcumulator = new Stack<Token>();
            _c3eStack = new Stack<string>();
            _temporaryVarCounter = 0;
            _c3eLineCounter = 0;
        }

        public async Task<SyntaticAnalyserResult> ExecuteAsync()
        {
            Main();

            await D();

            var tempStack = new Stack<string>();

            while (_c3eStack.Any())
                tempStack.Push(_c3eStack.Pop());

            _c3eStack = tempStack;

            _syntaticAnalyserResult.C3EList = _c3eAnalyserResults;
            _syntaticAnalyserResult.VarStack = _varStack.ToList();
            _syntaticAnalyserResult.VarStack.AddRange(_temporaryVarStackAcumulator);

            var file = File.Create($@"{Directory.GetCurrentDirectory()}\programaSimplesC3E.txt");

            file.Close();

            var stringList = new List<string>();

            foreach (var item in _syntaticAnalyserResult.C3EList)
            {
                stringList.Add($"{item.LeftMostValue?.TokenValue} {item.LeftMostOperator?.TokenValue} " +
                    $"{item.LeftValue?.TokenValue} {item.Operator?.TokenValue} {item.RightValue?.TokenValue}");
            }

            File.WriteAllLines($@"{Directory.GetCurrentDirectory()}\programaSimplesC3E.txt", stringList);

            return _syntaticAnalyserResult;
        }

        private void Main()
        {
            if (_lookAhead.TokenType.IsType())
                MatchToken();
            else
                throw new Exception();

            if (_lookAhead.TokenValue.Equals("MAIN", StringComparison.OrdinalIgnoreCase))
                MatchToken();
            else
                throw new Exception("Program does not contain a Main entrance method.");

            if (_lookAhead.TokenType == TokenType.LeftParenthesis)
                MatchToken();
            else
                throw new Exception();

            if (_lookAhead.TokenType == TokenType.RightParenthesis)
                MatchToken();
            else
                throw new Exception();

            if (_lookAhead.TokenType == TokenType.LeftBracers)
                MatchToken();
            else
                throw new Exception();
        }

        private async Task D()
        {
            await While();

            await If();

            if (_lookAhead.TokenType.IsType())
            {
                V();//Entende os tipos
                await D();//Depois pode ter qualquer coisa
            }

            if (_lookAhead.TokenType != TokenType.RightBracers &&
                _lookAhead.TokenType != TokenType.RightParenthesis &&
                _lookAhead.TokenType != TokenType.RightBrackets)
            {
                E();//Entende expressões booleanas e matemáticas
                MatchToken();
                await D();//Depois pode ter qualquer coisa
            }
        }

        private async Task While()
        {
            if (_lookAhead.TokenType == TokenType.WhileCommand)
            {
                GenerateC3E(
                    $"{_c3eLineCounter}. WHILE:",
                    new Token(TokenType.Label, "WHILE:"),
                    null,
                    null,
                    null,
                    null);

                MatchToken();

                var leftValue = E();//Entende expressões booleanas e matemáticas

                GenerateC3E(
                    $"{_c3eLineCounter}. if {leftValue.TokenValue} == 0",
                    new Token(TokenType.IfCommand, "if"),
                    null,
                    leftValue,
                    new Token(TokenType.EqualToOperator, "=="),
                    new Token(TokenType.IntegerConstant, "0"));

                GenerateC3E(
                    $"{_c3eLineCounter}. goto 'END_WHILE:'",
                    new Token(TokenType.GotoCommand, "goto"),
                    new Token(TokenType.Label, "END_WHILE:"),
                    null,
                    null,
                    null);

                MatchToken(); //LeftBracers

                await D();//Depois pode ter qualquer coisa

                GenerateC3E(
                    $"{_c3eLineCounter}. goto 'WHILE:'",
                    new Token(TokenType.GotoCommand, "goto"),
                    new Token(TokenType.Label, "WHILE:"),
                    null,
                    null,
                    null);

                GenerateC3E(
                    $"{_c3eLineCounter}. 'END_WHILE:'",
                    new Token(TokenType.Label, "END_WHILE:"),
                    null,
                    null,
                    null,
                    null);

                MatchToken();
                await D();//Depois pode ter qualquer coisa
            }
        }

        private async Task If()
        {
            if (_lookAhead.TokenType == TokenType.IfCommand)
            {
                MatchToken();//match if
                var leftValue = E();//Entende expressões booleanas e matemáticas

                GenerateC3E(
                    $"{_c3eLineCounter}. if {leftValue.TokenValue} == 0",
                    new Token(TokenType.IfCommand, "if"),
                    null,
                    leftValue,
                    new Token(TokenType.EqualToOperator, "=="),
                    new Token(TokenType.IntegerConstant, "0"));

                GenerateC3E(
                    $"{_c3eLineCounter}. goto 'ELSE:'",
                    new Token(TokenType.GotoCommand, "goto"),
                    new Token(TokenType.Label, "ELSE:"),
                    null,
                    null,
                    null);

                MatchToken(); //LeftBracers
                await D();//Depois pode ter qualquer coisa
                    //como saber se vai ter um else ou não?
                MatchToken();//RightBracers

                GenerateC3E(
                    $"{_c3eLineCounter}. 'ELSE:'",
                    new Token(TokenType.Label, "ELSE:"),
                    null,
                    null,
                    null,
                    null);

                await Else();

                await D();//Depois pode ter qualquer coisa
            }
        }

        private async Task Else()
        {
            if (_lookAhead.TokenType == TokenType.ElseCommand)
            {
                MatchToken();//match else
                MatchToken(); //LeftBracers
                await D();//Depois pode ter qualquer coisa
                MatchToken();//RightBracers     
            }
        }

        private void V()
        {
            Y();
        }

        private void Y()
        {
            var typeVal = W();
            var idVal = MatchToken();

            idVal.TokenType = typeVal.TokenType;

            _varStack.Push(idVal);
            X(typeVal);
        }

        private Token X(Token typeVal)
        {
            if (_lookAhead.TokenType == TokenType.Comma)
            {
                MatchToken();
                var id = MatchToken();

                id.TokenType = typeVal.TokenType;

                _varStack.Push(id);
                X(typeVal);
                return id;
            }
            else if (_lookAhead.TokenType == TokenType.Semicolon)
            {
                return MatchToken();
            }

            throw new NotImplementedException();
        }

        private Token W()
        {
            if (_lookAhead.TokenType.IsType())
            {
                return MatchToken();
            }

            throw new NotImplementedException();
        }

        private void DoThingy(Token leftValue, Func<Token> func)
        {
            var opVal = MatchToken();
            var rightValue = func();

            GenerateC3E(leftValue, opVal, rightValue);
        }

        private Token E()
        {
            var leftValue = TT();
            leftValue = R(leftValue);
            //MatchToken();

            return leftValue;
        }

        private Token R(Token leftValue)
        {
            Token newLeftValue = new Token();

            if (_temporaryVarStack.Any())
                newLeftValue = _temporaryVarStack.Pop();

            if (_lookAhead.TokenType == TokenType.AdditionOperator || 
                _lookAhead.TokenType == TokenType.SubtractionOperator ||
                _lookAhead.TokenType == TokenType.AssignmentOperator)
            {
                DoThingy(leftValue.TokenValue is null ? newLeftValue : leftValue, () => E());

                return R(null);
            }

            return newLeftValue.TokenValue is null ? leftValue : newLeftValue;
        }

        private Token TT()
        {
            var leftValue = F();
            leftValue = SS(leftValue);

            return leftValue;
        }

        private Token SS(Token leftValue)
        {
            Token newLeftValue = new Token();

            if (_temporaryVarStack.Any())
                newLeftValue = _temporaryVarStack.Pop();

            if (_lookAhead.TokenType == TokenType.MultiplicationOperator || 
                _lookAhead.TokenType == TokenType.DivisionOperator ||
                _lookAhead.TokenType == TokenType.OrOperator)
            {
                DoThingy(leftValue.TokenValue is null ? newLeftValue : leftValue, () => TT());

                return SS(null);
            }

            return newLeftValue.TokenValue is null ? leftValue : newLeftValue;
        }

        private Token F()
        {
            var leftValue = G();
            leftValue = H(leftValue);

            return leftValue;
        }

        private Token H(Token leftValue)
        {
            Token newLeftValue = new Token();

            if (_temporaryVarStack.Any())
                newLeftValue = _temporaryVarStack.Pop();

            if (_lookAhead.TokenType == TokenType.PowOperator ||
                _lookAhead.TokenType == TokenType.AndOperator ||
                _lookAhead.TokenType.IsComparisonOperator())
            {
                DoThingy(leftValue.TokenValue is null ? newLeftValue : leftValue, () => F());

                return H(null);
            }

            return newLeftValue.TokenValue is null ? leftValue : newLeftValue;
        }

        private Token G()
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
                return MatchToken();
            }
            else if (_lookAhead.TokenType == TokenType.Identifier)
            {
                var id = MatchToken();

                if (!_varStack.Any(v => v.TokenValue == id.TokenValue))
                    throw new NotImplementedException();

                return id;
            }

            throw new NotImplementedException();
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

        private void GenerateC3E(Token leftValue, Token opVal, Token rightValue)
        {
            if (opVal.TokenType.IsComparisonOperator() || opVal.TokenType.IsLogicOperator())
            {
                if (opVal.TokenType.IsComparisonOperator())
                {
                    GenerateC3E(
                        $"{_c3eLineCounter}. if {leftValue.TokenValue} {opVal.TokenValue.Invert()} {rightValue.TokenValue}",
                        new Token(TokenType.IfCommand, "if"),
                        null,
                        leftValue,
                        opVal.Invert(),
                        rightValue);
                }
                else
                {
                    GenerateC3E(
                    $"{_c3eLineCounter}. if {leftValue.TokenValue} {opVal.TokenValue} {rightValue.TokenValue}",
                    new Token(TokenType.IfCommand, "if"),
                    null,
                    leftValue,
                    opVal,
                    rightValue);
                }

                var helper = _c3eLineCounter;

                GenerateC3E(
                    $"{_c3eLineCounter}. goto LS{helper}",
                    new Token(TokenType.GotoCommand, "goto"),
                    new Token(TokenType.Label, $"LS{helper}"));

                GenerateC3E(
                    $"{_c3eLineCounter}. T{_temporaryVarCounter} = 1",
                    null,
                    null,
                    new Token(TokenType.TempVariable, $"T{_temporaryVarCounter}"),
                    new Token(TokenType.AssignmentOperator, "="),
                    new Token(TokenType.IntegerConstant, "1"));

                var helper2 = _c3eLineCounter;

                GenerateC3E(
                    $"{_c3eLineCounter}. goto LS{helper2}",
                    new Token(TokenType.GotoCommand, "goto"),
                    new Token(TokenType.Label, $"LS{helper2}"));

                GenerateC3E(
                    $"{_c3eLineCounter}. 'LS{helper}:'",
                    new Token(TokenType.Label, $"LS{helper}:"),
                    null,
                    null,
                    null,
                    null);

                GenerateC3E(
                    $"{_c3eLineCounter}. T{_temporaryVarCounter} = 0",
                    null,
                    null,
                    new Token(TokenType.TempVariable, $"T{_temporaryVarCounter}"),
                    new Token(TokenType.AssignmentOperator, "="),
                    new Token(TokenType.IntegerConstant, "0"));

                GenerateC3E(
                    $"{_c3eLineCounter}. 'LS{helper2}:'",
                    new Token(TokenType.Label, $"LS{helper2}:"),
                    null,
                    null,
                    null,
                    null);

                _temporaryVarStack.Push(new Token(TokenType.TempVariable, $"T{_temporaryVarCounter}"));
                _temporaryVarStackAcumulator.Push(new Token(TokenType.TempVariable, $"T{_temporaryVarCounter}"));
                _temporaryVarCounter++;
            }
            else if (opVal.TokenType != TokenType.AssignmentOperator)
            {
                GenerateC3E(
                    $"{_c3eLineCounter}. T{_temporaryVarCounter} = {leftValue.TokenValue} {opVal.TokenValue} {rightValue.TokenValue}",
                    new Token(TokenType.AssignmentOperator, "="),
                    new Token(TokenType.TempVariable, $"T{_temporaryVarCounter}"),
                    leftValue,
                    opVal,
                    rightValue);

                _temporaryVarStack.Push(new Token(TokenType.TempVariable, $"T{_temporaryVarCounter}"));
                _temporaryVarStackAcumulator.Push(new Token(TokenType.TempVariable, $"T{_temporaryVarCounter}"));
                _temporaryVarCounter++;
            }
            else
            {
                GenerateC3E(
                    $"{_c3eLineCounter}. {leftValue.TokenValue} {opVal.TokenValue} {rightValue.TokenValue}",
                    null,
                    null,
                    leftValue,
                    opVal,
                    rightValue);
            }
        }

        private void GenerateC3E(
            string c3eValue = null,
            Token leftMostOperator = null,
            Token leftMostValue = null,
            Token leftValue = null,
            Token op = null,
            Token rightValue = null)
        {
            _c3eStack.Push(c3eValue);

            _c3eAnalyserResults.Add(new C3EAnalyserResult
            {
                LeftMostOperator = leftMostOperator,
                LeftMostValue = leftMostValue,
                LeftValue = leftValue,
                Operator = op,
                RightValue = rightValue
            });

            _c3eLineCounter++;
        }
    }
}
