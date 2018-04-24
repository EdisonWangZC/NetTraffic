using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpPcap;
using PacketDotNet;
using System.Text.RegularExpressions;
using System.Net;

namespace NetTraffic
{

    public partial class Form1 : Form
    {
        public ProcessPerformanceInfo ProcInfo = new ProcessPerformanceInfo();
        public Form1()
        {

            InitializeComponent();

            lvwFile.Columns.Add("进程名称", 250);
            lvwFile.Columns.Add("进程ID", 100);
            lvwFile.Columns.Add("上传速度", 100);
            lvwFile.Columns.Add("下载速度", 100);

            lvwFile.View = View.Details;
            //    Process[] proc = Process.GetProcesses();
            //    calcute(proc);
        }
    
    public class ProcessPerformanceInfo : IDisposable
    {
        public int ProcessID { get; set; }//进程ID
        public string ProcessName { get; set; }//进程名
        public float PrivateWorkingSet { get; set; }//私有工作集(KB)
        public float WorkingSet { get; set; }//工作集(KB)
        public float CpuTime { get; set; }//CPU占用率(%)
        public float IOOtherBytes { get; set; }//每秒IO操作（不包含控制操作）读写数据的字节数(KB)
        public int IOOtherOperations { get; set; }//每秒IO操作数（不包括读写）(个数)
        public long NetSendBytes { get; set; }//网络发送数据字节数
        public long NetRecvBytes { get; set; }//网络接收数据字节数
        public long NetTotalBytes { get; set; }//网络数据总字节数
        public List<ICaptureDevice> dev = new List<ICaptureDevice>();

        /// <summary>
        /// 实现IDisposable的方法
        /// </summary>
        public void Dispose()
        {
            foreach (ICaptureDevice d in dev)
            {
                d.StopCapture();
                d.Close();
            }
        }
    }
        private void device_OnPacketArrivalSend(object sender, CaptureEventArgs e)
        {
            var len = e.Packet.Data.Length;
            ProcInfo.NetSendBytes += len;
        }

        private void device_OnPacketArrivalRecv(object sender, CaptureEventArgs e)
        {
            var len = e.Packet.Data.Length;
            ProcInfo.NetRecvBytes += len;
        }
        public void CaptureFlowSend(string IP, int portID, int deviceID)
        {
            ICaptureDevice device = (ICaptureDevice)CaptureDeviceList.New()[deviceID];
            
            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrivalSend);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            string filter = "src host " + IP + " and src port " + portID;
            device.Filter = filter;
            device.StartCapture();
            ProcInfo.dev.Add(device);
        }

        public void CaptureFlowRecv(string IP, int portID, int deviceID)
        {
            ICaptureDevice device = CaptureDeviceList.New()[deviceID];
            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrivalRecv);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            string filter = "dst host " + IP + " and dst port " + portID;
            device.Filter = filter;
            device.StartCapture();
            ProcInfo.dev.Add(device);
        }

        public void RefershInfo()
        {
            ProcInfo.NetRecvBytes = 0;
            ProcInfo.NetSendBytes = 0;
            ProcInfo.NetTotalBytes = 0;
            Thread.Sleep(1000);
            ProcInfo.NetTotalBytes = ProcInfo.NetRecvBytes + ProcInfo.NetSendBytes;
        }

        public void calcute(Process[] proc)
        {
            int k = 0;
            foreach (var item in proc)
            {
              //  lvwFile.Items.Clear();
                var item2 = new ListViewItem(item.ProcessName);
                item2.SubItems.Add(item.Id.ToString());
                ProcInfo.ProcessID = item.Id;
                int pid = ProcInfo.ProcessID;
                List<int> ports = new List<int>();
                #region 获取指定进程对应端口号
                Process pro = new Process();
                pro.StartInfo.FileName = "cmd.exe";
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.RedirectStandardInput = true;
                pro.StartInfo.RedirectStandardOutput = true;
                pro.StartInfo.RedirectStandardError = true;
                pro.StartInfo.CreateNoWindow = true;
                pro.Start();
                pro.StandardInput.WriteLine("netstat -ano");
                pro.StandardInput.WriteLine("exit");
                Regex reg = new Regex("\\s+", RegexOptions.Compiled);
                string line = null;
                ports.Clear();
                
                while ((line = pro.StandardOutput.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("TCP", StringComparison.OrdinalIgnoreCase))
                    {
                        line = reg.Replace(line, ",");
                        string[] arr = line.Split(',');
                        if (arr[4] == pid.ToString())
                        {
                            string soc = arr[1];
                            int pos = soc.LastIndexOf(':');
                            int pot = int.Parse(soc.Substring(pos + 1));
                            ports.Add(pot);
                        }
                    }
                    else if (line.StartsWith("UDP", StringComparison.OrdinalIgnoreCase))
                    {
                        line = reg.Replace(line, ",");
                        string[] arr = line.Split(',');
                        if (arr[3] == pid.ToString())
                        {
                            string soc = arr[1];
                            int pos = soc.LastIndexOf(':');
                            int pot = int.Parse(soc.Substring(pos + 1));
                            ports.Add(pot);
                        }
                    }
                }
                pro.Close();
                #endregion
                IPAddress[] addrList = Dns.GetHostByName(Dns.GetHostName()).AddressList;
                string IP = addrList[0].ToString();
                //获取本机网络设备
                var devices = CaptureDeviceList.Instance;
                int count = devices.Count;
                if (count < 1)
                {
                    Console.WriteLine("No device found on this machine");
                    return;
                }
                for (int i = 0; i < count; ++i)
                {
                    for (int j = 0; j < ports.Count; ++j)
                    {
                        CaptureFlowRecv(IP, ports[j], i);
                        CaptureFlowSend(IP, ports[j], i);
                    }
                }
               
                    Console.WriteLine(k.ToString() + " " + ProcInfo.ProcessID + "proc NetSendBytes : " + ProcInfo.NetSendBytes);
                    Console.WriteLine(k.ToString() + " " + ProcInfo.ProcessID + "proc NetRecvBytes : " + ProcInfo.NetRecvBytes);
                    k++;

             
               

                    item2.SubItems.Add(ProcInfo.NetSendBytes.ToString());
                    item2.SubItems.Add(ProcInfo.NetRecvBytes.ToString());
                    this.lvwFile.Items.Add(item2);
                ProcInfo.NetRecvBytes = 0;
                ProcInfo.NetSendBytes = 0;
                //每隔1s调用刷新函数对性能参数进行刷新
                //  RefershInfo();

                //最后要记得调用Dispose方法停止抓包并关闭设备
                  ProcInfo.Dispose();


            }
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
         ;
                /// <summary>
                /// 实时刷新性能参数
                /// </summary>

            //item2.SubItems.Add(item.Id.ToString());
            //item2.SubItems.Add(ProcInfo.NetTotalBytes.ToString());
            //item2.SubItems.Add( ProcInfo.NetSendBytes.ToString());
            //item2.SubItems.Add(ProcInfo.NetRecvBytes.ToString());
            //this.lvwFile.Items.Add(item2);
            //Console.WriteLine("proc NetTotalBytes : " + ProcInfo.NetTotalBytes);
            //          Console.WriteLine("proc NetSendBytes : " + ProcInfo.NetSendBytes);
            //          Console.WriteLine("proc NetRecvBytes : " + ProcInfo.NetRecvBytes);
         
            }
        
            //lvwFile.Items.Clear();
           
            //foreach (var item in proc)
            //{
            //    //   string strFullPath = Application.ExecutablePath;
            //    //  string strFileName = System.IO.Path.GetFileName(strFullPath);
            //    var item2 = new ListViewItem(item.ProcessName);
            //    item2.SubItems.Add(item.Id.ToString());
            // //   i++;
            //    this.lvwFile.Items.Add(item2);
            //}
           
        private void 关闭此进程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            //if (lvwFile.SelectedItems.Count >= 1)
            //{
            //    Process p = Process.GetProcessById(Convert.ToInt32(lvwFile.SelectedItems[0].SubItems[1].Text));
            //    if (MessageBox.Show("确定要结束进程吗？", "结束进程", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            //    {
            //        if (p != null)
            //            p.Kill();
            //    }
            //}
        }

        private void 开始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process[] proc = Process.GetProcesses();
            calcute(proc);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
