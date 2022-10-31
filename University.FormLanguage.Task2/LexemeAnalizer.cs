using System.Text;

namespace University.FormLanguage.Task2
{
    public class LexemeAnalizer
    {
        public List<Lexeme> Lexems { get; set; } = new List<Lexeme>();

        public bool StartFindLexems(string text)
        {
            SystemState state = SystemState.Start;

            SystemState prevSystemState;
            bool isAbleToAdd;

            text += " ";

            StringBuilder lexBufNext = new StringBuilder();

            StringBuilder lexBufCur = new StringBuilder();

            int textIndex = 0;

            while (state != SystemState.Error && state != SystemState.Final)
            {
                prevSystemState = state;

                isAbleToAdd = true;

                if (textIndex == text.Length && state != SystemState.Error)
                {
                    state = SystemState.Final;
                    break;
                }

                if (textIndex == text.Length)
                {
                    break;
                }

                char symbol = text[textIndex];

                switch (state)
                {
                    case SystemState.Start:
                        if (char.IsWhiteSpace(symbol))
                        {
                            state = SystemState.Start;
                        }
                        else if (char.IsDigit(symbol))
                        {
                            state = SystemState.Constant;
                        }
                        else if (char.IsLetter(symbol))
                        {
                            state = SystemState.Identifier;
                        }
                        else if (symbol == '>')
                        {
                            state = SystemState.Comparison;
                        }
                        else if (symbol == '<')
                        {
                            state = SystemState.ReverseComparison;
                        }
                        else if (symbol == '+' || symbol == '-' || symbol == '/' || symbol == '*')
                        {
                            state = SystemState.ArithmeticOperation;
                        }
                        else if (symbol == '=')
                        {
                            state = SystemState.Assigment;
                        }
                        else
                        {
                            state = SystemState.Error;
                        }
                        isAbleToAdd = false;
                        if (!char.IsWhiteSpace(symbol))
                        {
                            lexBufCur.Append(symbol);
                        }
                        break;
                    case SystemState.Comparison:
                        if (char.IsWhiteSpace(symbol))
                        {
                            state = SystemState.Start;
                        }
                        else if (symbol == '=')
                        {
                            state = SystemState.Start;
                            lexBufCur.Append(symbol);
                        }
                        else if (char.IsLetter(symbol))
                        {
                            state = SystemState.Identifier;
                            lexBufNext.Append(symbol);
                        }
                        else if (char.IsDigit(symbol))
                        {
                            state = SystemState.Constant;
                            lexBufNext.Append(symbol);
                        }
                        else
                        {
                            state = SystemState.Error;
                            isAbleToAdd = false;
                        }
                        break;
                    case SystemState.ReverseComparison:
                        if (char.IsWhiteSpace(symbol))
                        {
                            state = SystemState.Start;
                        }
                        else if (symbol == '>')
                        {
                            state = SystemState.Start;
                            lexBufCur.Append(symbol);
                        }
                        else if (symbol == '=')
                        {
                            state = SystemState.Start;
                            lexBufCur.Append(symbol);
                        }
                        else if (char.IsLetter(symbol))
                        {
                            state = SystemState.Identifier;
                            lexBufNext.Append(symbol);
                        }
                        else if (char.IsDigit(symbol))
                        {
                            state = SystemState.Constant;
                            lexBufNext.Append(symbol);
                        }
                        else
                        {
                            state = SystemState.Error;
                            isAbleToAdd = false;
                        }
                        break;
                    case SystemState.Assigment:
                        if (symbol == '=')
                        {
                            state = SystemState.Comparison;
                            lexBufCur.Append(symbol);
                        }
                        else
                        {
                            state = SystemState.Start;
                            lexBufNext.Append(symbol);
                        }
                        break;
                    case SystemState.Constant:
                        if (char.IsWhiteSpace(symbol))
                        {
                            state = SystemState.Start;
                        }
                        else if (char.IsDigit(symbol))
                        {
                            isAbleToAdd = false;
                            state = SystemState.Constant;
                            lexBufCur.Append(symbol);
                        }
                        else if (symbol == '<')
                        {
                            state = SystemState.ReverseComparison;
                            lexBufNext.Append(symbol);
                        }
                        else if (symbol == '>')
                        {
                            state = SystemState.Comparison;
                            lexBufNext.Append(symbol);
                        }
                        else if (symbol == '=')
                        {
                            state = SystemState.Assigment;
                            lexBufNext.Append(symbol);
                        }
                        else if (symbol == '+' || symbol == '-')
                        {
                            state = SystemState.ArithmeticOperation;
                            lexBufCur.Append(symbol);
                        }
                        else if (symbol == '/' || symbol == '*')
                        {
                            state = SystemState.ArithmeticOperation;
                            lexBufNext.Append(symbol);
                        }
                        else
                        {
                            state = SystemState.Error;
                            isAbleToAdd = false;
                        }
                        break;
                    case SystemState.Identifier:
                        if (char.IsWhiteSpace(symbol)) state = SystemState.Start;
                        else if (char.IsDigit(symbol) || char.IsLetter(symbol))
                        {
                            state = SystemState.Identifier;
                            isAbleToAdd = false;
                            lexBufCur.Append(symbol);
                        }
                        else if (symbol == '<')
                        {
                            state = SystemState.ReverseComparison;
                            lexBufNext.Append(symbol);
                        }
                        else if (symbol == '>')
                        {
                            state = SystemState.Comparison;
                            lexBufNext.Append(symbol);
                        }
                        else if (symbol == '=')
                        {
                            state = SystemState.Assigment;
                            lexBufNext.Append(symbol);
                        }
                        else if (symbol == '+' || symbol == '-' || symbol == '/' || symbol == '*')
                        {
                            state = SystemState.ArithmeticOperation;
                            lexBufNext.Append(symbol);
                        }
                        else if (symbol == ':')
                        {
                            state = SystemState.Assigment;
                            lexBufNext.Append(symbol);
                        }
                        else
                        {
                            state = SystemState.Error;
                            isAbleToAdd = false;
                        }
                        break;
                    case SystemState.ArithmeticOperation:
                        if (char.IsWhiteSpace(symbol))
                        {
                            state = SystemState.Start;
                        }
                        else if (char.IsLetter(symbol))
                        {
                            state = SystemState.Identifier;
                            lexBufNext.Append(symbol);
                        }
                        else if (char.IsDigit(symbol))
                        {
                            state = SystemState.Constant;
                            lexBufNext.Append(symbol);
                        }
                        else if (symbol == '-' || symbol == '+' || symbol == '/' || symbol == '*')
                        {
                            state = SystemState.ArithmeticOperation;
                            lexBufNext.Append(symbol);
                        }
                        else
                        {
                            state = SystemState.Error;
                            isAbleToAdd = false;
                        }
                        break;
                }

                if (isAbleToAdd)
                {
                    if (!AddDiffLexeme(prevSystemState, lexBufCur.ToString()))
                    {
                        return false;
                    }
                    lexBufCur = new StringBuilder(lexBufNext.ToString());
                    lexBufNext.Clear();
                }

                textIndex++;
            }
            return state == SystemState.Final;
        }

        private bool AddDiffLexeme(SystemState prevState, string value)
        {
            LexemeType lexType = LexemeType.Undefined;

            LexemeClass lexClass = LexemeClass.Undefined;

            if (prevState == SystemState.ArithmeticOperation)
            {
                lexType = LexemeType.ArithmeticOperation;
                lexClass = LexemeClass.SpecialSymbols;
            }
            else if (prevState == SystemState.Assigment)
            {
                lexClass = LexemeClass.SpecialSymbols;
                if (value == "==")
                {
                    lexType = LexemeType.Relation;
                }
                else
                {
                    lexType = LexemeType.Assigment;
                }
            }
            else if (prevState == SystemState.Constant)
            {
                lexType = LexemeType.Undefined;
                lexClass = LexemeClass.Constant;
            }
            else if (prevState == SystemState.ReverseComparison)
            {
                lexType = LexemeType.Relation;
                lexClass = LexemeClass.SpecialSymbols;
            }
            else if (prevState == SystemState.Comparison)
            {
                lexType = LexemeType.Relation;
                lexClass = LexemeClass.SpecialSymbols;
            }
            else if (prevState == SystemState.Identifier)
            {
                bool isKeyword = true;
                if (value.ToLower() == "if")
                {
                    lexType = LexemeType.If;
                }
                else if (value.ToLower() == "and")
                {
                    lexType = LexemeType.And;
                }
                else if (value.ToLower() == "or")
                {
                    lexType = LexemeType.Or;
                }
                else if (value.ToLower() == "else")
                {
                    lexType = LexemeType.Else;
                }
                else if (value.ToLower() == "then")
                {
                    lexType = LexemeType.Then;
                }
                else if (value.ToLower() == "end")
                {
                    lexType = LexemeType.End;
                }
                else
                {
                    lexType = LexemeType.Undefined;
                    isKeyword = false;
                }
                if (isKeyword)
                {
                    lexClass = LexemeClass.Keyword;
                }
                else
                {
                    lexClass = LexemeClass.Identifier;
                }
            }

            Lexeme lexeme = new()
            {
                Class = lexClass,
                Type = lexType,
                Value = value.Trim(),
            };


            if (lexeme.Value.Length > 0)
            {
                Lexems.Add(lexeme);
            }
            return true;
        }
    }
}
