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

        public List<PostfixEntry> EntryList { get; set; }

        private Stack<PostfixEntry> _stack;

        public bool Analyze(string text, out List<PostfixEntry> list)
        {
            LexemeAnalizer analyzer = new();
            _stack = new();
            if (!analyzer.StartFindLexems(text))
            {
                Error("Возникла ошибка в лексическом анализаторе");
                list = new();
                return false;
            }

            return Analize(analyzer.Lexems, out list);
        }

        private bool Analize(List<Lexeme> lexems, out List<PostfixEntry> answersEntries)
        {
            answersEntries = new();
            EntryList = new();
            int ind = EntryList.Count;
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
            while (enumerator.Current.Type == LexemeType.Or || enumerator.Current.Type == LexemeType.And)
            {
                var temp = enumerator.Current.Type;
                enumerator.MoveNext();
                if (!IsRelationalExpression())
                {
                    return false;
                }

                int x = temp switch
                {
                    LexemeType.And => WriteCmd(Cmd.AND),
                    LexemeType.Or => WriteCmd(Cmd.OR),
                    _ => throw new ArgumentException(enumerator.Current.Value)
                };
            }
            int indexOfJzAdress = EntryList.Count - 1;
            if (enumerator.Current.Type != LexemeType.Then)
            {
                Error($"Ожидается then: {Lexems.IndexOf(enumerator.Current)}");
                return false;
            }
            chain.Append("\n" + enumerator.Current.Value);
            enumerator.MoveNext();
            Console.WriteLine(chain.ToString());
            chain = new StringBuilder("\t");
            WriteCmdPtr(1);
            WriteCmd(Cmd.JMP);
            if (!IsArithmeticExpression())
                return false;

            Console.WriteLine(chain.ToString());

            int indexOfStartElse = EntryList.Count - 1;
            List<PostfixEntry> resultEntryList = EntryList;
            bool flag = false;
            if (enumerator.Current.Type != LexemeType.End)
            {
                if (enumerator.Current.Type != LexemeType.Else)
                {
                    Error($"Ожидается end или else: {Lexems.IndexOf(enumerator.Current)}");
                    return false;
                }
                else
                {
                    flag = true;
                    Console.WriteLine("else");
                    chain = new StringBuilder("\t");
                    enumerator.MoveNext();
                    if (!IsArithmeticExpression())
                        return false;
                    Console.WriteLine(chain.ToString());
                    resultEntryList = EntryList.Take(indexOfJzAdress + 1).ToList();
                    int tempIndex = resultEntryList.Count;
                    resultEntryList.Add(new()
                    {
                        EntryType = EntryType.CmdPtr,
                    });

                    resultEntryList.Add(new()
                    {
                        EntryType = EntryType.Cmd,
                        Cmd = Cmd.JZ
                    });
                    foreach (var item in EntryList.Skip(indexOfStartElse + 1))
                    {
                        resultEntryList.Add(item);
                    }

                    foreach (var item in EntryList.Skip(indexOfJzAdress + 1).Take(indexOfStartElse - indexOfJzAdress))
                    {
                        resultEntryList.Add(item);
                    }
                    resultEntryList[tempIndex].CmdPtr = resultEntryList.LastIndexOf(resultEntryList.FirstOrDefault(x => x.Cmd == Cmd.JMP)) + 1;
                }
            }
            int indexJmpPtr = resultEntryList.LastIndexOf(resultEntryList.FirstOrDefault(x => x.Cmd == Cmd.JMP)) - 1;
            if (flag)
            {
                resultEntryList[indexJmpPtr].CmdPtr = resultEntryList.Count;
            }
            else
            {
                resultEntryList[indexJmpPtr].CmdPtr = indexJmpPtr + 2;
            }
            Console.WriteLine(enumerator.Current.Value);

            answersEntries = resultEntryList;
            EntryList = resultEntryList;
            if (enumerator.MoveNext())
            {
                Error($"Лишние символы: {Lexems.IndexOf(enumerator.Current)}");
                return false;
            }

            foreach (var entry in answersEntries)
            {
                if (entry.EntryType == EntryType.Var)
                {
                    Console.Write($"{entry.Value} ");
                }
                else if (entry.EntryType == EntryType.Const)
                {
                    Console.Write($"{entry.Value} ");
                }
                else if (entry.EntryType == EntryType.Cmd)
                {
                    Console.Write($"{entry.Cmd} ");
                }
                else if (entry.EntryType == EntryType.CmdPtr)
                {
                    Console.Write($"ptr{entry.CmdPtr} ");
                }
            }

            WriteVariables();
            Console.WriteLine("Result: ");
            Interpret();
            return true;
        }

        private void WriteVariables()
        {
            try
            {
                Console.WriteLine("Enter variable values:");

                var variables = GetVariables().Select(v => v.Value).Distinct();
                foreach (var variable in variables)
                {
                    Console.Write($"{variable} = ");
                    var value = int.Parse(Console.ReadLine());
                    SetValuesToVariables(variable, value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool IsArithmeticExpression()
        {
            if (!IsOperand())
                return false;
            if (enumerator.Current.Type == LexemeType.Assigment)
            {
                chain.Append(enumerator.Current.Value + " ");
                enumerator.MoveNext();
            }
            if (!IsOperand())
                return false;
            while (enumerator.Current.Type == LexemeType.Assigment || enumerator.Current.Type == LexemeType.ArithmeticOperation)
            {
                chain.Append(enumerator.Current.Value + " ");
                var cmd = enumerator.Current.Value switch
                {
                    "+" => Cmd.ADD,
                    "-" => Cmd.SUB,
                    "*" => Cmd.MUL,
                    "/" => Cmd.DIV,
                    _ => throw new ArgumentException(enumerator.Current.Value)
                };
                enumerator.MoveNext();
                if (!IsOperand())
                    return false;
                WriteCmd(cmd);
            }
            WriteCmd(Cmd.SET);
            return true;
        }
        private bool IsRelationalExpression()
        {
            if (!IsOperand())
                return false;
            while (enumerator.Current.Type == LexemeType.Relation)
            {
                chain.Append(enumerator.Current.Value + " ");
                Cmd? cmd = enumerator.Current.Value switch
                {
                    "<" => Cmd.CMPL,
                    "<=" => Cmd.CMPLE,
                    ">" => Cmd.CMPG,
                    ">=" => Cmd.CMPGE,
                    "==" => Cmd.CMPE,
                    "<>" => Cmd.CMPNE,
                    _ => throw new ArgumentException(enumerator.Current.Value)
                };
                enumerator.MoveNext();
                if (!IsOperand())
                    return false;

                /*if (enumerator.Current)
                {
                    var temp = enumerator.Current.Type;
                    enumerator.MoveNext();
                    IsRelationalExpression();
                    int x = temp switch
                    {
                        LexemeType.And => WriteCmd(Cmd.AND),
                        LexemeType.Or => WriteCmd(Cmd.OR),
                        _ => throw new ArgumentException(enumerator.Current.Value)
                    };
                }*/
                WriteCmd(cmd.Value);
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
            if (enumerator.Current.Class == LexemeClass.Identifier)
            {
                WriteVar(Lexems.IndexOf(enumerator.Current));
            }
            else
            {
                WriteConst(Lexems.IndexOf(enumerator.Current));
            }
            chain.Append(enumerator.Current.Value + " ");

            enumerator.MoveNext();
            return true;
        }
        private int WriteCmd(Cmd cmd)
        {
            var command = new PostfixEntry
            {
                EntryType = EntryType.Cmd,
                Cmd = cmd,
            };
            EntryList.Add(command);
            return EntryList.Count - 1;
        }
        private int WriteVar(int index)
        {
            var variable = new PostfixEntry
            {
                EntryType = EntryType.Var,
                Value = Lexems[index].Value
            };
            EntryList.Add(variable);
            return EntryList.Count - 1;
        }
        private int WriteConst(int index)
        {
            var variable = new PostfixEntry
            {
                EntryType = EntryType.Const,
                Value = Lexems[index].Value
            };
            EntryList.Add(variable);
            return EntryList.Count - 1;
        }
        private int WriteCmdPtr(int ptr)
        {
            var cmdPtr = new PostfixEntry
            {
                EntryType = EntryType.CmdPtr,
                CmdPtr = ptr,
            };
            EntryList.Add(cmdPtr);
            return EntryList.Count - 1;
        }
        private void SetCmdPtr(int index, int ptr)
        {
            EntryList[index].CmdPtr = ptr;
        }

        private void Error(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine((error));
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void Interpret()
        {
            int temp;
            int pos = 0;
            Console.WriteLine($"Position: {pos}, Value: {GetEntryString(EntryList[pos])}, Stack: {GetStackState()}, Variable value: {GetVarValues()}");

            while (pos < EntryList.Count)
            {
                if (EntryList[pos].EntryType == EntryType.Cmd)
                {
                    var cmd = EntryList[pos].Cmd;
                    switch (cmd)
                    {
                        case Cmd.JMP:
                            pos = PopVal();
                            break;
                        case Cmd.JZ:
                            temp = PopVal();
                            if (PopVal() == 0) pos++; else pos = temp;
                            break;
                        case Cmd.SET:
                            SetVarAndPop(PopVal());
                            pos++;
                            break;
                        case Cmd.ADD:
                            PushVal(PopVal() + PopVal());
                            pos++;
                            break;
                        case Cmd.SUB:
                            var temp1 = -PopVal();
                            var temp2 = PopVal();
                            PushVal(temp1 + temp2);
                            pos++;
                            break;
                        case Cmd.MUL:
                            PushVal(PopVal() * PopVal());
                            pos++;
                            break;
                        case Cmd.DIV:
                            PushVal((int)(1.0 / PopVal() * PopVal()));
                            pos++;
                            break;
                        case Cmd.AND:
                            PushVal((PopVal() != 0 && PopVal() != 0) ? 1 : 0);
                            pos++;
                            break;
                        case Cmd.OR:
                            PushVal((PopVal() != 0 || PopVal() != 0) ? 1 : 0);
                            pos++;
                            break;
                        case Cmd.CMPE:
                            PushVal((PopVal() == PopVal()) ? 1 : 0);
                            pos++;
                            break;
                        case Cmd.CMPNE:
                            PushVal((PopVal() != PopVal()) ? 1 : 0);
                            pos++;
                            break;
                        case Cmd.CMPL:
                            PushVal((PopVal() > PopVal()) ? 1 : 0);
                            pos++;
                            break;
                        case Cmd.CMPLE:
                            PushVal((PopVal() >= PopVal()) ? 1 : 0);
                            pos++;
                            break;
                        case Cmd.CMPG:
                            PushVal((PopVal() < PopVal()) ? 1 : 0);
                            pos++;
                            break;
                        case Cmd.CMPGE:
                            PushVal((PopVal() <= PopVal()) ? 1 : 0);
                            pos++;
                            break;
                        default:
                            break;
                    }
                }
                else PushElm(EntryList[pos++]);

                if (pos < EntryList.Count)
                    Console.WriteLine($"Position: {pos}, Value: {GetEntryString(EntryList[pos])}, Stack: {GetStackState()}, Variable value: {GetVarValues()}");
                else
                {
                    Console.WriteLine($"Variable value: {GetVarValues()}");
                }
            }
        }

        private int PopVal()
        {
            if (_stack.Count != 0)
            {
                var obj = _stack.Pop();
                return obj.EntryType switch
                {
                    EntryType.Var => obj.CurrentValue.Value,
                    EntryType.Const => Convert.ToInt32(obj.Value),
                    EntryType.CmdPtr => obj.CmdPtr.Value,
                    _ => throw new ArgumentException("obj.EntryType")
                };
            }
            else
            {
                return 0;
            }
        }

        private void PushVal(int val)
        {
            var entry = new PostfixEntry
            {
                EntryType = EntryType.Const,
                Value = val.ToString()
            };
            _stack.Push(entry);
        }

        private void PushElm(PostfixEntry entry)
        {
            if (entry.EntryType == EntryType.Cmd)
            {
                throw new ArgumentException("EntryType");
            }
            _stack.Push(entry);
        }

        private void SetVarAndPop(int val)
        {
            var variable = _stack.Pop();
            if (variable.EntryType != EntryType.Var)
            {
                throw new ArgumentException("EntryType");
            }
            SetValuesToVariables(variable.Value, val);
        }

        private string GetEntryString(PostfixEntry entry)
        {
            if (entry.EntryType == EntryType.Var) return entry.Value;
            else if (entry.EntryType == EntryType.Const) return entry.Value;
            else if (entry.EntryType == EntryType.Cmd) return entry.Cmd.ToString();
            else if (entry.EntryType == EntryType.CmdPtr) return entry.CmdPtr.ToString();
            throw new ArgumentException("PostfixEntry");
        }

        private string GetStackState()
        {
            IEnumerable<PostfixEntry> entries = _stack;
            var sb = new StringBuilder();
            entries?.ToList().ForEach(e => sb.Append($"{GetEntryString(e)} "));
            return sb.ToString();
        }

        private string GetVarValues()
        {
            var sb = new StringBuilder();
            EntryList.Where(e => e.EntryType == EntryType.Var).Select(e => new { e.Value, e.CurrentValue }).Distinct().ToList().ForEach(e => sb.Append($"{e.Value} = {e.CurrentValue}; "));
            return sb.ToString();
        }

        private IEnumerable<PostfixEntry> GetVariables()
        {
            var temp = EntryList.Where(e => e.EntryType == EntryType.Var).ToList();
            return temp;
        }

        private void SetValuesToVariables(string name, int value)
        {
            GetVariables().Where(v => v.Value == name).ToList().ForEach(v => v.CurrentValue = value);
        }
    }
}
