using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL0_Compiler
{
    class SymbolTable
    {
        //private int lenth;
        public int tablePtr;                /*符号表指针，指向当前记录*/
        /*符号表记录的结构体，设置有3种类型：constant(0), variable(1), procedure(2)*/
        public struct record {
            public const int constant = 0;
            public const int variable = 1;
            public const int procedre = 2;
            public String name;             /*记录的名字*/
            public int type;                /*记录的类型*/
            public int value;               /*记录的值*/
            public int level;               /*记录所在的层*/
            public int address;             /*记录的地址*/
            public int size;                /*需要分配的空间，当记录类型为procedure时使用，默认值为0*/
        };
        public List<record> table;          /*符号表*/

        public SymbolTable()
        {
            tablePtr = 0;
            table = new List<record>();
            table.Add(new record());
        }

        /*向符号表中添加一条记录*/
        public void tableFill(String name, int type, int value, int level, int dx)
        {
            tablePtr++;
            record rec = new record();
            rec.name = name;
            rec.type = type;
            rec.value = value;
            rec.level = level;
            rec.address = dx;
            table.Add(rec);
        }

        /*根据序号获取符号表中的记录*/
        public record getItem(int i)
        {
            if (i >= table.Count())
                table.Add(new record());
            return table[i];
        }

        /*查找标识符在符号表中的位置*/
        public int locate(string name)
        {
            //Console.WriteLine(name);
            for (int i = table.Count - 1; i > 0; i--)
            {
                if (table[i].name == name)
                    return i;
            }
            return -1;
        }

        public void setAddr(int addr, int pos)
        {
            record temp = table[pos];
            temp.address = addr;
            table[pos] = temp;
        }
    }
}
