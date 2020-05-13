using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Youtube_Downloader
{
    //delegate
    public delegate void DelReadStdOutput(string result);
    public delegate void DelReadErrOutput(string result);

    public partial class Form1 : Form
    {
        //delegate event
        public event DelReadStdOutput ReadStdOutput;
        public event DelReadErrOutput ReadErrOutput;

        String dl_1 = @"C:\youtube-dl.exe";
        String dl_2 = Application.StartupPath + "\\youtube-dl.exe";

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            //3.将相应函数注册到委托事件中
            ReadStdOutput += new DelReadStdOutput(ReadStdOutputAction);
            ReadErrOutput += new DelReadErrOutput(ReadErrOutputAction);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            String url = Clipboard.GetText();

            if (url.Substring(0, 4) == "http")
            {
                Url.Text = url;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine(dl_2);
            if (File.Exists(dl_1) && File.Exists(dl_2))
            {
                Console.WriteLine(dl_1.ToString());
                Console.WriteLine(dl_2.ToString());
                TSSL4.Text = "All exists";
                TSSL4.BackColor = Color.Green;
            }
            else if (File.Exists(dl_1))
            {
                TSSL4.Text = "External";
                TSSL4.BackColor = Color.Blue;
            }
            else if (File.Exists(dl_2))
            {
                TSSL4.Text = "Internal";
                TSSL4.BackColor = Color.Blue;
            }
            else
            {
                TSSL4.Text = "Unavailable";
                TSSL4.BackColor = Color.Gray;
            }
        }

        private void Download_Click(object sender, EventArgs e)
        {
            if (TSSL4.Text != "Unavailable")
            {
                try
                {
                    richTextBox1.Clear();
                    Process process = new Process();
                    ProcessStartInfo startInfo = process.StartInfo;
                    startInfo.WindowStyle = ProcessWindowStyle.Normal;

                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/c " + dl_1 + " " + Url.Text;
                    Console.WriteLine(startInfo.Arguments.ToString());

                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardInput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.CreateNoWindow = true;

                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.OutputDataReceived += new DataReceivedEventHandler(OnDataReceived);
                    process.ErrorDataReceived += new DataReceivedEventHandler(OnErrorReceived);
                    process.EnableRaisingEvents = true;
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    richTextBox1.AppendText("Downloading job is starting...\r\n");
                }
                catch (Exception ex)
                {
                    MessageBox.Show( ex.Message.ToString(),"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
               
                MessageBox.Show("Component file not found, unable download.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                // 4. 异步调用，需要invoke
                this.Invoke(ReadStdOutput, new object[] { e.Data });
            }
        }

        private void OnErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                this.Invoke(ReadErrOutput, new object[] { e.Data });
            }
        }

        private void ReadStdOutputAction(string result)
        {
            this.richTextBox1.AppendText(result + "\r\n");
        }

        private void ReadErrOutputAction(string result)
        {
            this.richTextBox1.AppendText(result + "\r\n");
        }

        private void CmdProcess_Exited(object sender, EventArgs e)
        {
            // 执行结束后触发
        }

        private void TSSL4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Showes the status of the component file(youtube-dl.exe)", "Status",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void Update_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = process.StartInfo;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;

                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/c " + "youtube-dl.exe -U";
                Console.WriteLine(startInfo.Arguments.ToString());

                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = true;

                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.OutputDataReceived += new DataReceivedEventHandler(OnDataReceived);
                process.ErrorDataReceived += new DataReceivedEventHandler(OnErrorReceived);

                process.EnableRaisingEvents = true;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message.ToString(),"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
   
}
