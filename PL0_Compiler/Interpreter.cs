using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL0_Compiler
{
    class Interpreter
    {
        private const int stackSize = 1000;
        private const int arraySize = 500;
        public Pcode[] pcodeList;
        public string pcodeOutput;
        //public string input;
        public string output;
        public int inputFlag;
        public int outputFlag;

        public int[] runtimeStack;  /*运行栈*/
        public int arrayPtr;        /*指令寄存器 index*/
        public int pc;              /*程序地址寄存器 pc*/
        public int bp;              /*基址寄存器（指针）bp*/
        public int sp;              /*栈顶寄存器（指针）sp*/

        public Interpreter()
        {
            pcodeOutput = "";
            arrayPtr = 0;
            pcodeList = new Pcode[arraySize];
            for (int i = 0; i < arraySize; i++)
                pcodeList[i] = new Pcode(Pcode.PC.NUL, 0, 0);
            inputFlag = 0;
            outputFlag = 0;
        }

        public void interpInit()
        {
            runtimeStack = new int[stackSize];
            Array.Clear(runtimeStack, 0, stackSize);
            output = "";
            pc = 0;
            bp = 0;
            sp = 0;
        }

        //生成P-Code代码
        public void produceCodes(Pcode.PC f, int l, int a)
        {
            pcodeList[arrayPtr++] = new Pcode(f, l, a);
        }

        //列出所有代码
        public void listCode(int start)
        {
            for (int i = start; i < arrayPtr; i++)
            {
                pcodeOutput += i.ToString() + "\t" + Pcode.PCODE[(int)pcodeList[i].f].ToString() + "\t" + 
                    pcodeList[i].l.ToString() + "\t" + pcodeList[i].a.ToString() + "\n";

            }
        }

        public void interpret()
        {

                Console.WriteLine("asd" + pc);
                Pcode tempCode = pcodeList[pc++];
                //output += pc.ToString() + "\t" + Pcode.PCODE[(int)tempCode.f].ToString() + "\t" + tempCode.l.ToString() + "\t" + tempCode.a.ToString() + "\n";
                switch (tempCode.f)
                {
                    case Pcode.PC.LIT:
                        runtimeStack[sp++] = tempCode.a;
                        break;
                    case Pcode.PC.OPR:
                        //Console.WriteLine("vvvvvvv");
                        switch (tempCode.a)
                        {
                            case 0:
                                sp = bp;
                                pc = runtimeStack[sp + 2];
                                bp = runtimeStack[sp + 1];
                                break;
                            case 1:
                                runtimeStack[sp - 1] = -runtimeStack[sp - 1];
                                break;
                            case 2:
                                sp--;
                                runtimeStack[sp - 1] += runtimeStack[sp];
                                break;
                            case 3:
                                sp--;
                                runtimeStack[sp - 1] -= runtimeStack[sp];
                                break;
                            case 4:
                                sp--;
                                runtimeStack[sp - 1] *= runtimeStack[sp];
                                break;
                            case 5:
                                sp--;
                                runtimeStack[sp - 1] /= runtimeStack[sp];
                                break;
                            case 6:
                                sp--;
                                runtimeStack[sp - 1] %= 2;
                                break;
                            case 7:
                                sp--;
                                runtimeStack[sp - 1] %= runtimeStack[sp];
                                break;
                            case 8:
                                sp--;
                                runtimeStack[sp - 1] = (runtimeStack[sp] == runtimeStack[sp - 1] ? 1 : 0);
                                break;
                            case 9:
                                sp--;
                                runtimeStack[sp - 1] = (runtimeStack[sp] != runtimeStack[sp - 1] ? 1 : 0);
                                break;
                            case 10:
                                sp--;
                                runtimeStack[sp - 1] = (runtimeStack[sp - 1] < runtimeStack[sp] ? 1 : 0);
                                break;
                            case 11:
                                sp--;
                                runtimeStack[sp - 1] = (runtimeStack[sp - 1] >= runtimeStack[sp] ? 1 : 0);
                                break;
                            case 12:
                                sp--;
                                runtimeStack[sp - 1] = (runtimeStack[sp - 1] > runtimeStack[sp] ? 1 : 0);
                                break;
                            case 13:
                                sp--;
                                runtimeStack[sp - 1] = (runtimeStack[sp - 1] <= runtimeStack[sp] ? 1 : 0);
                                break;
                            case 14:
                                outputFlag = 1;
                                output += runtimeStack[sp - 1];
                                break;
                            case 15:
                                outputFlag = 1;
                                output += "\n";
                                break;
                            case 16:
                                inputFlag = 1;
                                break;
                        }
                        break;
                    case Pcode.PC.LOD:
                        runtimeStack[sp] = runtimeStack[getBasePtr(tempCode.l, runtimeStack, bp) + tempCode.a];
                        sp++;
                        break;
                    case Pcode.PC.STO:
                        sp--;
                        runtimeStack[getBasePtr(tempCode.l, runtimeStack, bp) + tempCode.a] = runtimeStack[sp];
                        break;
                    case Pcode.PC.CAL:
                        runtimeStack[sp] = getBasePtr(tempCode.l, runtimeStack, bp);
                        runtimeStack[sp + 1] = bp;
                        runtimeStack[sp + 2] = pc;
                        bp = sp;
                        pc = tempCode.a;
                        break;
                    case Pcode.PC.INT:
                        sp += tempCode.a;
                        break;
                    case Pcode.PC.JMP:
                        pc = tempCode.a;
                        break;
                    case Pcode.PC.JPC:
                        sp--;
                        if (runtimeStack[sp] == 0)
                            pc = tempCode.a;
                        break;
                }
        }

        public int getBasePtr(int l, int[] runtimeStack, int b)
        {
            while (l > 0)
            {
                b = runtimeStack[b];
                l--;
            }
            return b;
        }
    }
}
