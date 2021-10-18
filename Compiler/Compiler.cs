using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Compiler
{
    #region helperClasses
    internal class Token
    {
        public char A { get; set; }
        public int N { get; set; }
    }

    internal class Variable
    {
        public string varName;
        public char typeOfVar;
        private bool isArr;
        private int size;
        public Variable(string varName, char typeOfVar)
        {
            this.varName = varName;
            this.typeOfVar = typeOfVar;
            isArr = false;
            size = 0;
        }
        public Variable(string varName, char typeOfVar, bool isArr, int size) : this(varName, typeOfVar)
        {
            this.isArr = isArr;
            this.size = isArr ? size : 0;
        }
        public bool IsArray() => isArr;
        
        public int Get_Count() => isArr ? size : 0;
    }
    #endregion helperClasses
    
    public class CompilerC
    {
        #region init

        private int OpenBrackets { get; set; } = 0;
        private int LabelCount { get; set; } = 0;
        private char[] Arr { get; set; }
        private int Count { get; set; } = 0;
        private int AmountVar { get; set; } = 0;
        List<Variable> arrayOfVariables = new List<Variable>();
        List<string> keyWorlds = new List<string>();
        List<char> separatorsArray = new List<char>();
        List<string> doubleSeparatorsArray = new List<string>();
        List<char> symbolsArray = new List<char>();
        List<int> numbersArray = new List<int>();
        List<string> worldsArray = new List<string>();
        List<string> quotesTextArray = new List<string>();
        List<string> _dataVar = new List<string>();
        List<string> _mainVar = new List<string>();

        private void FillArray()
        {
            separatorsArray.Add('+'); //0
            separatorsArray.Add('-'); //1
            separatorsArray.Add('='); //2
            separatorsArray.Add('*'); //3
            separatorsArray.Add('/'); //4
            separatorsArray.Add('('); //5
            separatorsArray.Add(')'); //6
            separatorsArray.Add('{'); //7
            separatorsArray.Add('}'); //8
            separatorsArray.Add(':'); //9
            separatorsArray.Add(';'); //10
            separatorsArray.Add('#'); //11
            separatorsArray.Add('.'); //12
            separatorsArray.Add('['); //13
            separatorsArray.Add(']'); //14
            separatorsArray.Add('>'); //15
            separatorsArray.Add('<'); //16
            separatorsArray.Add(','); //17

            doubleSeparatorsArray.Add("=="); //0
            doubleSeparatorsArray.Add("!="); //1
            doubleSeparatorsArray.Add(">="); //2
            doubleSeparatorsArray.Add("<="); //3
            doubleSeparatorsArray.Add("+="); //4
            doubleSeparatorsArray.Add("-="); //5
            doubleSeparatorsArray.Add("/="); //6
            doubleSeparatorsArray.Add("--"); //7 
            doubleSeparatorsArray.Add("++"); //8
            doubleSeparatorsArray.Add("//"); //9
            doubleSeparatorsArray.Add("/*"); //10

            symbolsArray.Add(' ');
            symbolsArray.Add('\n');
            symbolsArray.Add('\t');
            symbolsArray.Add('\r');

            keyWorlds.Add("or");         //0
            keyWorlds.Add("and");        //1
            keyWorlds.Add("true");       //2
            keyWorlds.Add("false");      //3
            keyWorlds.Add("not");        //4
            keyWorlds.Add("array");      //5
            keyWorlds.Add("of");         //6
            keyWorlds.Add("integer");    //7
            keyWorlds.Add("char");       //8
            keyWorlds.Add("boolean");    //9
            keyWorlds.Add("declare");    //10
            keyWorlds.Add("program");    //11
            keyWorlds.Add("main");       //12
            keyWorlds.Add("end");        //13
            keyWorlds.Add("let");        //14
            keyWorlds.Add("if");         //15
            keyWorlds.Add("while");      //16
            keyWorlds.Add("then");       //17
            keyWorlds.Add("else");       //18
            keyWorlds.Add("do");         //19
            keyWorlds.Add("read");       //20
            keyWorlds.Add("write");      //21
            keyWorlds.Add("break");      //22
            keyWorlds.Add("case");       //23
            keyWorlds.Add("default");    //24
            keyWorlds.Add("for");        //25
            keyWorlds.Add("void");       //26
            keyWorlds.Add("xor");        //27
        }
        #endregion init

        #region interface
        
        public CompilerC(string filePath = @"C:\Archive\6 Семестр\СПО\Automat\text.txt")
        {
            var file = new FileStream(filePath, FileMode.Open);
            using (var sr = new StreamReader(file, System.Text.Encoding.Default))
            {
                var text = sr.ReadToEnd();
                Arr = text.ToCharArray();
            }
            FillArray();
        }

        public void Compile()
        {
            _mainVar.Add(".8086");
            _mainVar.Add(".model small");
            _mainVar.Add(".stack 100h");
            _mainVar.Add(".code");
            _mainVar.Add("main:");
            _mainVar.Add("mov ax, DGROUP");
            _mainVar.Add("mov ds, ax");
            _mainVar.Add("mov ax, 4C00h");
            _dataVar.Add(".data");
            _dataVar.Add("@buffer db 6");
            _dataVar.Add("blength db (?)");
            _dataVar.Add("@buf db 256 DUP (?)");
            _dataVar.Add("output db 6 DUP (?)");
            _dataVar.Add("err_msg db  \"Input error, try again\", 0Dh, 0Ah, \"$\"");
            _dataVar.Add("@true db \"true\"");
            _dataVar.Add("@false db \"false\"");
            _dataVar.Add("@@true db \"true$\"");
            _dataVar.Add("@@false db \"false$\"");
            _dataVar.Add("clear db 0Dh, 0Ah, \"$\""); 
            Prog();
            _dataVar.Add("end main");
        }

        public void WriteCompileFile(string filePath = @"C:\Archive\6 Семестр\СПО\Automat\programm.asm")
        {
            var fileRes = new FileStream(filePath, FileMode.Create);
            using (fileRes)
            {
                using (var sw = new StreamWriter(fileRes, System.Text.Encoding.Default))
                {
                    foreach (var item in _mainVar)
                    {
                        sw.WriteLine(item);
                    }
                    foreach (var item in _dataVar)
                    {
                        sw.WriteLine(item);
                    }
                }
            }
        }
        
        #endregion interface
        
        #region main
        private Token CheckNum(int num, List<int> arr)
        {
            var id = new Token {A = 'C'};
            if (arr.Contains(num))
            {
                for (var i = 0; i < arr.Count; i++)
                    if (arr[i].Equals(num))
                        id.N = i;
            }
            else
            {
                arr.Add(num);
                id.N = arr.Count - 1;
            }
            return id;
        }

        private Token CheckText(string text, List<string> arr, List<string> keyWord)
        {
            var id = new Token();
            if (keyWord.Contains(text))
            {
                id.A = 'K';
                for (var i = 0; i < keyWord.Count; i++)
                    if (keyWord[i].Equals(text))
                        id.N = i;
            }
            else if (arr.Contains(text))
            {
                id.A = 'I';
                for (int i = 0; i < arr.Count; i++)
                    if (arr[i].Equals(text))
                        id.N = i;
            }
            else
            {
                id.A = 'I';
                arr.Add(text);
                id.N = arr.Count - 1;
            }
            return id;
        }

        private Token CheckDoubleSeparators(string text, List<string> arr)
        {
            var id = new Token {A = 'D'};
            for (var i = 0; i < arr.Count; i++)
                if (arr[i].Equals(text))
                    id.N = i;
            return id;
        }

        private Token CheckSeparators(char text, List<char> arr)
        {
            var id = new Token {A = 'R'};
            for (var i = 0; i < arr.Count; i++)
                if (arr[i].Equals(text))
                    id.N = i;
            return id;
        }

        private Token CheckQuotesText(string text, List<string> arr)
        {
            var id = new Token {A = 'L'};
            if (arr.Contains(text))
            {
                for (var i = 0; i < arr.Count; i++)
                    if (arr[i].Equals(text))
                        id.N = i;
            }
            else
            {
                arr.Add(text);
                id.N = arr.Count - 1;
            }
            return id;
        }

        private Token Scan(bool check)
        {
            var num = 0;
            var done = false;
            var temp = "";
            var id = new Token {A = '-', N = 0};
            if (Count > Arr.Length)
            {
                return id;
            }
            try
            {
                while (Count < Arr.Length)
                {
                    if (done)
                    {
                        done = false;
                        break;
                    }

                    switch (num)
                    {
                        #region Condition 0
                        case 0:
                            if (char.IsDigit(Arr[Count]))
                            {
                                temp += Arr[Count];
                                num = 2;
                                ++Count;
                                if (Count == Arr.Length)
                                {
                                    id = CheckNum(int.Parse(temp), numbersArray);
                                    if (check)
                                        Count -= temp.Length;
                                    temp = "";
                                    num = 0;
                                    done = true;
                                }
                            }
                            else if (char.IsLetterOrDigit(Arr[Count]))
                            {
                                temp += Arr[Count];
                                num = 1;
                                Count++;
                                if (Count == Arr.Length)
                                {
                                    id = CheckText(temp, worldsArray, keyWorlds);
                                    if (check)
                                        Count -= temp.Length;
                                    temp = "";
                                    num = 0;
                                    done = true;
                                }
                            }
                            else if ((Count < Arr.Length - 2) && doubleSeparatorsArray.Contains($"{Arr[Count]}{Arr[Count + 1]}"))
                            {
                                temp += Arr[Count];
                                temp += Arr[Count + 1];
                                num = 4;
                                Count += 2;
                            }
                            else if (separatorsArray.Contains(Arr[Count]))
                            {
                                temp += Arr[Count];
                                ++Count;
                                id = CheckSeparators(temp[0], separatorsArray);
                                if (check)
                                    Count -= temp.Length;
                                temp = "";
                                num = 0;
                                done = true;
                            }
                            else if (Arr[Count].Equals('\"'))
                            {
                                temp += Arr[Count];
                                num = 5;
                                ++Count;
                                if (Count == Arr.Length)
                                    Error("Scan", 0);
                            }
                            else if (symbolsArray.Contains(Arr[Count]))
                            {
                                num = 0;
                                ++Count;
                            }
                            else
                            {
                                temp += Arr[Count];
                                Error("Scan", 1);
                            }

                            break;
                        #endregion

                        #region Condition 1
                        case 1:
                            if (Char.IsLetterOrDigit(Arr[Count]))
                            {
                                temp += Arr[Count];
                                num = 1;
                                ++Count;
                                if (Count == Arr.Length)
                                {
                                    id = CheckText(temp, worldsArray, keyWorlds);
                                    if (check)
                                        Count -= temp.Length;
                                    temp = "";
                                    num = 0;
                                    done = true;
                                }
                            }
                            else
                            {
                                id = CheckText(temp, worldsArray, keyWorlds);
                                if (check)
                                    Count -= temp.Length;
                                temp = "";
                                num = 0;
                                done = true;
                            }
                            break;
                        #endregion
                        
                        #region Condition 2
                        case 2:
                            if (Char.IsDigit(Arr[Count]))
                            {
                                temp += Arr[Count];
                                num = 2;
                                ++Count;
                                if (Count == Arr.Length)
                                {
                                    id = CheckNum(int.Parse(temp), numbersArray);
                                    if (check)
                                        Count -= temp.Length;
                                    temp = "";
                                    num = 0;
                                    done = true;
                                }
                            }
                            else
                            {
                                id = CheckNum(int.Parse(temp), numbersArray);
                                if (check)
                                    Count -= temp.Length;
                                temp = "";
                                num = 0;
                                done = true;
                            }
                            break;
                        #endregion
                        
                        #region Condition 4
                        case 4:
                            if (temp.Equals("//"))
                            {
                                while (Count != Arr.Length - 1)
                                {
                                    if (Arr[Count] == '\n')
                                    {
                                        break;
                                    }
                                    temp += Arr[Count];
                                    Count++;
                                }
                                _dataVar.Add($";{temp}");
                            }
                            else if (temp.Equals("/*"))
                            {
                                while (Count != Arr.Length - 1)
                                {
                                    if (Arr[Count].Equals('*') && Arr[Count + 1].Equals('/'))
                                        break;
                                    temp += Arr[Count];
                                    Count++;
                                }
                                temp += Arr[Count];
                                temp += Arr[Count+1];
                                Count += 2;
                                _dataVar.Add($";{temp}");
                            }
                            else
                            {
                                id = CheckDoubleSeparators(temp, doubleSeparatorsArray);
                                if (check)
                                    Count -= temp.Length;
                                done = true;
                            }
                            temp = "";
                            num = 0;

                            break;
                        #endregion

                        #region Condition 5
                        case 5:
                            while (!Arr[Count].Equals('\"'))
                            {
                                temp += Arr[Count];
                                ++Count;
                            }
                            temp += Arr[Count];
                            id = CheckQuotesText(temp, quotesTextArray);
                            if (check)
                                Count -= temp.Length;
                            done = true;
                            ++Count;
                            num = 0;
                            temp = "";

                            break;
                        #endregion
                        default:
                            throw new Exception("Undefined state");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}!");
            }

            return id;
        }
        #endregion

        #region arithmetic

        private void E()
        {
            T();
            E1();
        }

        private void E1() //+-
        {
            var token = Scan(true);

            switch (token.A)
            {
                // +
                case 'R' when token.N == 0:
                {
                    token = Scan(false);
                    T();
                    _mainVar.Add($"pop bx");
                    _mainVar.Add($"pop ax");
                    _mainVar.Add($"add ax, bx");
                    _mainVar.Add($"push ax");


                    token = Scan(true);
                    if (!token.A.Equals('-'))
                    {
                        E1();
                    }
                   
                    break;
                }
                // -
                case 'R' when token.N == 1:
                {
                    token = Scan(false);
                    T();
                    _mainVar.Add($"pop bx");
                    _mainVar.Add($"pop ax");
                    _mainVar.Add($"sub ax, bx");
                    _mainVar.Add($"push ax");

                    token = Scan(true);
                    if (!token.A.Equals('-'))
                    {
                        E1();
                    }

                    break;
                }
                case 'R' when (token.N == 6  || token.N == 14) && OpenBrackets > 0:
                case '-':
                case 'R' when (token.N == 10 || token.N == 15 || token.N == 16 || token.N == 17):
                case 'D' when (token.N == 0 || token.N == 1 || token.N == 2 || token.N == 3):
                    break;
                default:
                    Error("Арифметическое выражение", 1);
                    break;
            }
        }

        private void T()
        {
            F();
            T1();
        }

        private void T1() // */
        {
            var token = Scan(true); // * || /
            switch (token.A)
            {
                // *
                case 'R' when token.N == 3:
                {
                    token = Scan(false);
                    F();
                    _mainVar.Add($"pop bx");
                    _mainVar.Add($"pop ax");
                    _mainVar.Add($"mul bx");
                    _mainVar.Add($"push ax");

                    token = Scan(true);
                    if (!token.A.Equals('-'))
                    {
                        T1();
                    }

                    break;
                }
                // /
                case 'R' when token.N == 4:
                {
                    token = Scan(false);
                    F();
                    _mainVar.Add($"pop bx");
                    _mainVar.Add($"pop ax");
                    _mainVar.Add($"div bx");
                    _mainVar.Add($"push ax");
                
                    token = Scan(true);
                    if (!token.A.Equals('-'))
                    {
                        T1();
                    }

                    break;
                }
                case 'R' when (token.N == 6 || token.N == 14) && OpenBrackets > 0:
                case 'R' when (token.N == 0 || token.N == 1 || token.N == 10 || 
                               token.N == 15 || token.N == 16 || token.N == 17):
                case '-':
                case 'D' when (token.N == 0 || token.N == 1 || token.N == 2 || token.N == 3):
                    break;
                default:
                    Error("Арифметическое выражение", 1);
                    break;
            }
        }

        private void F()
        {
            var var = Scan(false);
            switch (var.A)
            {
                // NUM
                case 'C':
                    _mainVar.Add($"mov ax, {numbersArray[var.N]}");
                    _mainVar.Add($"push ax");
                    break;
                // '(E)'
                case 'R' when var.N == 5:
                {
                    OpenBrackets++;
                    E();
                    var tmp = Scan(false);
                    OpenBrackets--;

                    if (!(tmp.A.Equals('R') && tmp.N == 6))
                        Error("Арифметическое выражение", 2);
               
                    break;
                }
                // VAR
                case 'I':
                {
                    if (!worldsArray.Count.Equals(AmountVar))
                    {
                        Error("Арифметическое выражение", 9);
                    }
                    if (!arrayOfVariables[var.N - 1].typeOfVar.Equals('i'))
                    {
                        Error("Арифметическое выражение", 8);
                    }

                    var tmp = Scan(true);
                    switch (tmp.A)
                    {
                        // '['
                        case 'R' when tmp.N == 13:
                        {
                            tmp = Scan(false);

                            if (!arrayOfVariables[var.N - 1].IsArray())
                            {
                                Error("Арифметическое выражение", 8);
                            }

                            OpenBrackets++;
                            E();
                            tmp = Scan(false);
                            OpenBrackets--;
                            if (!(tmp.A.Equals('R') && tmp.N == 14))
                                Error("Арифметическое выражение", 3);
                            
                            _mainVar.Add($"pop ax");
                            _mainVar.Add($"mov si, ax");
                            _mainVar.Add($"mov ax, {arrayOfVariables[var.N - 1].varName}[si]");
                            _mainVar.Add($"push ax");

                            break;
                        }
                        case 'R' when (tmp.N == 0 || tmp.N == 1 || tmp.N == 3 || tmp.N == 4 || tmp.N == 6 || 
                                       tmp.N == 10 || tmp.N == 14 || tmp.N == 15 || tmp.N == 16 || tmp.N == 17):
                        case '-':
                        case 'D' when (tmp.N == 0 || tmp.N == 1 || tmp.N == 2 || tmp.N == 3):
                        {
                            if(arrayOfVariables[var.N - 1].IsArray())
                            {
                                Error("Арифметическое выражение", 8);
                            }
                            
                            _mainVar.Add($"mov ax, {arrayOfVariables[var.N - 1].varName}");
                            _mainVar.Add($"push ax");
                            break;
                        }
                        default:
                            Error("Арифметическое выражение", 1);
                            break;
                    }
                    break;
                }
                default:
                    Error("Арифметическое выражение", 1);
                    break;
            }
        }
        #endregion

        #region logic
        private void El()
        {
            Tl();
            var tok = Scan(true);
            while (tok.A.Equals('K') && (tok.N == 0))
            {
                tok = Scan(false);
                Tl();
                _mainVar.Add("pop bx");
                _mainVar.Add("pop ax");
                _mainVar.Add("or ax, bx");
                _mainVar.Add("push ax");
                tok = Scan(true);
            }
        }

        private void Tl()
        {
            Fl();
            var tok = Scan(true);
            while (tok.A.Equals('K') && (tok.N == 1))
            {
                tok = Scan(false);
                Fl();
                _mainVar.Add("pop bx");
                _mainVar.Add("pop ax");
                _mainVar.Add("and ax, bx");
                _mainVar.Add("push ax");
                tok = Scan(true);
            }
        }

        private void Fl()
        {
            var var = Scan(false);
            switch (var.A)
            {
                // VAR
                case 'I':
                {
                    if (!AmountVar.Equals(worldsArray.Count))
                    {
                        Error("Арифметическое выражение", 9);
                    }
                    if (!arrayOfVariables[var.N - 1].typeOfVar.Equals('b'))
                    {
                        Error("Логическое выражение", 8);
                    }

                    var token = Scan(true);
                    switch (token.A)
                    {
                        // [
                        case 'R' when token.N == 13:
                        {
                            if (!arrayOfVariables[var.N - 1].IsArray())
                            {
                                Error("Логическое выражение", 9);
                            }
                            
                            token = Scan(false);
                            OpenBrackets++;
                            E();
                            token = Scan(false);
                            OpenBrackets--;
                            if (!(token.A.Equals('R') && token.N == 14))
                                Error("Логическое выражение", 3);
                            
                            _mainVar.Add($"pop ax");
                            _mainVar.Add($"mov si, ax");
                            _mainVar.Add($"mov al, {arrayOfVariables[var.N - 1].varName}[si]");
                            _mainVar.Add($"push ax");
                            
                            break;
                        }
                        case 'R' when (token.N == 0 || token.N == 1 || token.N == 10 ||
                                       token.N == 3 || token.N == 4 || token.N == 6 || token.N == 14 
                                       || token.N == 15 || token.N == 16 || token.N == 17):
                        case 'D' when (token.N == 0 || token.N == 1 || token.N == 2 || token.N == 3):
                        {
                            if (arrayOfVariables[var.N - 1].IsArray())
                            {
                                Error("Логическое выражение", 9);
                            }
                            
                            _mainVar.Add($"xor ax, ax");
                            _mainVar.Add($"mov al, {arrayOfVariables[var.N - 1].varName}");
                            _mainVar.Add($"push ax");
                            
                            break;
                        }
                        default:
                            Error("Логическое выражение", 1);
                            break;
                    }

                    break;
                }
                // (EL)
                case 'R' when var.N == 5:
                {
                    OpenBrackets++;
                    El();
                    Token token = Scan(false);
                    OpenBrackets--;
                    
                    if (!(token.A.Equals('R') && token.N == 6)) // )
                        Error("Логическое выражение", 2);
                    
                    break;
                }
                // [
                case 'R' when var.N == 13:
                {
                    OpenBrackets++;
                    E();
                    var sign = Zn();
                    E();
                    var token = Scan(false);
                    OpenBrackets--;
                    if (!(token.A.Equals('R') && token.N == 14))
                        Error("Логическое выражение", 3);

                    _mainVar.Add("pop bx");
                    _mainVar.Add("pop ax");
                    _mainVar.Add("cmp ax, bx");
                    _mainVar.Add($"{sign} log_{LabelCount}");
                    _mainVar.Add($"push 0");
                    _mainVar.Add($"jmp end_log_{LabelCount}");
                    _mainVar.Add($"log_{LabelCount}:");
                    _mainVar.Add($"push 1");
                    _mainVar.Add($"end_log_{LabelCount}:");
                    LabelCount++;

                    break;
                }
                // true
                case 'K' when var.N == 2:
                    _mainVar.Add("push 1");
                    break;
                // false
                case 'K' when var.N == 3:
                    _mainVar.Add("push 0");
                    break;
                // not
                case 'K' when var.N == 4:
                    Fl();
                    _mainVar.Add("pop ax");
                    _mainVar.Add("xor ax, 1");
                    _mainVar.Add("push ax");
                    break;
                default:
                    Error("Логическое выражение", 1);
                    break;
            }
        }

        private string Zn()
        {
            var tok = Scan(false);
            switch (tok.A)
            {
                // >
                case 'R' when (tok.N == 15):
                    return "jg";
                // <
                case 'R' when (tok.N == 16):
                    return "jl";
                // ==
                case 'D' when (tok.N == 0):
                    return "je";
                // != 
                case 'D' when (tok.N == 1):
                    return "jne";
                // >=
                case 'D' when (tok.N == 2):
                    return "jge";
                // <=
                case 'D' when (tok.N == 3):
                    return "jle";
                default:
                    Error("Логическое выражение", 4);
                    return null;
            }
        }
        #endregion

        #region programm

        public void Prog()
        {
            var tok = Scan(false); // program
            if (tok.A.Equals('K') && tok.N.Equals(11))
            {
                tok = Scan(false); // i
                if (!tok.A.Equals('I'))
                {
                    Error("Prog", 5);
                }

                tok = Scan(false); // ;
                if (!(tok.A.Equals('R') && tok.N.Equals(10)))
                {
                    Error("Prog", 6);
                }


                tok = Scan(true); // declare
                if (tok.A.Equals('K') && tok.N.Equals(10))
                {
                    DecalreBlock();
                }

                tok = Scan(false); // main
                if (!(tok.A.Equals('K') && tok.N.Equals(12)))
                {
                    Error("Prog", 7);
                }

                OperatorsBlock();

                tok = Scan(false); // end
                if (!(tok.A.Equals('K') && tok.N.Equals(13)))
                {
                    Error("Prog", 7);
                }
                tok = Scan(false); // .
                if (!(tok.A.Equals('R') && tok.N.Equals(12)))
                {
                    Error("Prog", 7);
                }
            }
            else
            {
                Error("Prog", 7);
            }
        }

        private void DecalreBlock()
        {
            var tok = Scan(false); //declare
            if (tok.A.Equals('K') && tok.N.Equals(10))
            {
                tok = Scan(true);
                if (tok.A.Equals('I'))
                {
                    var Temp_List = new List<string>();
                    while (tok.A.Equals('I'))
                    {
                        tok = Scan(false); // I
                        Temp_List.Add(worldsArray[tok.N]);
                        tok = Scan(true); // ,
                        while (tok.A.Equals('R') && tok.N.Equals(17))
                        {
                            tok = Scan(false); // ,
                            tok = Scan(false); // I
                            if (tok.A.Equals('I'))
                            {
                                Temp_List.Add(worldsArray[tok.N]);
                            }
                            else
                            {
                                Error("DeclareBlock", 7);
                            }
                            tok = Scan(true);
                        }

                        tok = Scan(false); // :
                        if (!(tok.A.Equals('R') && tok.N.Equals(9)))
                        {
                            Error("DeclareBlock", 7);
                        }

                        Type(Temp_List);

                        tok = Scan(false); // ;
                        if (!(tok.A.Equals('R') && tok.N.Equals(10)))
                        {
                            Error("DeclareBlock", 7);
                        }

                        tok = Scan(true);
                    }
                }
                else
                {
                    Error("DeclareBlock", 5);
                }
                AmountVar = worldsArray.Count;
            }
            else
            {
                Error("DeclareBlock", 7);
            }
        }

        private void Type(List<String> vars)
        {
            Token tok = Scan(true);
            if (tok.A.Equals('K') && tok.N.Equals(5))
            {
                Array(vars);
            }
            else
            {
                string type = BaseType();
                foreach (var variable in vars)
                {
                    if (type.Equals("char") || type.Equals("bool"))
                    {
                        _dataVar.Add($"{variable} db (?)");
                    }
                    else
                    {
                        _dataVar.Add($"{variable} dw (?)");
                    }
                    arrayOfVariables.Add(new Variable(variable, type[0]));
                }
                vars.Clear();
            }
        }

        private string BaseType()
        {
            Token tok = Scan(false);
            if (tok.A.Equals('K') && tok.N.Equals(7)) // int
            {
                return "int";
            }
            else if (tok.A.Equals('K') && tok.N.Equals(8)) //char
            {
                return "char";
            }
            else if (tok.A.Equals('K') && tok.N.Equals(9)) //bool
            {
                return "bool";
            }
            else
            {
                Error("BaseType", 8);
                return null;
            }
        }

        private void Array(List<string> vars)
        {
            int size = 0;
            Token tok = Scan(false); // array
            if (tok.A.Equals('K') && tok.N.Equals(5))
            {
                tok = Scan(false); // [
                if (!(tok.A.Equals('R') && tok.N.Equals(13)))
                {
                    Error("Array", 7);
                }
                tok = Scan(false); // C
                if (tok.A.Equals('C'))
                {
                    size = numbersArray[tok.N];
                }
                else
                {
                    Error("Array", 7);
                }

                tok = Scan(false); // ]
                if (!(tok.A.Equals('R') && tok.N.Equals(14)))
                {
                    Error("Array", 7);
                }

                tok = Scan(false); // of
                if (!(tok.A.Equals('K') && tok.N.Equals(6)))
                {
                    Error("Array", 7);
                }

                string type = BaseType();
                string temp = type.Equals("int") ? "dw" : "db";
                foreach (var variable in vars)
                {
                    _dataVar.Add($"{variable} {temp} {size} DUP (?)");
                    arrayOfVariables.Add(new Variable(variable, type[0], true, size));
                }
                vars.Clear();            
            }
            else
            {
                Error("Array", 7);
            }
        }

        private void OperatorsBlock()
        {
            Token tok = Scan(false); // {
            if (tok.A.Equals('R') && tok.N.Equals(7))
            {
                tok = Scan(true); // { || let || I || if || while || read || write || case
                while (tok.A.Equals('R') && tok.N.Equals(7) || (tok.A.Equals('K') && (tok.N.Equals(14) || tok.N.Equals(15) || tok.N.Equals(16) || tok.N.Equals(20) || tok.N.Equals(21) || tok.N.Equals(23))) || tok.A.Equals('I'))
                {
                    Operator();

                    tok = Scan(true);
                }

                tok = Scan(false); // }
                if (!(tok.A.Equals('R') && tok.N.Equals(8)))
                {
                    Error("OperatorsBlock", 7);
                }
            }
            else
            {
                Error("OperatorsBlock", 7);
            }
        }

        private void Operator()
        {
            Token tok = Scan(true);
            if (tok.A.Equals('K') && tok.N.Equals(14) || tok.A.Equals('I')) // let, I
            {
                Assignment();
                tok = Scan(false); // ;
                if (tok.A.Equals('R') && tok.N.Equals(10))
                { }
                else
                    Error("Operator", 6);
            }
            else if (tok.A.Equals('R') && tok.N.Equals(7)) // {
            {
                OperatorsBlock();
            }
            else if (tok.A.Equals('K') && tok.N.Equals(15)) // if
            {
                Condition();
            }
            else if (tok.A.Equals('K') && tok.N.Equals(16)) // while
            {
                CycleWhile();
            }
            else if (tok.A.Equals('K') && tok.N.Equals(23)) // case
            {
                CasePascal();
            }
            else if (tok.A.Equals('K') && tok.N.Equals(20)) // read
            {
                Read();
                tok = Scan(false); // ;
                if (tok.A.Equals('R') && tok.N.Equals(10))
                { }
                else
                    Error("Operator", 6);
            }
            else if (tok.A.Equals('K') && tok.N.Equals(21)) // write
            {
                Write();
                tok = Scan(false); // ;
                if (tok.A.Equals('R') && tok.N.Equals(10))
                { }
                else
                    Error("Operator", 6);
            }
        }

        private void Assignment()
        {
            Token tok = Scan(true); // let
            if (tok.A.Equals('K') && tok.N.Equals(14))
            {
                tok = Scan(false);
            }

            Token var = Scan(false);  // I
            if (var.A.Equals('I'))
            {
                if (worldsArray.Count > AmountVar)
                {
                    Error("Assignment", 9);
                }
            }
            else
            {
                Error("Assignment", 5);
            }


            tok = Scan(true);
            if (tok.A.Equals('R') && tok.N.Equals(13)) // [
            {
                if (!arrayOfVariables[var.N - 1].IsArray())
                {
                    Error("Assignment", 8);
                }

                tok = Scan(false);
                OpenBrackets++;
                E(); // stack si
                tok = Scan(false);
                OpenBrackets--;
                if (!(tok.A.Equals('R') && tok.N == 14))
                    Error("Assignment", 3);
            }
            //////////////////////////////////////

            tok = Scan(false); // =
            if (tok.A.Equals('R') && tok.N.Equals(2))
            {

            }
            else
            {
                Error("Assignment", 7);
            }

            tok = Scan(true);
            switch (arrayOfVariables[var.N - 1].typeOfVar)
            {
                case 'c':
                    {
                        if(arrayOfVariables[var.N-1].IsArray()) // if Array
                        {
                            if (tok.A.Equals('L')) // L
                            {
                                tok = Scan(false);
                                _mainVar.Add($"xor ax, ax");
                                _mainVar.Add($"mov al, '{quotesTextArray[tok.N][1]}'");
                                _mainVar.Add($"pop bx");
                                _mainVar.Add($"mov si, bx");
                                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}[si], al");
                            }
                            else if (tok.A.Equals('I') && arrayOfVariables[tok.N - 1].typeOfVar.Equals('c')) // I(char)
                            {
                                Token right_var = Scan(false); 
                                if (worldsArray.Count > AmountVar) // if not char
                                {
                                    Error("Assignment", 9);
                                }

                                tok = Scan(true); // [
                                if (tok.A.Equals('R') && tok.N.Equals(13)) // = I[E]
                                {
                                    if (!arrayOfVariables[right_var.N - 1].IsArray())
                                    {
                                        Error("Assignment", 8);
                                    }
                                    tok = Scan(false);
                                    OpenBrackets++;
                                    E();
                                    tok = Scan(false);
                                    OpenBrackets--;
                                    if (!(tok.A.Equals('R') && tok.N == 14))
                                        Error("Assignment", 3);
                                    _mainVar.Add($"xor ax, ax");
                                    _mainVar.Add("pop bx");
                                    _mainVar.Add("mov si, bx");
                                    _mainVar.Add($"mov al, {arrayOfVariables[right_var.N-1].varName}[si]");
                                }
                                else // = I 
                                {
                                    if (arrayOfVariables[right_var.N - 1].IsArray())
                                    {
                                        Error("Assignment", 8);
                                    }
                                    _mainVar.Add($"xor ax, ax");
                                    _mainVar.Add($"mov al, {arrayOfVariables[right_var.N - 1].varName}");
                                }
                                _mainVar.Add($"pop bx");
                                _mainVar.Add($"mov si, bx");
                                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}[si], al");
                            }
                            else
                            {
                                Error("Assignment", 8);
                            }
                        }
                        else // if not Array
                        {
                            Token right_var = Scan(true);
                            if (tok.A.Equals('L')) // L
                            {
                                tok = Scan(false);
                                _mainVar.Add($"xor ax, ax");
                                _mainVar.Add($"mov al, '{quotesTextArray[tok.N][1]}'");
                                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}, al");
                            }
                            else if (tok.A.Equals('I') && arrayOfVariables[tok.N - 1].typeOfVar.Equals('c')) // I(char)
                            {
                                tok = Scan(false);
                                if (worldsArray.Count > AmountVar)
                                {
                                    Error("Assignment", 9);
                                }

                                tok = Scan(true);
                                if (tok.A.Equals('R') && tok.N.Equals(13)) // = I[E]
                                {
                                    if (!arrayOfVariables[right_var.N - 1].IsArray())
                                    {
                                        Error("Assignment", 8);
                                    }
                                    tok = Scan(false);
                                    OpenBrackets++;
                                    E();
                                    tok = Scan(false);
                                    OpenBrackets--;
                                    if (!(tok.A.Equals('R') && tok.N == 14))
                                        Error("Assignment", 3);
                                    _mainVar.Add($"xor ax, ax");
                                    _mainVar.Add("pop bx");
                                    _mainVar.Add("mov si, bx");
                                    _mainVar.Add($"mov al, {arrayOfVariables[right_var.N - 1].varName}[si]");
                                }
                                else
                                {
                                    if (arrayOfVariables[right_var.N - 1].IsArray())
                                    {
                                        Error("Assignment", 8);
                                    }
                                    _mainVar.Add($"xor ax, ax");
                                    _mainVar.Add($"mov al, {arrayOfVariables[right_var.N - 1].varName}");
                                }
                                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}, al");
                            }
                            else
                            {
                                Error("Assignment", 8);
                            }
                        }
                            break;
                    }
                case 'i':
                    {
                        if (arrayOfVariables[var.N - 1].IsArray()) // if array
                        {
                                E();
                                _mainVar.Add("pop ax");
                                _mainVar.Add($"pop bx");
                                _mainVar.Add($"mov si, bx");
                                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}[si], ax");
                           // }
                        }
                        else // if not array
                        {
                                E();
                                _mainVar.Add("pop ax");
                                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}, ax");
                        }
                        break;
                    }
                case 'b':
                    {
                        if (arrayOfVariables[var.N - 1].IsArray()) // if array
                        {

                            if (tok.A.Equals('I') && arrayOfVariables[tok.N - 1].typeOfVar.Equals('b')) // I(bool)
                            {
                                Token right_var = Scan(false);
                                if (worldsArray.Count > AmountVar)
                                {
                                    Error("Assignment", 9);
                                }

                                tok = Scan(true);
                                if (tok.A.Equals('R') && tok.N.Equals(13)) // = I[E]
                                {
                                    if (!arrayOfVariables[right_var.N - 1].IsArray())
                                    {
                                        Error("Assignment", 8);
                                    }
                                    tok = Scan(false);
                                    OpenBrackets++;
                                    E();
                                    tok = Scan(false);
                                    OpenBrackets--;
                                    if (!(tok.A.Equals('R') && tok.N == 14))
                                        Error("Assignment", 3);
                                    _mainVar.Add($"xor ax, ax");
                                    _mainVar.Add("pop bx");
                                    _mainVar.Add("mov si, bx");
                                    _mainVar.Add($"mov al, {arrayOfVariables[right_var.N - 1].varName}[si]");
                                }
                                else
                                {
                                    if (arrayOfVariables[right_var.N - 1].IsArray())
                                    {
                                        Error("Assignment", 8);
                                    }
                                    _mainVar.Add($"xor ax, ax");
                                    _mainVar.Add($"mov al, {arrayOfVariables[right_var.N - 1].varName}");
                                }
                                _mainVar.Add($"pop bx");
                                _mainVar.Add($"mov si, bx");
                                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}[si], al");
                            }
                            else // El
                            {
                                El();
                                _mainVar.Add("pop ax");
                                _mainVar.Add($"pop bx");
                                _mainVar.Add($"mov si, bx");
                                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}[si], al");
                            }
                        }
                        else //if not array
                        {
                            if (tok.A.Equals('I') && arrayOfVariables[tok.N - 1].typeOfVar.Equals('b')) // I(bool)
                            {
                                Token right_var = Scan(false);
                                if (worldsArray.Count > AmountVar)
                                {
                                    Error("Assignment", 9);
                                }

                                tok = Scan(true);
                                if (tok.A.Equals('R') && tok.N.Equals(13)) // = I[E]
                                {
                                    if (!arrayOfVariables[right_var.N - 1].IsArray())
                                    {
                                        Error("Assignment", 8);
                                    }
                                    tok = Scan(false);
                                    OpenBrackets++;
                                    E();
                                    tok = Scan(false);
                                    OpenBrackets--;
                                    if (!(tok.A.Equals('R') && tok.N == 14))
                                        Error("Assignment", 3);
                                    _mainVar.Add($"xor ax, ax");
                                    _mainVar.Add("pop bx");
                                    _mainVar.Add("mov si, bx");
                                    _mainVar.Add($"mov al, {arrayOfVariables[right_var.N - 1].varName}[si]");
                                }
                                else 
                                {
                                    if (arrayOfVariables[right_var.N - 1].IsArray())
                                    {
                                        Error("Assignment", 8);
                                    }
                                    _mainVar.Add($"xor ax, ax");
                                    _mainVar.Add($"mov al, {arrayOfVariables[right_var.N - 1].varName}");
                                }
                                    _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}, al");
                            }
                            else // El
                            {
                                El();
                                _mainVar.Add("pop ax");
                                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}, al");
                            } 
                        }
                        break;
                    }
                default:
                    Error("Assignment", 1);
                    break;
            }
        }

        private void Condition()
        {
            Token tok = Scan(false); // if
            int count = LabelCount;
            LabelCount++;
            tok = Scan(false);
            if (tok.A.Equals('R') && tok.N.Equals(5)) // (
            {
                OpenBrackets++;
                El();
                _mainVar.Add("pop ax");
                _mainVar.Add("cmp ax, 1");
                _mainVar.Add($"jne else_{count}");
                tok = Scan(false);
                OpenBrackets--;
                if (!(tok.A.Equals('R') && tok.N.Equals(6)))
                {
                    Error("Condition", 2);
                }
            }
            else
            {
                Error("Condition", 7);
            }

            tok = Scan(false);
            if (!(tok.A.Equals('K') && tok.N.Equals(17))) //  then
            {
                Error("Condition", 7);
            }

            Operator();
            _mainVar.Add($"jmp end_if_{count}");

            tok = Scan(true);
            _mainVar.Add($"else_{count}:");
            if (tok.A.Equals('K') && tok.N.Equals(18)) //  else
            {
                tok = Scan(false);
                Operator();
                //main_var.Add($"jmp end_if_{count}");
            }
            _mainVar.Add($"end_if_{count}:");
        }

        private void CycleWhile()
        {
            Token tok = Scan(false); // while

            tok = Scan(false); // (
            int count = LabelCount;
            LabelCount++;
            if (tok.A.Equals('R') && tok.N.Equals(5))
            {
                OpenBrackets++;
                _mainVar.Add($"start_while_{count}:");
                El();
                _mainVar.Add("pop ax");
                _mainVar.Add("cmp ax, 1");
                _mainVar.Add($"jne end_while_{count}");
                tok = Scan(false);
                OpenBrackets--;
                if (!(tok.A.Equals('R') && tok.N.Equals(6)))
                    Error("CycleWhile", 2);
            }
            else
            {
                Error("CycleWhile", 7);
            }

            tok = Scan(false); // do
            if (tok.A.Equals('K') && tok.N.Equals(19))
            {
                Operator();
            }
            else
            {
                Error("CycleWhile", 7);
            }
            _mainVar.Add($"jmp start_while_{count}");
            _mainVar.Add($"end_while_{count}:");
        }

        private void CasePascal()
        {
            List<int> arr = new List<int>(); 
            List<char> arr_c = new List<char>(); 
            List<Token> arr_b = new List<Token>(); 
            int count = LabelCount;
            LabelCount++;
            Token tok = Scan(false); // case
            Token var = Scan(false); // var
            if (worldsArray.Count > AmountVar) // if I not exist
            {
                Error("Case condition", 9);
            }

            tok = Scan(true); // case
            if (tok.A.Equals('R') && tok.N.Equals(13)) // [
            {
                if (!arrayOfVariables[var.N - 1].IsArray())
                {
                    Error("Case Condition", 8);
                }
                tok = Scan(false);
                OpenBrackets++;
                E();
                _mainVar.Add("pop bx");/////////////
                _mainVar.Add("push bx");/////////////
                _mainVar.Add($"mov ax, {arrayOfVariables[var.N - 1].Get_Count()}"); ////
                _mainVar.Add("cmp ax, bx"); ///////////
                _mainVar.Add($"jae eeeennnndddd_1_{count}"); ///////////
                _mainVar.Add($"jmp eeeennnndddd_{count}"); ///////////
                _mainVar.Add($"eeeennnndddd_1_{count}:"); ///////////
                tok = Scan(false);
                OpenBrackets--;
                if (!(tok.A.Equals('R') && tok.N == 14))
                {
                    Error("Case Condition", 3);
                }
            }
            else if (arrayOfVariables[var.N - 1].IsArray())
            {
                Error("Case Condition", 8);
            }

            tok = Scan(false); // of
            if (!(tok.A.Equals('K') && tok.N.Equals(6)))
            {
                Error("Case condition", 7);
            }
            
            int i = 1; // счетчик случаев 
            switch (arrayOfVariables[var.N-1].typeOfVar)
            {
                case 'i':
                    {
                        if (!arrayOfVariables[var.N - 1].IsArray())
                        {
                            _mainVar.Add($"mov ax, {arrayOfVariables[var.N - 1].varName}"); // помещаем сравниваемое значение в стек
                        }
                        else 
                        {
                            _mainVar.Add("pop di");
                            _mainVar.Add($"mov ax, {arrayOfVariables[var.N - 1].varName}[di]"); // помещаем сравниваемое значение в стек
                        }
                        _mainVar.Add($"push ax");
                        tok = Scan(true);
                        if (!tok.A.Equals('C'))
                        {
                            Error("Case condition", 8);
                        }

                        while (tok.A.Equals('C'))
                        {
                            tok = Scan(false); // C
                            if (!arr.Contains(numbersArray[tok.N]))
                            {
                                arr.Add(numbersArray[tok.N]);
                            }
                            else
                            {
                                Error("Case condition", 7);
                            }
                            _mainVar.Add($"pop ax");
                            _mainVar.Add($"mov bx, {numbersArray[tok.N]}");
                            _mainVar.Add($"push ax");
                            _mainVar.Add($"cmp ax, bx");
                            _mainVar.Add($"je case_{i}_{count}");
                           // main_var.Add($"jne case_{i}_{count}_end");
                            
                            tok = Scan(true); //,
                            while (tok.A.Equals('R') && tok.N.Equals(17))
                            {
                                _ = Scan(false); // ,
                                tok = Scan(false); // C
                                if (!tok.A.Equals('C'))
                                {
                                    Error("Case condition", 8);
                                }
                                if (!arr.Contains(numbersArray[tok.N]))
                                {
                                    arr.Add(numbersArray[tok.N]);
                                }
                                else
                                {
                                    Error("Case condition", 7);
                                }
                                _mainVar.Add($"pop ax");
                                _mainVar.Add($"mov bx, {numbersArray[tok.N]}");
                                _mainVar.Add($"push ax");
                                _mainVar.Add($"cmp ax, bx");
                                _mainVar.Add($"je case_{i}_{count}");
                                tok = Scan(true);
                            }
                            _mainVar.Add($"jmp case_{i}_{count}_end");

                                tok = Scan(false);
                            if (!(tok.A.Equals('R') && tok.N.Equals(9))) // :
                            {
                                Error("Case condition", 7);
                            }

                            _mainVar.Add($"case_{i}_{count}:");
                            Operator();
                            _mainVar.Add($"jmp case_{count}_end");
                            _mainVar.Add($"case_{i}_{count}_end:");
                            i++;
                            tok = Scan(true);
                        }
                        break;
                    }
                case 'c':
                    {
                        _mainVar.Add($"xor ax, ax"); // помещаем сравниваемое значение в стек
                        if (!arrayOfVariables[var.N - 1].IsArray())
                        {
                            _mainVar.Add($"mov al, {arrayOfVariables[var.N - 1].varName}"); // помещаем сравниваемое значение в стек
                        }
                        else
                        {
                            _mainVar.Add("pop di");
                            _mainVar.Add($"mov al, {arrayOfVariables[var.N - 1].varName}[di]"); // помещаем сравниваемое значение в стек
                        }
                        _mainVar.Add($"push ax");
                        tok = Scan(true);
                        if (!tok.A.Equals('L'))
                        {
                            Error("Case condition", 8);
                        }

                        while (tok.A.Equals('L'))
                        {
                            tok = Scan(false);
                            if (!arr_c.Contains(quotesTextArray[tok.N][1]))
                            {
                                arr_c.Add(quotesTextArray[tok.N][1]);
                            }
                            else
                            {
                                Error("Case condition", 7);
                            }
                            _mainVar.Add($"xor ax, ax");
                            _mainVar.Add($"pop ax");
                            _mainVar.Add($"xor bx, bx");
                            _mainVar.Add($"mov bl, '{quotesTextArray[tok.N][1]}'");
                            _mainVar.Add($"push ax");
                            _mainVar.Add($"cmp al, bl");
                            _mainVar.Add($"je case_{i}_{count}");
                            //main_var.Add($"jmp case_{i}_{count}_end");

                            tok = Scan(true); //,
                            while (tok.A.Equals('R') && tok.N.Equals(17))
                            {
                                _ = Scan(false); // ,
                                tok = Scan(false); // C
                                if (!tok.A.Equals('L'))
                                {
                                    Error("Case condition", 8);
                                }
                                if (!arr_c.Contains(quotesTextArray[tok.N][1]))
                                {
                                    arr_c.Add(quotesTextArray[tok.N][1]);
                                }
                                else
                                {
                                    Error("Case condition", 7);
                                }
                                _mainVar.Add($"xor ax, ax");
                                _mainVar.Add($"pop ax");
                                _mainVar.Add($"xor bx, bx");
                                _mainVar.Add($"mov bl, '{quotesTextArray[tok.N][1]}'");
                                _mainVar.Add($"push ax");
                                _mainVar.Add($"cmp al, bl");
                                _mainVar.Add($"je case_{i}_{count}");
                                tok = Scan(true);
                            }
                            _mainVar.Add($"jmp case_{i}_{count}_end");


                            tok = Scan(false);
                            if (!(tok.A.Equals('R') && tok.N.Equals(9))) // :
                            {
                                Error("Case condition", 7);
                            }

                            _mainVar.Add($"case_{i}_{count}:");
                            Operator();
                            _mainVar.Add($"jmp case_{count}_end");
                            _mainVar.Add($"case_{i}_{count}_end:");
                            i++;
                            tok = Scan(true);
                        }
                        break;
                    }
                case 'b':
                    {
                        _mainVar.Add($"xor ax, ax"); // помещаем сравниваемое значение в стек
                        if (!arrayOfVariables[var.N - 1].IsArray())
                        {
                            _mainVar.Add($"mov al, {arrayOfVariables[var.N - 1].varName}"); // помещаем сравниваемое значение в стек
                        }
                        else
                        {
                            _mainVar.Add("pop di");
                            _mainVar.Add($"mov al, {arrayOfVariables[var.N - 1].varName}[di]"); // помещаем сравниваемое значение в стек
                        }
                        _mainVar.Add($"push ax");

                        tok = Scan(true);
                        if (!(tok.A.Equals('K') && (tok.N.Equals(2) || tok.N.Equals(3))))
                        {
                            Error("Case condition", 8);
                        }
                        arr_b.Add(tok);

                        while (tok.A.Equals('K') && (tok.N.Equals(2) || tok.N.Equals(3)))
                        {
                            //if (!arr_b.Contains(tok))
                            //{
                            //    arr_b.Add(tok);
                            //}
                            //else 
                            //{
                            //    Error("Case condition", 7);
                            //}
                            tok = Scan(false);
                            _mainVar.Add($"xor ax, ax");
                            _mainVar.Add($"pop ax");
                            if (tok.N.Equals(2)) // true
                            {
                                _mainVar.Add($"mov bx, 1");
                            }
                            else
                            {
                                _mainVar.Add($"mov bx, 0");
                            }
                            _mainVar.Add($"push ax");
                            _mainVar.Add($"cmp al, bl");
                            _mainVar.Add($"jne case_{i}_{count}_end");

                            tok = Scan(false);
                            if (!(tok.A.Equals('R') && tok.N.Equals(9))) // :
                            {
                                Error("Case condition", 7);
                            }

                            _mainVar.Add($"case_{i}_{count}:");
                            Operator();
                            _mainVar.Add($"jmp case_{count}_end");
                            _mainVar.Add($"case_{i}_{count}_end:");
                            i++;
                            tok = Scan(true);
                        }

                            break;
                    }
                default:
                    {
                        Error("Case condition", 1);
                        break;
                    }
            }
            tok = Scan(true); // else
            if (tok.A.Equals('K') && tok.N.Equals(18))
            {
            _mainVar.Add($"case_{count}_else:");
                tok = Scan(false);
                Operator();
            }
            _mainVar.Add($"jmp case_{count}_end"); 

            _mainVar.Add($"eeeennnndddd_{count}:"); ////////////
            _dataVar.Add($"var_{count} db \"CASE_ERROR\", 0Dh, 0Ah, \"$\"");
            _mainVar.Add($"lea dx, var_{count}");
            _mainVar.Add("mov ah, 9");
            _mainVar.Add("int 21h");

            _mainVar.Add($"case_{count}_end:");

            tok = Scan(false); // end
            if (!(tok.A.Equals('K') && tok.N.Equals(13)))
            {
                Error("Case condition", 7);
            }
            tok = Scan(false); // ;
            if (!(tok.A.Equals('R') && tok.N.Equals(10)))
            {
                Error("Case condition", 7);
            }
        }

        #region read
        private void Read()
        {
            Token tok = Scan(false); // read

            tok = Scan(false); // (
            if (tok.A.Equals('R') && tok.N.Equals(5))
            {
                OpenBrackets++;
                var var = Scan(false);  // I
                if (worldsArray.Count > AmountVar)
                {
                    Error("Read", 9);
                }

                if (var.A.Equals('I'))
                {
                    switch (arrayOfVariables[var.N-1].typeOfVar)
                    {
                        case 'b':
                            {
                                Read_Bool(var);
                                break;
                            }
                        case 'c':
                            {
                                Read_Char(var);
                                break;
                            }
                        case 'i':
                            {
                                Read_Int(var);
                                break;
                            }
                        default:
                            break;
                    }
                }
                else 
                {
                    Error("Read", 7);
                }

                tok = Scan(true); // ,
                while (tok.A.Equals('R') && tok.N.Equals(17))
                {
                    tok = Scan(false); // , 
                    var = Scan(false); // I
                    if (worldsArray.Count > AmountVar)
                    {
                        Error("Read", 9);
                    }

                    if (var.A.Equals('I'))
                    {
                        switch (arrayOfVariables[var.N - 1].typeOfVar)
                        {
                            case 'b':
                                {
                                    Read_Bool(var);
                                    break;
                                }
                            case 'c':
                                {
                                    Read_Char(var);
                                    break;
                                }
                            case 'i':
                                {
                                    Read_Int(var);
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Error("Read", 7);
                    }

                    tok = Scan(true);
                }

                tok = Scan(false); // )
                OpenBrackets--;
                if (!(tok.A.Equals('R') && tok.N.Equals(6)))
                    Error("Read", 2);
            }
            else
            {
                Error("Read", 7);
            }
        }

        private void Read_Bool(Token var)
        {
            Token tok = Scan(true); // [
            if (tok.A.Equals('R') && tok.N.Equals(13))
            {
                if (!arrayOfVariables[var.N - 1].IsArray())
                {
                    Error("Read", 8);
                }
                tok = Scan(false);
                OpenBrackets++;
                E();
                tok = Scan(false);
                OpenBrackets--;
                if (!(tok.A.Equals('R') && tok.N == 14))
                {
                    Error("Read", 3);
                }
            }
            else if (arrayOfVariables[var.N - 1].IsArray())
            {
                Error("Read", 8);
            }

            int count = LabelCount;
            LabelCount++;

            _mainVar.Add($"input_bool_start_{count}:");
            _mainVar.Add("mov ah, 0Ah");
            _mainVar.Add("lea dx, @buffer");
            _mainVar.Add("int 21h");

            _mainVar.Add("cmp blength, 4");
            _mainVar.Add($"je l4_{count}");
            _mainVar.Add("cmp blength, 5");
            _mainVar.Add($"je l5_{count}");
            _mainVar.Add($"jmp lerror_{count}");

            _mainVar.Add($"l4_{count}:");
            _mainVar.Add("cmp @buf[0], \"t\"");
            _mainVar.Add($"jne lerror_{count}");
            _mainVar.Add("cmp @buf[1], \"r\"");
            _mainVar.Add($"jne lerror_{count}");
            _mainVar.Add("cmp @buf[2], \"u\"");
            _mainVar.Add($"jne lerror_{count}");
            _mainVar.Add("cmp @buf[3], \"e\"");
            _mainVar.Add($"jne lerror_{count}");
            _mainVar.Add("push 1");
            _mainVar.Add($"jmp input_bool_end_{count}");

            _mainVar.Add($"l5_{count}:");
            _mainVar.Add("cmp @buf[0], \"f\"");
            _mainVar.Add($"jne lerror_{count}");
            _mainVar.Add("cmp @buf[1], \"a\"");
            _mainVar.Add($"jne lerror_{count}");
            _mainVar.Add("cmp @buf[2], \"l\"");
            _mainVar.Add($"jne lerror_{count}");
            _mainVar.Add("cmp @buf[3], \"s\"");
            _mainVar.Add($"jne lerror_{count}");
            _mainVar.Add("cmp @buf[4], \"e\"");
            _mainVar.Add($"jne lerror_{count}");
            _mainVar.Add("push 0");
            _mainVar.Add($"jmp input_bool_end_{count}");

            _mainVar.Add($"lerror_{count}:");
            _mainVar.Add("lea dx, err_msg");
            _mainVar.Add("mov ah, 9");
            _mainVar.Add("int 21h");
            _mainVar.Add($"jmp input_bool_start_{count}");

            _mainVar.Add($"input_bool_end_{count}:");
            _mainVar.Add("pop ax");
            if (!arrayOfVariables[var.N - 1].IsArray())
            {
                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}, al");
            }
            else
            {
                _mainVar.Add("pop di");
                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}[di], al");
            }
        }

        private void Read_Char(Token var)
        {
            var tok = Scan(true); // [
            if (tok.A.Equals('R') && tok.N.Equals(13))
            {
                if (!arrayOfVariables[var.N - 1].IsArray())
                {
                    Error("Read", 8);
                }
                tok = Scan(false);
                OpenBrackets++;
                E();
                tok = Scan(false);
                OpenBrackets--;
                if (!(tok.A.Equals('R') && tok.N == 14))
                {
                    Error("Read", 3);
                }

            }
            else if (arrayOfVariables[var.N - 1].IsArray())
            {
                Error("Read", 8);
            }

            _mainVar.Add("mov ah, 0Ah");
            _mainVar.Add("lea dx, @buffer");
            _mainVar.Add("int 21h");
            _mainVar.Add("xor dx,dx");
            _mainVar.Add("mov dl, @buf[0]");

            if (!arrayOfVariables[var.N - 1].IsArray())
            {
                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}, dl");
            }
            else
            {
                _mainVar.Add("pop di");
                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}[di], dl");
            }
        }

        private void Read_Int(Token var)
        {
            var tok = Scan(true); // [
            if (tok.A.Equals('R') && tok.N.Equals(13))
            {
                if (!arrayOfVariables[var.N - 1].IsArray())
                {
                    Error("Read", 8);
                }
                tok = Scan(false);
                OpenBrackets++;
                E();
                tok = Scan(false);
                OpenBrackets--;
                if (!(tok.A.Equals('R') && tok.N == 14))
                {
                    Error("Read", 3);
                }
            }
            else if (arrayOfVariables[var.N - 1].IsArray())
            {
                Error("Read", 8);
            }

            int count = LabelCount;
            LabelCount++;

            _mainVar.Add($"start_input_int_{count}:");
            _mainVar.Add("mov ah, 0Ah");
            _mainVar.Add("lea dx, @buffer");
            _mainVar.Add("int 21h");
            _mainVar.Add("mov ax, 0");
            _mainVar.Add("mov cx, 0");
            _mainVar.Add("mov cl, byte ptr[blength]");
            _mainVar.Add("mov bx, cx");
            _mainVar.Add($"int_s1_{count}:");
            _mainVar.Add("dec bx");
            _mainVar.Add("mov al, @buf[bx]");
            _mainVar.Add("cmp al, \"9\"");
            _mainVar.Add($"ja lerror_{count}");
            _mainVar.Add("cmp al, \"0\"");
            _mainVar.Add($"jb lerror_{count}");
            _mainVar.Add($"loop int_s1_{count}");
            _mainVar.Add("mov cl, byte ptr[blength]");
            _mainVar.Add("mov di, 0");
            _mainVar.Add("mov ax, 0");
            _mainVar.Add($"int_s2_{count}:");
            _mainVar.Add("mov bl, @buf[di]");
            _mainVar.Add("inc di");
            _mainVar.Add("sub bl, 30h");
            _mainVar.Add("add ax, bx");
            _mainVar.Add("mov si, ax");
            _mainVar.Add("mov bx, 10");
            _mainVar.Add("mul bx");
            _mainVar.Add($"loop int_s2_{count}");
            _mainVar.Add("mov ax, si");
            if (!arrayOfVariables[var.N - 1].IsArray())
            {
                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}, ax");
            }
            else
            {
                _mainVar.Add("pop di");
                _mainVar.Add($"mov {arrayOfVariables[var.N - 1].varName}[di], ax");
            }
            _mainVar.Add($"jmp end_input_int_{count}");
            _mainVar.Add($"lerror_{count}:");
            _mainVar.Add("lea dx, err_msg");
            _mainVar.Add("mov ah, 9");
            _mainVar.Add("int 21h");
            _mainVar.Add($"jmp start_input_int_{count}");
            _mainVar.Add($"end_input_int_{count}:");
            
        }
        #endregion read

        #region write
        private void Write()
        {
            Token tok = Scan(false); // write

            tok = Scan(false); // (
            if (tok.A.Equals('R') && tok.N.Equals(5))
            {
                OpenBrackets++;
               
                var var = Scan(true); // E || "text" || EL || I
                if (worldsArray.Count > AmountVar)
                {
                    Error("Write", 9);
                }

                if (var.A.Equals('L'))
                {
                    var = Scan(false);
                    Write_Lit(var);
                }
                else 
                {
                    if (var.A.Equals('I'))
                    {
                        switch (arrayOfVariables[var.N - 1].typeOfVar)
                        {
                            case 'i':
                                {
                                    E();
                                    Write_Int();
                                    break;
                                }
                            case 'c':
                                {
                                    var = Scan(false); // I
                                    tok = Scan(true); // [
                                    if (tok.A.Equals('R') && tok.N.Equals(13))
                                    {
                                        if (!arrayOfVariables[var.N - 1].IsArray())
                                        {
                                            Error("Write", 8);
                                        }

                                        tok = Scan(false);
                                        OpenBrackets++;
                                        E();
                                        tok = Scan(false);
                                        OpenBrackets--;
                                        if (!(tok.A.Equals('R') && tok.N == 14))
                                        {
                                            Error("Write", 3);
                                        }

                                    }
                                    else if (arrayOfVariables[var.N - 1].IsArray())
                                    {
                                        Error("Write", 8);
                                    }
                                    Write_Char(var);
                                    break;
                                }
                            case 'b':
                                {
                                    El();
                                    Write_Bool();
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    else 
                    {
                        E();
                        Write_Int();
                    }

                }

                tok = Scan(true); // ,
                while (tok.A.Equals('R') && tok.N.Equals(17))
                {
                    tok = Scan(false); // ,
                    var = Scan(true); // E || "text" || EL || I
                    if (worldsArray.Count > AmountVar)
                    {
                        Error("Write", 9);
                    }

                    if (var.A.Equals('L'))
                    {
                        var = Scan(false);
                        Write_Lit(var);
                    }
                    else if (var.A.Equals('I'))
                    {
                        switch (arrayOfVariables[var.N - 1].typeOfVar)
                        {
                            case 'i':
                                {
                                    E();
                                    Write_Int();
                                    break;
                                }
                            case 'c':
                                {
                                    var = Scan(false); // I
                                    tok = Scan(true); // [
                                    if (tok.A.Equals('R') && tok.N.Equals(13))
                                    {
                                        if (!arrayOfVariables[var.N - 1].IsArray())
                                        {
                                            Error("Write", 8);
                                        }

                                        tok = Scan(false);
                                        OpenBrackets++;
                                        E();
                                        tok = Scan(false);
                                        OpenBrackets--;
                                        if (!(tok.A.Equals('R') && tok.N == 14))
                                        {
                                            Error("Write", 3);
                                        }

                                    }
                                    else if (arrayOfVariables[var.N - 1].IsArray())
                                    {
                                        Error("Write", 8);
                                    }
                                    Write_Char(var);
                                    break;
                                }
                            case 'b':
                                {
                                    El();
                                    Write_Bool();
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    else
                    {
                        E();
                        Write_Int();
                    }

                    tok = Scan(true);
                }

                tok = Scan(false); // (
                OpenBrackets--;
                if (!(tok.A.Equals('R') && tok.N.Equals(6)))
                    Error("Write", 2);

            }
            else
            {
                Error("Write", 7);
            }
        }

        private void Write_Lit(Token lit)
        {
            int count = LabelCount;
            LabelCount++;
            _dataVar.Add($"var_{count} db {quotesTextArray[lit.N]}, 0Dh, 0Ah, \"$\"");
            _mainVar.Add($"lea dx, var_{count}");
            _mainVar.Add("mov ah, 9");
            _mainVar.Add("int 21h");
        }

        private void Write_Char(Token var)
        { 
            _mainVar.Add("xor ax, ax");
            if (!arrayOfVariables[var.N - 1].IsArray())
            {
                _mainVar.Add($"mov al, {arrayOfVariables[var.N - 1].varName}");
            }
            else
            {
                _mainVar.Add("pop di");
                _mainVar.Add($"mov al, {arrayOfVariables[var.N - 1].varName}[di]");
            }
            //main_var.Add("xor dx, dx");
            _mainVar.Add("mov dl, al");
            _mainVar.Add("mov ah, 2");
            _mainVar.Add("int 21h");

            _mainVar.Add("lea dx, clear");
            _mainVar.Add("mov ah, 9");
            _mainVar.Add("int 21h");
        }

        private void Write_Bool()
        {
            int count = LabelCount;
            LabelCount++;
            _mainVar.Add("pop ax");
            _mainVar.Add("cmp ax, 0");
            _mainVar.Add($"je l_false_{count}");
          
            _mainVar.Add("lea dx, @@true");
            _mainVar.Add($"jmp l_out_{count}");
            
            _mainVar.Add($"l_false_{count}:");
            _mainVar.Add("lea dx, @@false");

            _mainVar.Add($"l_out_{count}:");
            _mainVar.Add("mov ah, 9");
            _mainVar.Add("int 21h");

            _mainVar.Add("lea dx, clear");
            _mainVar.Add("mov ah, 9");
            _mainVar.Add("int 21h");
        }

        private void Write_Int()
        {
            int count = LabelCount;
            LabelCount++;
            _mainVar.Add($"pop ax");
            _mainVar.Add($"mov bx, 10");
            _mainVar.Add($"mov di, 0");
            _mainVar.Add($"mov si, ax");
            _mainVar.Add($"cmp ax, 0");
            _mainVar.Add($"jns l1_int_{count}");
           
            _mainVar.Add($"neg si");
            _mainVar.Add($"mov ah, 2");
            _mainVar.Add($"mov dl, \"-\"");
            _mainVar.Add($"int 21h");
            _mainVar.Add($"mov ax, si");
            
            _mainVar.Add($"l1_int_{count}:");
            _mainVar.Add($"xor dx, dx");
            _mainVar.Add($"div bx");
            _mainVar.Add($"add dl, 30h");
            _mainVar.Add($"mov output[di], dl");
            _mainVar.Add($"inc di");
            _mainVar.Add($"cmp al, 0");
            _mainVar.Add($"jnz l1_int_{count}");

            _mainVar.Add($"mov cx, di");
            _mainVar.Add($"dec di");
            _mainVar.Add($"mov ah, 2");
            
            _mainVar.Add($"l2_int_{count}:");
            _mainVar.Add($"mov dl, output[di]");
            _mainVar.Add($"dec di");
            _mainVar.Add($"int 21h");
            _mainVar.Add($"loop l2_int_{count}");

            _mainVar.Add("lea dx, clear");
            _mainVar.Add("mov ah, 9");
            _mainVar.Add("int 21h");
        }
        #endregion write

        #endregion programm

        #region debug
        private void Error(string func, int code)
        {
            Console.WriteLine($"В функции: {func}.\nПроизошла ошибка:");
            switch (code)
            {
                case 0: Console.Write("Не закрыты кавычки!"); break;
                case 1: Console.Write("Не известный символ!"); break;
                case 2: Console.Write("Не закрыта скобка ) !"); break;
                case 3: Console.Write("Не закрыта скобка ] !"); break;
                case 4: Console.Write("Необходим логический знак!"); break;
                case 5: Console.Write("Необходим идентификатор!"); break;
                case 6: Console.Write("Необходимо ; !"); break;
                case 7: Console.Write("Неправильный синтаксис!"); break;
                case 8: Console.Write("Неправильный базовый тип!"); break;
                case 9: Console.Write("Несуществующий идентификатор!"); break;

                default:
                    Console.Write("Неизвестная ошибка"); break;
            }
            Process.GetCurrentProcess().Kill();
        }
        #endregion debug
    }
}