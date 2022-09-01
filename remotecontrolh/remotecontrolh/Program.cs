using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace remotecontrolh
{
    static class Program
    {
        static public bool Hidestart = false;
        static public bool Restart = false;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string mutexName = "REMOKONSNDDAD";
            //Mutexオブジェクトを作成する
            System.Threading.Mutex mutex = new System.Threading.Mutex(false, mutexName);
            bool hasHandle = false;
            try
            {
                try
                {
                    //ミューテックスの所有権を要求する
                    hasHandle = mutex.WaitOne(0, false);
                }
                //.NET Framework 2.0以降の場合
                catch (System.Threading.AbandonedMutexException)
                {
                    //別のアプリケーションがミューテックスを解放しないで終了した時
                    hasHandle = true;
                }
                //ミューテックスを得られたか調べる
                if (hasHandle == false)
                {
                    //得られなかった場合は、すでに起動していると判断して終了
                    MessageBox.Show("多重起動はできません。\rタスクトレイを見てみてください","エラー",MessageBoxButtons.OK,MessageBoxIcon.Stop);

                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                foreach (string s in args)
                {
                    if (string.Join("", args) == "HIDESTART")
                    {
                        Hidestart = true;
                    }
                   else if (string.Join("", args) == "RESTART")
                    {
                        Restart = true;
                    }
                }
           


                Application.Run(new Form1());
            }
            finally
            {
                if (hasHandle)
                {
                    //ミューテックスを解放する
                    mutex.ReleaseMutex();
                }
                mutex.Close();
            }
        }
        static bool Hideinfo()
        {
            return Hidestart;
        }
    }
}
