using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PL0_Compiler
{
    public partial class Form2 : Form
    {
        public string str;
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            str = textBox1.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
