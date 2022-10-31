using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.FormLanguage.Task2
{
    public class SyntaxAnalyzer
    {
        private List<Lexeme> Lexems { get; set; }

        private IEnumerator<Lexeme> enumerator { get; set; }

        private StringBuilder chain { get; set; }

        public bool Analyze(string text)
        {
            LexemeAnalizer analyzer = new();

            if (!analyzer.StartFindLexems(text))
            {
                Error("Возникла ошибка в лексическом анализаторе");
                return false;
            }

            return Analize(analyzer.Lexems);
        }

        private bool Analize(List<Lexeme> lexems)
        {
            Lexems = lexems;
            enumerator = lexems.GetEnumerator();
            if (lexems.Count == 0)
                return false;
            chain = new StringBuilder();
            if (!enumerator.MoveNext() || enumerator.Current.Type != LexemeType.If)
            {
                Error($"Ожидается if: {Lexems.IndexOf(enumerator.Current)}");
                return false;
            }
            chain.Append(enumerator.Current.Value + " ");
            enumerator.MoveNext();

            if (!IsRelationalExpression())
            {
                return false;
            }

            if (enumerator.Current.Type != LexemeType.Then)
            {
                Error($"Ожидается then: {Lexems.IndexOf(enumerator.Current)}");
                return false;
            }
            chain.Append("\n" + enumerator.Current.Value);
            enumerator.MoveNext();
            Console.WriteLine(chain.ToString());
            chain = new StringBuilder("\t");

            if (!IsArithmeticExpression())
                return false;
            Console.WriteLine(chain.ToString());


            if (enumerator.Current.Type != LexemeType.End)
            {
                if (enumerator.Current.Type != LexemeType.Else)
                {
                    Error($"Ожидается end или else: {Lexems.IndexOf(enumerator.Current)}");
                    return false;

                }
                else
                {
                    Console.WriteLine("else");
                    chain = new StringBuilder("\t");
                    enumerator.MoveNext();
                    if (!IsArithmeticExpression())
                        return false;
                    Console.WriteLine(chain.ToString());
                }
            }
            Console.WriteLine(enumerator.Current.Value);


            if (enumerator.MoveNext())
            {
                Error($"Лишние символы: {Lexems.IndexOf(enumerator.Current)}");
                return false;

            }


            return true;
        }

        private bool IsArithmeticExpression()
        {
            if (!IsOperand()) 
                return false;
            while (enumerator.Current.Type == LexemeType.Assigment || enumerator.Current.Type == LexemeType.ArithmeticOperation)
            {
                chain.Append(enumerator.Current.Value + " ");
                enumerator.MoveNext();
                if (!IsOperand()) 
                    return false;
            }
            return true;
        }
        private bool IsRelationalExpression()
        {
            if (!IsOperand())
                return false;
            if (enumerator.Current.Type == LexemeType.Relation)
            {
                chain.Append(enumerator.Current.Value + " ");
                enumerator.MoveNext();
                if (!IsOperand()) 
                    return false;
            }
            return true;
        }

        private bool IsOperand()
        {
            if (enumerator.Current == null || (enumerator.Current.Class != LexemeClass.Identifier && enumerator.Current.Class != LexemeClass.Constant))
            {
                Error($"Ожидается переменная или константа {Lexems.IndexOf(enumerator.Current, 0)}");
                return false;
            }
            chain.Append(enumerator.Current.Value + " ");

            enumerator.MoveNext();
            return true;
        }

        private void Error(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine((error));
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
