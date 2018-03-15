using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace PL0_Compiler
{
    class SyntaxAnalysis
    {
        private LexicalAnalysis lex;    /*词法分析器*/
        private SymbolTable symTable;   /*符号表*/
        public Interpreter interp;      /*解释器*/
        private int dx;                 /*当前作用域的堆栈帧大小*/
        public compilerError Err;       /*错误处理*/

        private List<LexicalAnalysis.SY> firstDeclare;
        private List<LexicalAnalysis.SY> firstStatement;
        private List<LexicalAnalysis.SY> firstFactor;

        internal LexicalAnalysis Lex { get => lex; set => lex = value; }

        /*测试当前符号是否合法*/
        private void checkList(List<LexicalAnalysis.SY> list1, List<LexicalAnalysis.SY> list2, int err)
        {
            if (!list1.Contains(lex.symbol))
            {
                Err.putErrorMessage(err, lex.lineID);
                list1 = list1.Union(list2).ToList<LexicalAnalysis.SY>();
                while (!list1.Contains(lex.symbol))
                    lex.getsym();
            }
        }

        /*初始化语法分析器，将当前作用域的堆栈帧大小置为0*/
        public SyntaxAnalysis()
        {
            lex = new LexicalAnalysis();
            symTable = new SymbolTable();
            interp = new Interpreter();
            Err = new compilerError();
            dx = 0;

            firstDeclare = new List<LexicalAnalysis.SY>();
            firstStatement = new List<LexicalAnalysis.SY>();
            firstFactor = new List<LexicalAnalysis.SY>();

            firstDeclare.Add(LexicalAnalysis.SY.CONSTSY);
            firstDeclare.Add(LexicalAnalysis.SY.VARSY);
            firstDeclare.Add(LexicalAnalysis.SY.PROCEDURESY);

            firstStatement.Add(LexicalAnalysis.SY.BEGINSY);
            firstStatement.Add(LexicalAnalysis.SY.CALLSY);
            firstStatement.Add(LexicalAnalysis.SY.IFSY);
            firstStatement.Add(LexicalAnalysis.SY.WHILESY);
            firstStatement.Add(LexicalAnalysis.SY.REPEATSY);

            firstFactor.Add(LexicalAnalysis.SY.IDSY);
            firstFactor.Add(LexicalAnalysis.SY.INTSY);
            firstFactor.Add(LexicalAnalysis.SY.LPARSY);
        }

        /*语法分析入口*/
        public void analyse()
        {
            List<LexicalAnalysis.SY> nextLevel = new List<LexicalAnalysis.SY>();
            nextLevel.AddRange(firstDeclare);
            nextLevel.AddRange(firstStatement);
            nextLevel.Add(LexicalAnalysis.SY.DOTSY);
            lex.getsym();
            parser(0, nextLevel);
            if (lex.symbol != LexicalAnalysis.SY.DOTSY)
                Err.putErrorMessage(9, lex.lineID);
        }

        /*分析块，参数为当前分程序所在的层和当前块的follow集*/
        private void parser(int level, List<LexicalAnalysis.SY> followSet)
        {
            List<LexicalAnalysis.SY> nextLevel = new List<LexicalAnalysis.SY>();
            int dx0 = dx;
            int tx0 = symTable.tablePtr;
            int cx0;
            dx = 3;
            symTable.setAddr(interp.arrayPtr, symTable.tablePtr);
            interp.produceCodes(Pcode.PC.JMP, 0, 0);
            do
            {
                if (lex.symbol == LexicalAnalysis.SY.CONSTSY)
                {
                    lex.getsym();
                    constDeclaration(level);
                    while (lex.symbol == LexicalAnalysis.SY.COMMASY)
                    {
                        lex.getsym();
                        constDeclaration(level);
                    }
                    if (lex.symbol == LexicalAnalysis.SY.SEMISY)
                        lex.getsym();
                    else
                        Err.putErrorMessage(5, lex.lineID);
                }
                if (lex.symbol == LexicalAnalysis.SY.VARSY)
                {
                    lex.getsym();
                    varDeclaration(level);
                    while (lex.symbol == LexicalAnalysis.SY.COMMASY)
                    {
                        lex.getsym();
                        varDeclaration(level);
                    }
                    if (lex.symbol == LexicalAnalysis.SY.SEMISY)
                        lex.getsym();
                    else
                        Err.putErrorMessage(5, lex.lineID);
                }

                while (lex.symbol == LexicalAnalysis.SY.PROCEDURESY)
                {
                    lex.getsym();
                    if (lex.symbol == LexicalAnalysis.SY.IDSY)
                    {
                        Console.WriteLine(lex.TOKEN1 + "    " + dx);
                        symTable.tableFill(lex.TOKEN1, SymbolTable.record.procedre, 0, level, interp.arrayPtr + 1);
                        lex.getsym();
                    }
                    else
                        Err.putErrorMessage(4, lex.lineID);
                    if (lex.symbol == LexicalAnalysis.SY.SEMISY)
                        lex.getsym();
                    else
                        Err.putErrorMessage(5, lex.lineID);
                    nextLevel = followSet;
                    nextLevel.Add(LexicalAnalysis.SY.SEMISY);
                    parser(level + 1, nextLevel);
                    if (lex.symbol == LexicalAnalysis.SY.SEMISY)
                    {
                        lex.getsym();
                        nextLevel = firstStatement;
                        nextLevel.Add(LexicalAnalysis.SY.IDSY);
                        nextLevel.Add(LexicalAnalysis.SY.PROCEDURESY);
                    }
                    else
                        Err.putErrorMessage(5, lex.lineID);
                }
                nextLevel = firstStatement;
                nextLevel.Add(LexicalAnalysis.SY.IDSY);
                checkList(nextLevel, firstDeclare, 7);

            } while (firstDeclare.Contains(lex.symbol));
            SymbolTable.record rec = symTable.getItem(tx0);
            //Console.WriteLine(tx0 + "   " + rec.address + "   " + interp.arrayPtr);
            interp.pcodeList[rec.address].a = interp.arrayPtr;
            rec.address = interp.arrayPtr;
            rec.size = dx;
            cx0 = interp.arrayPtr;
            interp.produceCodes(Pcode.PC.INT, 0, dx);
            nextLevel = followSet;
            nextLevel.Add(LexicalAnalysis.SY.SEMISY);
            nextLevel.Add(LexicalAnalysis.SY.ENDSY);
            statement(nextLevel, level);
            interp.produceCodes(Pcode.PC.OPR, 0, 0);
            nextLevel = new List<LexicalAnalysis.SY>();
            checkList(followSet, nextLevel, 8);
            dx = dx0;
            symTable.tablePtr = tx0;
        }

        /*常量说明*/
        private void constDeclaration(int level)
        {
            if (lex.symbol == LexicalAnalysis.SY.IDSY)
            {
                string name = lex.TOKEN1;
                lex.getsym();
                if (lex.symbol == LexicalAnalysis.SY.EQUSY || lex.symbol == LexicalAnalysis.SY.ASSIGNSY)
                {
                    if (lex.symbol == LexicalAnalysis.SY.ASSIGNSY)
                    {
                        Err.putErrorMessage(1, lex.lineID);
                    }
                    lex.getsym();
                    if (lex.symbol == LexicalAnalysis.SY.INTSY)
                    {
                        //Console.WriteLine("adasdasd");
                        symTable.tableFill(name, SymbolTable.record.constant, lex.NUM, level, dx);
                        lex.getsym();
                    }
                    else
                        Err.putErrorMessage(2, lex.lineID);
                }
                else
                    Err.putErrorMessage(3, lex.lineID);
            }
            else
                Err.putErrorMessage(4, lex.lineID);
        }

        /*变量说明*/
        private void varDeclaration(int level)
        {
            
            if (lex.symbol == LexicalAnalysis.SY.IDSY)
            {
                //Console.WriteLine(lex.TOKEN1);
                symTable.tableFill(lex.TOKEN1, SymbolTable.record.variable, 0, level, dx);
                dx++;
                lex.getsym();
            }
            else
                Err.putErrorMessage(4, lex.lineID);
        }

        private void condition(List<LexicalAnalysis.SY> followSet, int level)
        {
            if (lex.symbol == LexicalAnalysis.SY.ODDSY)
            {
                lex.getsym();
                expression(followSet, level);
                interp.produceCodes(Pcode.PC.OPR, 0, 6);
            }
            else
            {
                List<LexicalAnalysis.SY> nextLevel = followSet;
                nextLevel.Add(LexicalAnalysis.SY.EQUSY);
                nextLevel.Add(LexicalAnalysis.SY.UNEQUSY);
                nextLevel.Add(LexicalAnalysis.SY.LESSSY);
                nextLevel.Add(LexicalAnalysis.SY.LOESY);
                nextLevel.Add(LexicalAnalysis.SY.GRTSY);
                nextLevel.Add(LexicalAnalysis.SY.GOESY);
                expression(nextLevel, level);
                if (lex.symbol == LexicalAnalysis.SY.EQUSY || lex.symbol == LexicalAnalysis.SY.UNEQUSY || lex.symbol == LexicalAnalysis.SY.LESSSY ||
                    lex.symbol == LexicalAnalysis.SY.LOESY || lex.symbol == LexicalAnalysis.SY.GRTSY || lex.symbol == LexicalAnalysis.SY.GOESY)
                {
                    LexicalAnalysis.SY opType = lex.symbol;
                    lex.getsym();
                    expression(followSet, level);
                    interp.produceCodes(Pcode.PC.OPR, 0, lex.getOPR(opType));
                }
                else
                    Err.putErrorMessage(20, lex.lineID);
            }

        }

        /*分析因子*/
        private void factor(List<LexicalAnalysis.SY> followSet, int level)
        {
            checkList(firstFactor, followSet, 24);
            if (firstFactor.Contains(lex.symbol))
            {
                if (lex.symbol == LexicalAnalysis.SY.IDSY)
                {
                    int index = symTable.locate(lex.TOKEN1);
                    if (index > 0)
                    {
                        SymbolTable.record rec = symTable.getItem(index);
                        switch (rec.type)
                        {
                            case SymbolTable.record.constant:
                                interp.produceCodes(Pcode.PC.LIT, 0, rec.value);
                                break;
                            case SymbolTable.record.variable:
                                interp.produceCodes(Pcode.PC.LOD, level - rec.level, rec.address);
                                break;
                            case SymbolTable.record.procedre:
                                Err.putErrorMessage(21, lex.lineID);
                                break;
                        }
                    }
                    else
                        Err.putErrorMessage(11, lex.lineID);
                    lex.getsym();
                }
                else if (lex.symbol == LexicalAnalysis.SY.INTSY)
                {
                    int num = lex.NUM;
                    interp.produceCodes(Pcode.PC.LIT, 0, num);
                    lex.getsym();
                }
                else if (lex.symbol == LexicalAnalysis.SY.LPARSY)
                {
                    lex.getsym();
                    List<LexicalAnalysis.SY> nextLevel = followSet;
                    nextLevel.Add(LexicalAnalysis.SY.RPARSY);
                    expression(nextLevel, level);
                    if (lex.symbol == LexicalAnalysis.SY.RPARSY)
                        lex.getsym();
                    else
                        Err.putErrorMessage(22, lex.lineID);
                }
                else
                    checkList(followSet, firstFactor, 23);
            }
        }

        /*分析项*/
        private void term(List<LexicalAnalysis.SY> followSet, int level)
        {
            List<LexicalAnalysis.SY> nextLevel = followSet;
            followSet.Add(LexicalAnalysis.SY.STARSY);
            followSet.Add(LexicalAnalysis.SY.DIVISY);
            factor(nextLevel, level);
            while (lex.symbol == LexicalAnalysis.SY.STARSY || lex.symbol == LexicalAnalysis.SY.DIVISY)
            {
                LexicalAnalysis.SY opType = lex.symbol;
                lex.getsym();
                factor(nextLevel, level);
                interp.produceCodes(Pcode.PC.OPR, 0, lex.getOPR(opType));
            }
        }

        /*分析表达式*/
        private void expression(List<LexicalAnalysis.SY> followSet, int level)
        {
            if (lex.symbol == LexicalAnalysis.SY.PLUSSY || lex.symbol == LexicalAnalysis.SY.MINUSSY)
            {
                lex.getsym();
                List<LexicalAnalysis.SY> nextLevel = followSet;
                nextLevel.Add(LexicalAnalysis.SY.PLUSSY);
                nextLevel.Add(LexicalAnalysis.SY.MINUSSY);
                term(nextLevel, level);
                LexicalAnalysis.SY opType = lex.symbol;
                if (opType == LexicalAnalysis.SY.MINUSSY)
                {
                    interp.produceCodes(Pcode.PC.OPR, 0, 1);
                }
            }
            else
            {
                List<LexicalAnalysis.SY> nextLevel = followSet;
                nextLevel.Add(LexicalAnalysis.SY.PLUSSY);
                nextLevel.Add(LexicalAnalysis.SY.MINUSSY);
                term(nextLevel, level);
            }

            while (lex.symbol == LexicalAnalysis.SY.PLUSSY || lex.symbol == LexicalAnalysis.SY.MINUSSY)
            {
                LexicalAnalysis.SY opType = lex.symbol;
                lex.getsym();
                List<LexicalAnalysis.SY> nextLevel = followSet;
                nextLevel.Add(LexicalAnalysis.SY.PLUSSY);
                nextLevel.Add(LexicalAnalysis.SY.MINUSSY);
                term(nextLevel, level);
                interp.produceCodes(Pcode.PC.OPR, 0, lex.getOPR(opType));
            }
        }

        private void statement(List<LexicalAnalysis.SY> followSet, int level)
        {
            switch (lex.symbol)
            {
                case LexicalAnalysis.SY.IDSY:
                    assignStatement(followSet, level);
                    break;
                case LexicalAnalysis.SY.READSY:
                    readStatement(followSet, level);
                    break;
                case LexicalAnalysis.SY.WRITESY:
                    writeStatement(followSet, level);
                    break;
                case LexicalAnalysis.SY.CALLSY:
                    callStatement(followSet, level);
                    break;
                case LexicalAnalysis.SY.IFSY:
                    ifStatement(followSet, level);
                    break;
                case LexicalAnalysis.SY.BEGINSY:
                    beginStatement(followSet, level);
                    break;
                case LexicalAnalysis.SY.WHILESY:
                    whileStatement(followSet, level);
                    break;
                case LexicalAnalysis.SY.REPEATSY:
                    repeatStatement(followSet, level);
                    break;
                default:
                    List<LexicalAnalysis.SY> nextLevel = new List<LexicalAnalysis.SY>();
                    checkList(followSet, nextLevel, 19);
                    break;
            }
        }
        
        private void assignStatement(List<LexicalAnalysis.SY> followSet, int level)
        {
            //Console.WriteLine(symTable.tablePtr);
            int index = symTable.locate(lex.TOKEN1);
            //Console.WriteLine(index);
            if (index > 0)
            {
                SymbolTable.record rec = symTable.getItem(index);
                if (rec.type == SymbolTable.record.variable)
                {
                    lex.getsym();
                    if (lex.symbol == LexicalAnalysis.SY.ASSIGNSY)
                        lex.getsym();
                    else
                        Err.putErrorMessage(13, lex.lineID);
                    List<LexicalAnalysis.SY> nextLevel = followSet;
                    expression(nextLevel, level);
                    interp.produceCodes(Pcode.PC.STO, level - rec.level, rec.address);
                }
                else
                    Err.putErrorMessage(12, lex.lineID);
            }
            else
                Err.putErrorMessage(11, lex.lineID);
        }

        private void readStatement(List<LexicalAnalysis.SY> followSet, int level)
        {
            lex.getsym();
            if (lex.symbol == LexicalAnalysis.SY.LPARSY)
            {
                int index = 0;
                do
                {
                    lex.getsym();
                    if (lex.symbol == LexicalAnalysis.SY.IDSY)
                        index = symTable.locate(lex.TOKEN1);
                    if (index < 0)
                        Err.putErrorMessage(11, lex.lineID);
                    else
                    {
                        SymbolTable.record rec = symTable.getItem(index);
                        if (rec.type == SymbolTable.record.variable)
                        {
                            interp.produceCodes(Pcode.PC.OPR, 0, 16);
                            interp.produceCodes(Pcode.PC.STO, level - rec.level, rec.address);
                        }
                        else
                            Err.putErrorMessage(32, lex.lineID);
                    }
                    lex.getsym();
                } while (lex.symbol == LexicalAnalysis.SY.COMMASY);
            }
            else
                Err.putErrorMessage(33, lex.lineID);
            if (lex.symbol == LexicalAnalysis.SY.RPARSY)
                lex.getsym();
            else
                Err.putErrorMessage(22, lex.lineID);
        }

        private void writeStatement(List<LexicalAnalysis.SY> followSet, int level)
        {
            lex.getsym();
            if (lex.symbol == LexicalAnalysis.SY.LPARSY)
            {
                do
                {
                    lex.getsym();
                    List<LexicalAnalysis.SY> nextLevel = followSet;
                    nextLevel.Add(LexicalAnalysis.SY.COMMASY);
                    nextLevel.Add(LexicalAnalysis.SY.RPARSY);
                    expression(nextLevel, level);
                    interp.produceCodes(Pcode.PC.OPR, 0, 14);
                } while (lex.symbol == LexicalAnalysis.SY.COMMASY);

                if (lex.symbol == LexicalAnalysis.SY.RPARSY)
                    lex.getsym();
                else
                    Err.putErrorMessage(22, lex.lineID);
            }
            else
                Err.putErrorMessage(33, lex.lineID);
            interp.produceCodes(Pcode.PC.OPR, 0, 15);
        }

        private void callStatement(List<LexicalAnalysis.SY> followSet, int level)
        {
            lex.getsym();
            if (lex.symbol == LexicalAnalysis.SY.IDSY)
            {
                int index = symTable.locate(lex.TOKEN1);
                if (index > 0)
                {
                    SymbolTable.record rec = symTable.getItem(index);
                    if (rec.type == SymbolTable.record.procedre)
                    {
                        Console.WriteLine(rec.name + rec.address);
                        interp.produceCodes(Pcode.PC.CAL, level - rec.level, rec.address);
                    }
                        
                    else
                        Err.putErrorMessage(15, lex.lineID);
                }
                else
                    Err.putErrorMessage(11, lex.lineID);
                lex.getsym();
            }
            else
                Err.putErrorMessage(14, lex.lineID);
        }

        private void ifStatement(List<LexicalAnalysis.SY> followSet, int level)
        {
            lex.getsym();
            List<LexicalAnalysis.SY> nextLevel = followSet;
            nextLevel.Add(LexicalAnalysis.SY.THENSY);
            nextLevel.Add(LexicalAnalysis.SY.DOSY);
            condition(nextLevel, level);
            if (lex.symbol == LexicalAnalysis.SY.THENSY)
                lex.getsym();
            else
                Err.putErrorMessage(16, lex.lineID);
            int cx1 = interp.arrayPtr;
            interp.produceCodes(Pcode.PC.JPC, 0, 0);
            statement(followSet, level);
            interp.pcodeList[cx1].a = interp.arrayPtr;
            if (lex.symbol == LexicalAnalysis.SY.ELSESY)
            {
                interp.pcodeList[cx1].a++;
                lex.getsym();
                int tmpPtr = interp.arrayPtr;
                interp.produceCodes(Pcode.PC.JMP, 0, 0);
                statement(followSet, level);
                interp.pcodeList[tmpPtr].a = interp.arrayPtr;
            }
        }

        private void beginStatement(List<LexicalAnalysis.SY> followSet, int level)
        {
            lex.getsym();
            List<LexicalAnalysis.SY> nextLevel = followSet;
            nextLevel.Add(LexicalAnalysis.SY.SEMISY);
            nextLevel.Add(LexicalAnalysis.SY.ENDSY);
            statement(nextLevel, level);
            while (firstStatement.Contains(lex.symbol) || lex.symbol == LexicalAnalysis.SY.SEMISY)
            {
                if (lex.symbol == LexicalAnalysis.SY.SEMISY)
                    lex.getsym();
                else
                    Err.putErrorMessage(10, lex.lineID);
                statement(nextLevel, level);
            }
            if (lex.symbol == LexicalAnalysis.SY.ENDSY)
                lex.getsym();
            else
                Err.putErrorMessage(17, lex.lineID);
        }

        private void whileStatement(List<LexicalAnalysis.SY> followSet, int level)
        {
            Console.WriteLine(interp.arrayPtr);
            int cx1 = interp.arrayPtr;
            lex.getsym();
            List<LexicalAnalysis.SY> nextLevel = followSet;
            nextLevel.Add(LexicalAnalysis.SY.DOSY);
            condition(nextLevel, level);
            int cx2 = interp.arrayPtr;
            interp.produceCodes(Pcode.PC.JPC, 0, 0);
            if (lex.symbol == LexicalAnalysis.SY.DOSY)
                lex.getsym();
            else
                Err.putErrorMessage(18, lex.lineID);
            statement(followSet, level);
            interp.produceCodes(Pcode.PC.JMP, 0, cx1);
            interp.pcodeList[cx2].a = interp.arrayPtr;
        }

        private void repeatStatement(List<LexicalAnalysis.SY> followSet, int level)
        {
            int cx1 = interp.arrayPtr;
            lex.getsym();
            List<LexicalAnalysis.SY> nextLevel = followSet;
            nextLevel.Add(LexicalAnalysis.SY.SEMISY);
            nextLevel.Add(LexicalAnalysis.SY.UNTILSY);
            statement(nextLevel, level);
            while (firstStatement.Contains(lex.symbol) || lex.symbol == LexicalAnalysis.SY.SEMISY)
            {
                if (lex.symbol == LexicalAnalysis.SY.SEMISY)
                    lex.getsym();
                else
                    Err.putErrorMessage(33, lex.lineID);
                statement(nextLevel, level);
            }
            if (lex.symbol == LexicalAnalysis.SY.UNTILSY)
            {
                lex.getsym();
                condition(followSet, level);
                interp.produceCodes(Pcode.PC.JPC, 0, cx1);
            }
            else
                Err.putErrorMessage(34, lex.lineID);
        }
    }
}
