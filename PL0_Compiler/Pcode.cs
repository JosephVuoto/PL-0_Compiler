using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL0_Compiler
{
    class Pcode
    {
        /*定义操作码的枚举*/
        public enum PC
        {
            LIT, OPR, LOD, STO, CAL,
            INT, JMP, JPC, RED, WRT, NUL
        };

        /*相应的操作码名称*/
        public static String[] PCODE =
        {
            "LIT", "OPR", "LOD", "STO", "CAL",
            "INT", "JMP", "JPC", "RED", "WRT"
        };

        public PC f;    /*操作码*/
        public int l;   /*变量或过程被引用的分程序与说明该变量或过程的分程序的层次差*/
        public int a;   /*参数，对于不同指令有不同的含义*/

        public Pcode(PC _f, int _l, int _a)
        {
            f = _f;
            l = _l;
            a = _a;
        }
    }
}
