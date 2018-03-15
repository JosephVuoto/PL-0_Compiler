using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL0_Compiler
{
    class compilerError
    {
        public static string[] errors = new string[]
        {
            "",
            "01. 常数说明中的=写成:=",
            "02. 常数说明中的=后应是数字",
            "03. 常数说明中的标识符后应是=",
            "04. const，var，procedure 后应是标识符",
            "05. 漏掉了,或;",
            "06. 过程说明后的符号不正确（应是语句开始符或过程定义符）",
            "07. 应是语句开始符",
            "08. 程序体内语句部分的后跟符不正确",
            "09. 程序结尾丢了句号.",
            "10. 语句之间漏了;",
            "11. 标识符未说明",
            "12. 赋值语句中，赋值号左部标识符属性应是变量",
            "13. 赋值语句左部标识符后应是赋值号:=",
            "14. call后应为标识符",
            "15. call后标识符属性应为过程",
            "16. 条件语句中丢了then",
            "17. 丢了end或;",
            "18. while型循环语句中丢了do",
            "19. 语句后的符号不正确",
            "20. 应为关系运算符",
            "21. 表达式内标识符属性不能是过程",
            "22. 表达式中漏掉右括号)",
            "23. 因子后的非法符号", 
            "24. 表达式的开始符不能是此符号",
            "25. ",
            "26. ",
            "27. ",
            "28. ",
            "29. ",
            "30. ",
            "31. 数越界",
            "32. read语句括号中的标识符不是变量",
            "33. 缺少左括号",
            "34. 漏掉until"
        };

        private int errorNum;
        private string errorMessage;

        public compilerError()
        {
            errorNum = 0;
            errorMessage = "";
        }

        public string ErrorMessage { get => errorMessage; set => errorMessage = value; }
        public int ErrorNum { get => errorNum; set => errorNum = value; }

        public void putErrorMessage(int errorID, int line)
        {
            errorMessage += "Line: " + line + "\nError Message: " + errors[errorID] + "\n\n";
            errorNum++;
        }
    }
}
