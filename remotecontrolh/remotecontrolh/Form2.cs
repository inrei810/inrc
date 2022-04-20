using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using inreifunc;

namespace remotecontrolh
{
    public partial class Form2 : Form
    {
       
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (Inicontrol.Writetoini(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\INRC\\setting.ini","inrc","pass",textBox1.Text)&&Inicontrol.Writetoini(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\INRC\\setting.ini","inrc","server",textBox2.Text))
            {
                Close();

            }
            else
            {
                MessageBox.Show($"設定に失敗しました。\r\n{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}への正常なアクセス権があるか確認してください");
            }

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string s ,s1;
            Inicontrol.Readfromini(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\INRC\\setting.ini", "inrc", "pass", out s);
            Inicontrol.Readfromini(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\INRC\\setting.ini", "inrc", "server", out s1);
            textBox2.Text = s1;
            textBox1.Text = s;



        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WebClient wc2 = new WebClient();
            wc2.Encoding = System.Text.Encoding.UTF8;
            try
            {
                string st = wc2.DownloadString(textBox2.Text + "/check.php?key=" + textBox1.Text);
                if (st.Substring(0,2)=="0,")
                {
                    MessageBox.Show(st.Remove(0,2),"エラー",MessageBoxButtons.OK,MessageBoxIcon.Error);

                }
                if(st.Substring(0,2)=="1,")
                {
                    MessageBox.Show(st.Remove(0, 2), "やったぜ。", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("接続できませんでした\r\n"+ex.Message,"エラー",MessageBoxButtons.OK,MessageBoxIcon.Stop);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("key:発行されたキー名を入力します。\r\n接続先サーバー:URLを入力します。サーバーがINRCを導入している必要があります。URLはキー発行フォームに記載されているはずです。URLが不明な場合、サーバーの管理者等にお問い合わせください。\r\n例:https://example.com/inrc\r\nhttps://example.com");
        }
    }
}
