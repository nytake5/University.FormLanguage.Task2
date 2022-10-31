// See https://aka.ms/new-console-template for more information


using University.FormLanguage.Task2;

string input = Console.ReadLine();

LexemeAnalizer analizer = new LexemeAnalizer();
//if b <= a then b = b * 53 else a = b / 2 end
//if b <= a then b = b * 53 end
//ifa a > 0 then a = a - 353 end
//if a > 0 then a = a - 353 else end
/*
if (analizer.StartFindLexems(input)) //"ifa a >= b and a == 0 then if b <= a or b >= 10 then b = b * 53 end else a = a + 535 end"))
{
    Console.WriteLine("True");
}
else
{
    Console.WriteLine("False");
}

List<Lexeme> lexems = analizer.Lexems;

foreach (var item in lexems)
{
    Console.WriteLine(item.ToString());
}*/
SyntaxAnalyzer syntax = new();
if(syntax.Analyze(input))
{
    Console.WriteLine("True");
}
else
{
    Console.WriteLine("False");
}