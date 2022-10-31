namespace University.FormLanguage.Task2
{
    public enum LexemeType
    {
        If,
        Then,
        Else,
        End,
        And,
        Or,
        ArithmeticOperation,
        Relation,
        Assigment,
        Undefined
    }

    public enum LexemeClass
    {
        Keyword,
        Identifier,
        Constant,
        SpecialSymbols,
        Undefined
    }

    public enum SystemState
    {
        Start,
        Identifier,
        Constant,
        Error,
        Final,
        Comparison,
        ReverseComparison,
        ArithmeticOperation,
        Assigment
    }
}
