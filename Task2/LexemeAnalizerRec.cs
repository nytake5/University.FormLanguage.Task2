using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.FormLanguage.Task2;

namespace Task2
{
    public class LexemeAnalizerRec
    {
        private List<Lexeme> _lexemeList;
        private IEnumerator<Lexeme> _lexemeEnumerator;
        public bool Run(string code)
        {
            var analyser = new LexicalAnalyzer();
            var result = analyser.Run(string.Join(Environment.NewLine, code));
            if (!result)
            {
                throw new Exception("Errors were occurred in lexical analyze");
            }
            return IsWhileStatement(analyser.Lexemes);
        }
        private bool IsWhileStatement(List<Lexeme> lexemeList)
        {
            _lexemeList = lexemeList;
            if (lexemeList.Count == 0) return false;
            _lexemeEnumerator = lexemeList.GetEnumerator();

            if (!_lexemeEnumerator.MoveNext() || _lexemeEnumerator.Current.Type != LexemeType.For) { Error("Ожидается for", _lexemeList.IndexOf(_lexemeEnumerator.Current)); }

            _lexemeEnumerator.MoveNext();
            if (!IsIdentifier()) return false;

            if (_lexemeEnumerator.Current == null || (_lexemeEnumerator.Current.Class != LexemeClass.SpecialSymbols && _lexemeEnumerator.Current.Value != "=")) { Error("Ожидается '='", _lexemeList.IndexOf(_lexemeEnumerator.Current)); }

            _lexemeEnumerator.MoveNext();
            if (!IsArithmeticExpression()) return false;

            if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Type != LexemeType.To) { Error("Ожидается to", _lexemeList.IndexOf(_lexemeEnumerator.Current)); }

            _lexemeEnumerator.MoveNext();
            if (!IsArithmeticExpression()) return false;

            while (IsStatement()) ;
            if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Type != LexemeType.Next) { Error("Ожидается next", _lexemeList.IndexOf(_lexemeEnumerator.Current)); }
            if (_lexemeEnumerator.MoveNext()) { Error("Лишние символы", _lexemeList.IndexOf(_lexemeEnumerator.Current)); }
            return true;
        }

        private bool IsCondition()
        {
            if (!IsLogicalExpression()) return false;
            while (_lexemeEnumerator.Current.Type == LexemeType.Or)
            {
                _lexemeEnumerator.MoveNext();
                if (!IsLogicalExpression()) return false;
            }
            return true;
        }
        private bool IsLogicalExpression()
        {
            if (!RelationalExpression()) return false;
            while (_lexemeEnumerator.Current.Type == LexemeType.And)
            {
                _lexemeEnumerator.MoveNext();
                if (!RelationalExpression()) return false;
            }
            return true;
        }
        private bool RelationalExpression()
        {
            if (!IsOperand()) return false;
            if (_lexemeEnumerator.Current.Type == LexemeType.Relation)
            {
                _lexemeEnumerator.MoveNext();
                if (!IsOperand()) return false;
            }
            return true;
        }
        private bool IsIdentifier()
        {
            if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Class != LexemeClass.Identifier)
            {
                Error("Ожидается переменная", _lexemeList.IndexOf(_lexemeEnumerator.Current));
                return false;
            }
            _lexemeEnumerator.MoveNext();
            return true;
        }
        private bool IsOperand()
        {
            if (_lexemeEnumerator.Current == null || (_lexemeEnumerator.Current.Class != LexemeClass.Identifier && _lexemeEnumerator.Current.Class != LexemeClass.Constant))
            {
                Error("Ожидается переменная или константа", _lexemeList.IndexOf(_lexemeEnumerator.Current));
                return false;
            }
            _lexemeEnumerator.MoveNext();
            return true;
        }
        private bool IsLogicalOperation()
        {
            if (_lexemeEnumerator.Current == null || (_lexemeEnumerator.Current.Type != LexemeType.And && _lexemeEnumerator.Current.Type != LexemeType.Or))
            {
                Error("Ожидается логическая операция", _lexemeList.IndexOf(_lexemeEnumerator.Current));
                return false;
            }
            _lexemeEnumerator.MoveNext();
            return true;
        }
        private bool IsStatement()
        {
            if (_lexemeEnumerator.Current != null && _lexemeEnumerator.Current.Type == LexemeType.Next) return false;
            if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Class != LexemeClass.Identifier)
            {
                if (_lexemeEnumerator.Current.Type == LexemeType.Output)
                {
                    _lexemeEnumerator.MoveNext();
                    if (!IsOperand()) return false;
                    return true;
                }
                Error("Ожидается переменная", _lexemeList.IndexOf(_lexemeEnumerator.Current));
                return false;
            }
            _lexemeEnumerator.MoveNext();
            if (_lexemeEnumerator.Current == null || _lexemeEnumerator.Current.Type != LexemeType.Relation)
            {
                Error("Ожидается присваивание", _lexemeList.IndexOf(_lexemeEnumerator.Current));
                return false;
            }
            _lexemeEnumerator.MoveNext();
            if (!IsArithmeticExpression()) return false;
            return true;
        }
        private bool IsArithmeticExpression()
        {
            if (!IsOperand()) return false;
            while (_lexemeEnumerator.Current.Type == LexemeType.ArithmeticOperation)
            {
                _lexemeEnumerator.MoveNext();
                if (!IsOperand()) return false;
            }
            return true;
        }
        private void Error(string message, int position)
        {
            throw new Exception($"{message} в позиции: {position}");
        }
    }
}
