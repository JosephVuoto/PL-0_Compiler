using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PL0_Compiler
{
    public partial class Form1 : Form
    {
        SyntaxAnalysis sa;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sa = new SyntaxAnalysis();
            sa.Lex.CODES1 = richTextBox1.Text.ToString() + "  ";
            try
            {
                sa.analyse();
            } catch (Exception ex)
            {
                MessageBox.Show("缺少符号\n", "Compiler Error");
            }
            sa.interp.listCode(0);
            //sa.interp.interpret();
            richTextBox2.Text = sa.interp.pcodeOutput;
            //sa.interp.input = richTextBox3.Text;
            if (sa.Err.ErrorNum > 0)
                MessageBox.Show(sa.Err.ErrorMessage, "Compiler Error");
        }

        private void button2_Click(object sender, EventArgs e)
        {

            OpenFileDialog codeFile = new OpenFileDialog();
            codeFile.Filter = "All files(*.*)|*.*";
            if (codeFile.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.Text = "";
                StreamReader sr = File.OpenText(codeFile.FileName);
                while (sr.EndOfStream != true)
                {
                    richTextBox1.Text += sr.ReadLine() + "\n";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //sa.interp.interpret();
            richTextBox4.Text = "";
            sa.interp.interpInit();
            do
            {
                sa.interp.interpret();
                if (sa.interp.outputFlag == 1)
                {
                    richTextBox4.Text = sa.interp.output;
                    sa.interp.outputFlag = 0;
                }
                if (sa.interp.inputFlag == 1)
                {
                    int value = 0;
                    Form2 f = new Form2();
                    f.ShowDialog();
                    if (f.DialogResult == DialogResult.OK)
                        value = Convert.ToInt32(f.str);
                    //Console.WriteLine(value + "    asdasdasdasdasdsadasd");
                    sa.interp.runtimeStack[sa.interp.sp - 1] = value;
                    sa.interp.inputFlag = 0;
                }
            } while (sa.interp.pc != 0);
        }
    }
}
