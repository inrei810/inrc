using INRC;
using inreifunc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace remotecontrolh
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Form4 form4 = new Form4();
        bool notouch;
        bool loop =false;
        private void Form1_Load(object sender, EventArgs e)
        {
           

            if (File.Exists($@"C:\Users\{Environment.UserName}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\remotecontrol.lnk"))
            {
                checkBox2.Checked = true;
            }
          
           
          if (Program.Hidestart == true||Program.Restart==true)
            {
                button1.PerformClick();
                
            }
            else
            {
                MessageBox.Show("これは、このプログラムを実行したPCを遠隔操作するプログラムです。\r次の画面で受付ボタンを押すと遠隔操作を受け付けます。\r\n他人のPCで無断で実行しないでください\r\n何かトラブルがあっても、作者は一切責任を取りません。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                
            }
            DateTime dt = DateTime.Now;
        
            form4.ApeLog(" [INFO]起動しました\r\n");
        }

        //rootは受信したコマンド命令
        string root;
        //Status,warningは警告について
        string Status;
        string warning;
        //プロセスクラスの初期化
        Process process = new Process();
        //変数cmdを初期化
        string cmd = "";
        public string UDK(string root)
        {
            //このメソッドで受信を行う
            WebClient wc2 = new WebClient();
            wc2.Encoding = System.Text.Encoding.UTF8;
            return wc2.DownloadString(url+$"/user/send{root}.txt");
        }
        string url;
        public async void button1_Click(object sender, EventArgs e)
        {

           if(!Inicontrol.Readfromini(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\INRC\\setting.ini", "inrc", "server", out url))
            {
                
                MessageBox.Show("設定画面を開き、設定してください", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;
            try
            {
                //警告文をダウンロード
                Status = wc.DownloadString(url + "/user/status.txt");
                warning = wc.DownloadString(url + "/user/warning.txt");
            }

            catch (Exception ex)
            {
                DateTime dt = DateTime.Now;

                form4.ApeLog(" [Exception]:メッセージを受信できませんでした\r\n");
                MessageBox.Show(ex.Message);
            }
            if (Inicontrol.Readfromini(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\INRC\\setting.ini","inrc","pass",out root) && Status=="")
            {
               
                Post("", "ar", root,url);

                if (warning!="")
                {
                    //exeの引数を確認し、もし再起動から起動したのであればメッセージをダイアログしない
                    if (Program.Hidestart == true || Program.Restart == true)
                    {
                        DateTime dt = DateTime.Now;

                        form4.ApeLog(" [Info]:メッセージ:" + warning + "\r\n");


                    }
                    else
                    {
                        DateTime dt = DateTime.Now;

                        form4.ApeLog(" [Info]:メッセージを受信しました\r\n");
                        MessageBox.Show(warning, "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                try 
                {
        

                    form4.ApeLog(" [Info]:Settingを読み込みました\r\n");
                }
                catch (Exception ex)
                {
                    DateTime dt = DateTime.Now;

                    form4.ApeLog(" [Exception]:"+ex.Message+"\r\n");
                    MessageBox.Show(ex.Message);

                }
                //下準備
                loop = true;
                label1.Text = "実行中";
                label1.ForeColor = Color.Green;
                while ( loop==true)
                {
                    //while文を使い、1秒に一回、命令がないか確認する
                    try
                    {
                        await Task.Delay(1000);
                   
                        try
                        {
                            //命令をダウンロードしてcmdに格納
                            cmd = UDK(root);


                        }
                        catch (WebException ex)
                        {
                            DateTime dt = DateTime.Now;

                            form4.ApeLog($" [Error]:{ex.Message}\r\n");

                            MessageBox.Show("接続できませんでした。\r\nキーが間違っているか、登録されていません", "エラーorz", MessageBoxButtons.OK, MessageBoxIcon.Stop); 

                            button2.PerformClick();
                            cmd = "";
                        }
                        catch (Exception ex)
                        {
                            DateTime dt = DateTime.Now;

                            form4.ApeLog(" [Exception]:"+ex.Message+"\r\n");

                            MessageBox.Show(ex.Message);
                            button2.PerformClick();
                            cmd = "";
                        }
                      //もし命令があるなら
                        if (cmd != "")
                        {
                            DateTime dt = DateTime.Now;

                            form4.ApeLog(" [Receive]:" + cmd + "\r\n");

                            //サーバー上に記載されている命令を消す
                            Post("", "ar", root,url);
                            if (cmd.Length>=9)
                            {
                                string msg = cmd.Substring(0, 10);
                                if (msg == "/msgbox-x:")
                                {
                                    cmd = cmd.Replace("/msgbox-x:", "");
                                    cmd = cmd.Replace(@"\r\n", "\r\n");
                                    Task.Run(() => { MessageBox.Show(cmd, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop); });
                                    Post("", "ar", root,url);
                                    continue;
                                }
                                else if (msg == "/msgbox-!:")
                                {
                                    cmd = cmd.Replace("/msgbox-!:", "");
                                    cmd = cmd.Replace(@"\r\n", "\r\n");

                                    Task.Run(() => { MessageBox.Show(cmd, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); });

                                    Post("", "ar", root,url);
                                    continue;

                                }
                                else if(msg =="/msgbox-i:")
                                {
                                    cmd = cmd.Replace("/msgbox-i:", "");
                                    cmd = cmd.Replace(@"\r\n", "\r\n");

                                    Task.Run(() => { MessageBox.Show(cmd, "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);});
                                    Post("", "ar", root,url);
                                    continue ;
                                }
                            }
                            if (cmd == "/exit")
                            {
                                //終了
                                Post("RC:終了しました。", "r", root,url);
                                await Task.Delay(1000);

                                Application.Exit();
                            }
                            else if (cmd == "/restart")
                            {
                                //再起動
                                Post("RC:再起動しました。", "r", root,url);
                                await Task.Delay(1000);

                                Application.Exit();
                                if (hide == true)
                                {
                                    System.Diagnostics.Process.Start(Application.ExecutablePath, "HIDESTART");

                                }
                                else
                                {
                                    System.Diagnostics.Process.Start(Application.ExecutablePath, "RESTART");

                                }
                            }
                            else if (cmd == "/startup")
                            {
                                //スタートアップキリカえ
                                if (checkBox2.Checked == true)
                                {
                                    checkBox2.Checked = false;
                                    Post("startup:false", "r", root,url);

                                    form4.ApeLog(" [Run]:Startupをfalseにしました\r\n");
                                }
                                else
                                {
                                    checkBox2.Checked = true;
                                    Post("startup:true", "r", root,url);
                                    form4.ApeLog(" [Run]:Startupをtrueにしました\r\n");

                                }
                            }
                            else if (cmd == "/log")
                            {
                                //logの有無
                                if (checkBox1.Checked == true)
                                {
                                    checkBox1.Checked = false;
                                    Post("Log:false", "r", root,url);
                                    
                                    form4.ApeLog(" [Run]:logをfalseにしました\r\n");


                                }
                                else
                                {
                                    checkBox1.Checked = true;
                                    Post("Log:true", "r", root,url);
                                    form4.ApeLog(" [Run]:logをtrueにしました\r\n");
                                  

                                }
                            }
                            else if (cmd == "/del")
                            {
                                //自滅
                                try
                                {

                                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/setting.txt");
                                }
                                catch { }
                               
                                Process p = new Process();
                                p.StartInfo.FileName = "cmd.exe";
                                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                p.StartInfo.Arguments = $"/c taskkill /im {Path.GetFileName(Application.ExecutablePath)} /f & timeout 2 & del \"{Application.ExecutablePath}\" /q";
                                p.Start();
                            }
                            else if (0 <= cmd.IndexOf("cd "))
                            {
                                //カレントディレクトリ移動
                                cmd = cmd.Replace("cd ", "");

                                Environment.CurrentDirectory = cmd;
                                await Task.Delay(1000);

                                Post("カレントディレクトリを" + Environment.CurrentDirectory + "にしました", "r",root,url);
                                form4.ApeLog(" [Run]:カレントディレクトリを変更しました\r\n");

                            }
                            else
                            {
                                //Processオブジェクトを作成する
                                if (0 <= cmd.IndexOf("/hourippa"))
                                {
                                    notouch = true;
                                    cmd = cmd.Replace("/hourippa","");
                                }
                                //cmd実行
                                label1.Text = "処理中";
                                Text = "INRC-処理中";

                                form4.ApeLog(" [Run]:" + "cmd " + "/c " + cmd + " 終了を待機します\r\n");
                                if (notouch != true)
                                {
                               
                                    await Task.Run(() => {
                                        try
                                        {
                                            Process process1 = new Process();
                                            process1.StartInfo.FileName = "cmd";
                                            process1.StartInfo.Arguments = "/c " + cmd;
                                            process1.StartInfo.CreateNoWindow = true;
                                            process1.StartInfo.UseShellExecute = false;
                                            process1.StartInfo.RedirectStandardOutput = true; 
                                            process1.StartInfo.RedirectStandardError = true;
                                            process1.Start();
                                            result = process1.StandardOutput.ReadToEnd() + process1.StandardError.ReadToEnd();
                                            process1.WaitForExit();
                                            process1.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            ew = true;
                                            Invoke((MethodInvoker)(() => {

                                                Post("Error:" + ex.Message, "r", root,url);
                                                form4.ApeLog($" [Exception]: { ex.Message} 例外の内容が送信されました\r\n");
                                            }));
                                        }

                                    });

                                }
                                else
                                {
                                    notouch = false;
                                    System.Diagnostics.Process process18 = new System.Diagnostics.Process();
                                    process18.StartInfo.FileName = "cmd";
                                    process18.StartInfo.Arguments = "/k " + cmd;
                                    process18.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                    process18.Start();
                                    result = "ほうりっぱモードがオンなので実行結果を返せません";
                                    process18.Close();
                                }
                                if(!ew)
                                {
                                    label1.Text = "実行中";
                                    Text = "INRC";

                                    form4.ApeLog("└>終了しました\r\n");

                                    if (checkBox1.Checked == true)
                                    {
                                        File.AppendAllText("./result.log", result + "\r\n"); form4.ApeLog(" [Run]:Logを出力しました\r\n");
                                    }

                                    int byteCount = Encoding.GetEncoding("Shift_JIS").GetByteCount(result);
                                    if (byteCount <= 1048576)
                                    {
                                        result = result.Replace("", "");
                                        Post(result, "r", root,url);
                                        form4.ApeLog(" [Run]:実行結果を送信しました\r\n");


                                    }
                                    else
                                    {
                                        Post("実行結果のサイズが1MB以上です。", "r", root,url);
                                        form4.ApeLog(" [Error]:実行結果のサイズが1MB以上です。サーバーへの負荷が予想されたため、実行結果の送信はされていません。\r\n");

                                    }
                                }
                                else
                                {
                                    label1.Text = "実行中";
                                    Text = "INRC";
                                    ew = false;
                                }
  

                            }
                        }

                    }
                    catch (Exception ex) { Post("Error:" + ex.Message, "r", root,url);

                        form4.ApeLog($" [Exception]: { ex.Message} 例外の内容が送信されました\r\n");
                    }


                }
            }
            else if (Status!="")
            {
                MessageBox.Show(Status,"エラー",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                DateTime dt = DateTime.Now;

                form4.ApeLog($" [Error]:サーバーエラーを受信しました\r\n");

            }
            else
            {
                MessageBox.Show("settingが見つかりません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                DateTime dt = DateTime.Now;

                form4.ApeLog($" [Error]:settingが見つかりません\r\n");
            }

        }
        private void process1_Exited(object sender, EventArgs e)
        {
           

        }
     
     
      private int Proctrl(string cmd)
        {
            try
            {

                if (notouch!=true)
                {
                    System.Diagnostics.Process process1 = new System.Diagnostics.Process();
                    process1.StartInfo.FileName = "cmd";
                    process1.StartInfo.Arguments = "/c " + cmd;
                    process1.StartInfo.CreateNoWindow = true;
                    process1.StartInfo.UseShellExecute = false;
                    process1.StartInfo.RedirectStandardOutput = true;
                    process1.StartInfo.RedirectStandardError = true;
                    process1.Start();
                    result = process1.StandardOutput.ReadToEnd()+process1.StandardError.ReadToEnd();
                    process1.WaitForExit();
                    process1.Close();

                }
                else
                {
                    notouch = false;
                    System.Diagnostics.Process process1 = new System.Diagnostics.Process();
                    process1.StartInfo.FileName = "cmd";
                    process1.StartInfo.Arguments = "/k " + cmd;
                    process1.Start();
                    result = "ほうりっぱモードがオンなので実行結果を返せません";
                    process1.Close ();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 0;
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            //受信を止める
            loop = false;

            label1.Text = "停止中";
            DateTime dt = DateTime.Now;

            form4.ApeLog($" [Info]: 停止しました\r\n");


            label1.ForeColor = Color.Red;
        }
        /// <summary>
        /// サーバーを操作する程度の能力
        /// </summary>
        /// <param name="cmd">コマンド内容。</param>
        /// <param name="rw">鯖に対して何の操作を行うか。arで初期化、rでサーバーへ記述</param>
        /// <param name="key">キー</param>
        /// <returns></returns>
        private void Post(string cmd, string rw ,string key,string url)
        {
            //第一引数に実行結果、第二引数は、動作(↓参照)、第三引数はキー
            //rwはrでコマンド実行結果を返す。arで初期化を行う
     
            WebClient client = new WebClient();
            NameValueCollection collection = new NameValueCollection();
            //collection.Add("title", "content");で引数指定
            collection.Add("sor", rw);
            collection.Add("key", key);
            collection.Add("cmd",cmd);

            client.UploadValues(url+"/index.php", collection);
            client.Dispose();

        }
        bool hide = false;
        private string result="初期状態";
        private bool ew;

        private void button3_Click(object sender, EventArgs e)
        {
            Hide();
            hide = true;
            if (loop == false)
            {
                notifyIcon1.BalloonTipText = "リモートコントロールは動いていません。設定してください。";
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;

            }
            else
            {
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;

                notifyIcon1.BalloonTipText = "バックグラウンドで正常に動いてます。\r\nタスクトレイからいつでも確認できます";

            }
            form4.ApeLog($" [Info]:バックグラウンドで実行中\r\n");
            notifyIcon1.ShowBalloonTip(3000);

        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                string shortcutPath = $@"C:\Users\{Environment.UserName}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\remotecontrol.lnk";
                string targetPath = Application.ExecutablePath;
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut =
                    (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.Arguments = "HIDESTART";
                
                shortcut.WorkingDirectory = Application.StartupPath;

                shortcut.Description = "RemoteControl";
                shortcut.IconLocation = Application.ExecutablePath + ",0";
                shortcut.Save();

                
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shortcut);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell);
            }
            else
            {
                File.Delete($@"C:\Users\{Environment.UserName}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\remotecontrol.lnk");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
           
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (Program.Hidestart==true)
            {
                Hide();
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
   
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        
        private void button5_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e) { 
        }


     

        private void button8_Click(object sender, EventArgs e)
        {

            form4.Show();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            Show();

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

      
    }
}
