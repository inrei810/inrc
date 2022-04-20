using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INRC
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide(); }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {

            Hide();
            e.Cancel = true;
        }
        public bool ApeLog (string str)
            {
            DateTime dt = DateTime.Now;
　　　　　　
            textBox1.AppendText(dt.ToString("G")+str);
            return true;
            }
        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }
       
    }
}
