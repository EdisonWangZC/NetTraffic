using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetTraffic
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            lvwFile.Columns.Add("进程名称", 250);
            lvwFile.Columns.Add("进程ID", 100);
            lvwFile.Columns.Add("序号", 100);
            lvwFile.View = View.Details;
            timer1.Start();
        }

        private void 结束进程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvwFile.SelectedItems.Count >= 1)
            {
                Process p = Process.GetProcessById(Convert.ToInt32(lvwFile.SelectedItems[0].SubItems[1].Text));
                if (MessageBox.Show("确定要结束进程吗？", "结束进程", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    if (p != null)
                        p.Kill();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int k = 0;
            lvwFile.Items.Clear();
            Process[] proc = Process.GetProcesses();
            foreach (var item in proc)
            {
                var item2 = new ListViewItem(item.ProcessName);
                item2.SubItems.Add(item.Id.ToString());
                item2.SubItems.Add(k.ToString());
                this.lvwFile.Items.Add(item2);
                k++;


            }
        }
    }
}
