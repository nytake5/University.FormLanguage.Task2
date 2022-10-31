namespace University.FormLanguage.Task2
{
    public class Lexeme
    {
        public LexemeType Type { get; set; }
        public LexemeClass Class { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"Type: {Type}, Class: {Class}, Value: {Value}";
        }
    }
}