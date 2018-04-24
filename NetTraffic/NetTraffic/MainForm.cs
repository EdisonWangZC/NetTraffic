using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace NetTraffic
{
    public partial class MainForm : Form
    {
        private static bool hovered = false;
        public MainForm()
        {
            InitializeComponent();
            this.Location = (Point)new Size(SystemInformation.WorkingArea.Right-this.Size.Width, (SystemInformation.WorkingArea.Bottom - SystemInformation.WorkingArea.Top)/4);
        }
        
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        [DllImport("User32.dll")]
        public static extern bool PtInRect(ref Rectangle r, Point p);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        [StructLayout(LayoutKind.Sequential)]
        internal struct MEMORYSTATUSEX //获得系统信息
        {
            internal uint dwLength;
            internal uint dwMemoryLoad;//内存使用率
            internal ulong ullTotalPhys;//物理内存
            internal ulong ullAvailPhys;//可用物理内存
            internal ulong ullTotalPageFile;//认可总量：总数
            internal ulong ullAvailPageFile;//认可总量：未用
            internal ulong ullTotalVirtual;//虚拟内存
            internal ulong ullAvailVirtual;//可用虚拟内存
        }
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll ", CharSet = CharSet.Auto, SetLastError = true)]//调用系统DLL（内存模块使用）
        static extern bool GlobalMemoryStatus(ref MEMORYSTATUSEX lpBuffer);//获得系统DLL里的函数

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.notifyIcon1.Text = "Net Monitor\r\nDouble click to show/hide main window";
            float netRecv = NetTrafficCore.GetNetReceived();
            float netSend = NetTrafficCore.GetNetSent();
            string netRecvText = "";
            string netSendText = "";              
            if (netRecv < 1024 * 1024)
            {
                netRecvText = (netRecv / 1024).ToString("0.0") + "KB/s";
            }
            else if (netRecv >= 1024 * 1024)
            {
                netRecvText = (netRecv / (1024 * 1024)).ToString("0.00") + "MB/s";
            }

            if (netSend < 1024 * 1024)
            {
                netSendText = (netSend / 1024).ToString("0.0") + "KB/s";
            }
            else if (netSend >= 1024 * 1024)
            {
                netSendText = (netSend / (1024 * 1024)).ToString("0.00") + "MB/s";
            }
            label1.Text = netSendText;
            label2.Text = netRecvText;

            MEMORYSTATUSEX vBuffer = new MEMORYSTATUSEX();//实例化结构
            GlobalMemoryStatus(ref vBuffer);//给此结构赋值
            string Memory = Convert.ToString(vBuffer.dwMemoryLoad);
            label5.Text = Memory + "%";
            if (Convert.ToInt32(vBuffer.dwMemoryLoad) < 75)
            {
                ModifyProgressBarColor.SetState(verticalProgressBar1, 1);
            }
            else if (Convert.ToInt32(vBuffer.dwMemoryLoad) >= 75 && Convert.ToInt32(vBuffer.dwMemoryLoad) < 90)
            {
                ModifyProgressBarColor.SetState(verticalProgressBar1, 3);
            }
            else if(Convert.ToInt32(vBuffer.dwMemoryLoad)>=90)
            {
                ModifyProgressBarColor.SetState(verticalProgressBar1, 2);
            }
            verticalProgressBar1.Value = Convert.ToInt32(vBuffer.dwMemoryLoad);

            timer1.Interval = 200;
            AutoSideHideOrShow();

        }

        void AutoSideHideOrShow()
        {
            int sideThickness = 4;//边缘的厚度，窗体停靠在边缘隐藏后留出来的可见部分的厚度  

            //如果窗体最小化或最大化了则什么也不做  
            if (this.WindowState == FormWindowState.Minimized || this.WindowState == FormWindowState.Maximized)
            {
                return;
            }

            //如果鼠标在窗体内  
            if (Cursor.Position.X >= this.Left && Cursor.Position.X < this.Right && Cursor.Position.Y >= this.Top && Cursor.Position.Y < this.Bottom)
            {
                //如果窗体离屏幕边缘很近，则自动停靠在该边缘  
                if (this.Top <= sideThickness)
                {
                    this.Top = 0;
                }
                if (this.Left <= sideThickness)
                {
                    this.Left = 0;
                }
                if (this.Left >= Screen.PrimaryScreen.WorkingArea.Width - this.Width - sideThickness)
                {
                    this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
                }
            }
            //当鼠标离开窗体以后  
            else
            {
                //隐藏到屏幕左边缘  
                if (this.Left == 0)
                {
                    this.Left = sideThickness - this.Width;
                }
                //隐藏到屏幕右边缘  
                else if (this.Left == Screen.PrimaryScreen.WorkingArea.Width - this.Width)
                {
                    this.Left = Screen.PrimaryScreen.WorkingArea.Width - sideThickness;
                }
                //隐藏到屏幕上边缘  
                else if (this.Top == 0 && this.Left > 0 && this.Left < Screen.PrimaryScreen.WorkingArea.Width - this.Width)
                {
                    this.Top = sideThickness - this.Height;
                }
            }
        }


        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);

            int screenRight = SystemInformation.WorkingArea.Right;//屏幕右边缘           
            if (screenRight - this.Right <= 10 && hovered == false)//往右靠
            {
                this.Location = new System.Drawing.Point(screenRight - this.Size.Width, this.Top);
            }
            int screenLeft = SystemInformation.WorkingArea.Left;
            if (this.Left - screenLeft <= 10)
                this.Location = new System.Drawing.Point(screenLeft, this.Top);

            int screenBottom = SystemInformation.WorkingArea.Bottom;
            if (screenBottom - this.Bottom <= 10)
                this.Location = new System.Drawing.Point(this.Left, screenBottom - this.Size.Height);

            int screenTop = SystemInformation.WorkingArea.Top;
            if (this.Top - screenTop <= 10)
                this.Location = new System.Drawing.Point(this.Left, screenTop);

            if (e.Button == MouseButtons.Right)
            {
            
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && e.Button == MouseButtons.Left)
            {
                this.Show();
                隐藏ToolStripMenuItem.Text = "隐藏";
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && e.Button == MouseButtons.Left)
            {
                this.Show();
                隐藏ToolStripMenuItem.Text = "隐藏";
                this.WindowState = FormWindowState.Normal;
            }
            else if(this.WindowState == FormWindowState.Normal && e.Button == MouseButtons.Left)
            {
                this.Hide();
                隐藏ToolStripMenuItem.Text = "显示";
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定退出?", "Title", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                this.Dispose();
                this.Close();
            }
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }

        private void 开机自启动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                if (开机自启动ToolStripMenuItem.Checked)
                {

                    string StartupPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Startup);
                    Console.WriteLine(StartupPath);
                    //获得文件的当前路径
                    string dir = Directory.GetCurrentDirectory();                  
                    string exeDir = dir + @"\NetTraffic.exe";
                    Console.WriteLine(exeDir);
                    System.IO.File.Copy(exeDir, StartupPath + @"\NetTraffic.lnk", true);
                }
                else
                {
                    string StartupPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Startup);
                    System.IO.File.Delete(StartupPath + @"\NetTraffic.lnk");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            }

        private void 隐藏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (隐藏ToolStripMenuItem.Text == "隐藏")
            {
                this.Hide();
                notifyIcon1.BalloonTipTitle = "注意";
                notifyIcon1.BalloonTipText = "双击重新打开我";
                notifyIcon1.ShowBalloonTip(1000);
                隐藏ToolStripMenuItem.Text = "显示";
            }
            else
            {
                this.Show();
                隐藏ToolStripMenuItem.Text = "隐藏";
            }
        }

        private void 详细信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dtinfor = new Form1();
            dtinfor.Show();
        }

        private void 结束进程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var proc_end = new Form2();
            proc_end.Show();
        }
    }
}
