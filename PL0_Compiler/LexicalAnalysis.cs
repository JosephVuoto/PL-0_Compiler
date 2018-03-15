using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL0_Compiler
{
    class LexicalAnalysis
    {
        public enum SY
        {
            NONESY, CONSTSY, VARSY, PROCEDURESY, ODDSY, IFSY, THENSY, ELSESY,
            WHILESY, DOSY, CALLSY, BEGINSY, ENDSY, REPEATSY, UNTILSY,
            READSY, WRITESY, IDSY, INTSY, COMMASY, SEMISY, LPARSY, RPARSY,
            COLONSY, DOTSY, PLUSSY, MINUSSY, STARSY, DIVISY, EQUSY, LESSSY, GRTSY,
            ASSIGNSY, UNEQUSY, LOESY, GOESY, DOUBLESY, ERRORSY
        };
        
        public static String[] NAME = { "", "const", "var", "procedure", "odd", "if",
                                "then", "else", "while", "do", "call", "begin",
                                "end", "repeat", "until", "read", "write", "",
                                "", ",", ";", "(", ")", ":", ".", "+", "-", "*", "/",
                                "=", "<", ">", ":=", "<>", "<=", ">=", ""
                                };
        public static char[] charSet = { ',', '.', ';', '(', ')', ':', '+', '-', '*', '/', '<', '>', '=' };
        private char CHAR;          /*当前读的字符*/
        private String TOKEN;        /*单词字符串*/
        private String CODES;		/*源程序字符串*/
        public int NUM;                /*整数值*/
        public double DBL;
        public SY symbol;				/*当前单词的编码*/
        private int position;
        private bool isDouble;
        public int lineID;


        public string CODES1 { get => CODES; set => CODES = value; }
        public string TOKEN1 { get => TOKEN; set => TOKEN = value; }

        public LexicalAnalysis()
        {
            this.TOKEN1 = "";
            this.CODES = "";
            this.NUM = 0;
            this.position = 0;
            this.lineID = 1;
            //Console.WriteLine("advvxvxcvv");
        }

        public int getOPR(SY s)
        {
            switch(s)
            {
                case SY.PLUSSY:
                    return 2;
                case SY.MINUSSY:
                    return 3;
                case SY.STARSY:
                    return 4;
                case SY.DIVISY:
                    return 5;
                case SY.EQUSY:
                    return 8;
                case SY.UNEQUSY:
                    return 9;
                case SY.LESSSY:
                    return 10;
                case SY.GOESY:
                    return 11;
                case SY.GRTSY:
                    return 12;
                case SY.LOESY:
                    return 13;
            }
            return 0;
        }

        private bool isSpace()
        {
            return CHAR == ' ';
        }

        private bool isTab()
        {
            return CHAR == '\t';
        }

        private bool isNewline()
        {
            if (CHAR == '\n')
            {
                lineID++;
                return true;
            }
            else
                return false;
        }

        private bool isDigit()
        {
            return CHAR >= '0' && CHAR <= '9';
        }

        private bool isLetter()
        {
            return (CHAR >= 'a' && CHAR <= 'z') || (CHAR >= 'A' && CHAR <= 'Z');
        }

        private bool isColon()
        {
            return CHAR == ':';
        }

        private bool isDot()
        {
            return CHAR == '.';
        }

        private bool isPlus()
        {
            return CHAR == '+';
        }

        private bool isMinus()
        {
            return CHAR == '-';
        }

        private bool isStar()
        {
            return CHAR == '*';
        }

        private bool isDivi()
        {
            return CHAR == '/';
        }

        bool isLpar()
        {
            return CHAR == '(';
        }

        private bool isRpar()
        {
            return CHAR == ')';
        }

        private bool isComma()
        {
            return CHAR == ',';
        }

        private bool isSemi()
        {
            return CHAR == ';';
        }

        private bool isEqu()
        {
            return CHAR == '=';
        }

        private bool isGrt()
        {
            return CHAR == '>';
        }

        private bool isLess()
        {
            return CHAR == '<';
        }

        private bool isInCharSet()
        {
            return ((IList)charSet).Contains(CHAR);
        }

        private int transInt()
        {
            return Convert.ToInt32(TOKEN);
        }

        private double transDouble()
        {
            return Convert.ToDouble(TOKEN);
        }

        private bool isEOF()
        {
            return position == CODES.Length - 1;
        }

        private void clearToken()
        {
            TOKEN1 = "";
        }

        private void getChar()
        {
            CHAR = CODES.ToCharArray()[position];
            position++;
        }

        private void retreact()
        {
            position--;
        }

        private void catToken()
        {
            TOKEN1 += CHAR;
        }


        private int reserver()
        {
            if (TOKEN1 == "const")
                return 1;
            else if (TOKEN1 == "var")
                return 2;
            else if (TOKEN1 == "procedure")
                return 3;
            else if (TOKEN1 == "odd")
                return 4;
            else if (TOKEN1 == "if")
                return 5;
            else if (TOKEN1 == "then")
                return 6;
            else if (TOKEN1 == "else")
                return 7;
            else if (TOKEN1 == "while")
                return 8;
            else if (TOKEN1 == "do")
                return 9;
            else if (TOKEN1 == "call")
                return 10;
            else if (TOKEN1 == "begin")
                return 11;
            else if (TOKEN1 == "end")
                return 12;
            else if (TOKEN1 == "repeat")
                return 13;
            else if (TOKEN1 == "until")
                return 14;
            else if (TOKEN1 == "read")
                return 15;
            else if (TOKEN1 == "write")
                return 16;
            else
                return 0;
        }

        public void error()
        {
            Console.WriteLine("Err");
            while (!(isSpace() || isTab() || isNewline()))
            {
                if (isInCharSet())
                {
                    retreact();
                    break;
                }
                catToken();
                if (isEOF())
                    break;
                getChar();
            }
        }

        public int getsym()
        {
            //Console.WriteLine("dassdasd");
            clearToken();
            getChar();
            while ((isSpace() || isNewline() || isTab()) && position != CODES.Length)
                getChar();
            if (position == CODES.Length)
                return 0;
            if (isLetter())
            {
                while (isLetter() || isDigit())
                {
                    catToken();
                    getChar();
                    
                    if (!(isInCharSet() || isLetter() || isDigit() || isSpace() || isNewline() || isTab()))
                    {
                        error();
                        Console.WriteLine(CHAR + "\n11111");
                        symbol = SY.ERRORSY;
                        return 1;
                    }
                }
                retreact();
                int resultValue = reserver();
                if (resultValue == 0)
                {
                    symbol = SY.IDSY;
                }
                else
                {
                    symbol = (SY)resultValue;
                }
            }
            else if (isDigit())
            {
                isDouble = false;
                while (isDigit())
                {
                    catToken();
                    getChar();
                    if (isLetter() || (!(isInCharSet() || isLetter() || isDigit() || isSpace() || isTab() || isNewline())))
                    {
                        Console.WriteLine(CHAR);
                        Console.WriteLine("asdasd");
                        error();
                        symbol = SY.ERRORSY;
                        return 1;
                    }
                    if (isDot() && isDouble == false)
                    {
                        getChar();
                        if (isDigit())
                        {
                            System.Console.WriteLine(CHAR);
                            isDouble = true;
                            retreact();
                            retreact();
                            getChar();
                            catToken();
                            getChar();
                        }
                        else
                        {
                            retreact();
                        }
                    }
                }
                retreact();
                if (isDouble)
                {
                    DBL = transDouble();
                    symbol = SY.DOUBLESY;
                }
                else
                {
                    NUM = transInt();
                    symbol = SY.INTSY;
                }
            }
            else if (isComma())
            {
                symbol = SY.COMMASY;
            }
            else if (isSemi())
            {
                symbol = SY.SEMISY;
            }
            else if (isLpar())
            {
                symbol = SY.LPARSY;
            }
            else if (isRpar())
            {
                symbol = SY.RPARSY;
            }
            else if (isColon())
            {
                getChar();
                if (isEqu())
                {
                    symbol = SY.ASSIGNSY;
                }
                else
                {
                    retreact();
                    symbol = SY.COLONSY;
                }
            }
            else if (isDot())
            {
                symbol = SY.DOTSY;
            }
            else if (isPlus())
            {
                symbol = SY.PLUSSY;
            }
            else if (isMinus())
            {
                symbol = SY.MINUSSY;
            }
            else if (isStar())
            {
                symbol = SY.STARSY;
            }
            else if (isDivi())
            {
                symbol = SY.DIVISY;
            }
            else if (isEqu())
            {
                symbol = SY.EQUSY;
            }
            else if (isGrt())
            {
                getChar();
                if (isEqu())
                {
                    symbol = SY.GOESY;
                }
                else
                {
                    retreact();
                    symbol = SY.GRTSY;
                }
            }
            else if (isLess())
            {
                getChar();
                if (isGrt())
                {
                    symbol = SY.UNEQUSY;
                }
                else if (isEqu())
                {
                    symbol = SY.LOESY;
                }
                else
                {
                    retreact();
                    symbol = SY.LESSSY;
                }
            }
            else if (isEOF())
                return 0;
            else
            {
                //Console.WriteLine("asdavcxcv");
                error();
                symbol = SY.ERRORSY;
            }
            Console.WriteLine(lineID + " " + symbol);
            return 1;
            
        }
    }
}
